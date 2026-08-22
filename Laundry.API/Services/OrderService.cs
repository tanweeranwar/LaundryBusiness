using Laundry.API.DTOs.Orders;
using Laundry.API.Entities;
using Laundry.API.Enums;
using Laundry.API.Interfaces;
using Laundry.API.Models.Pricing;
using Laundry.API.Repositories;
using Laundry.API.Exceptions;

namespace Laundry.API.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly IPricingService _pricingService;
    private readonly IOrderNumberGenerator _orderNumberGenerator;

    public OrderService(
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
        IBranchRepository branchRepository,
        IPricingService pricingService,
        IOrderNumberGenerator orderNumberGenerator)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _branchRepository = branchRepository;
        _pricingService = pricingService;
        _orderNumberGenerator = orderNumberGenerator;
    }

    public async Task<OrderDto> CreateAsync(CreateOrderDto request)
    {
        ArgumentNullException.ThrowIfNull(request);

        await ValidateCreateRequestAsync(request);

        PricingResult pricing = await _pricingService.CalculateAsync(
            request.BranchId,
            request.Items,
            request.DiscountAmount);

        var order = new Order
        {
            OrderNumber = await _orderNumberGenerator.GenerateAsync(),
            BranchId = request.BranchId,
            CustomerId = request.CustomerId,
            OrderDate = DateTime.Now,

            ExpectedDeliveryDate = DateTime.SpecifyKind(
                request.ExpectedDeliveryDate,
                DateTimeKind.Local),

            Status = OrderStatus.Created,

            PaymentStatus = OrderPaymentStatus.Pending,

            Subtotal = pricing.Subtotal,
            DiscountAmount = pricing.Discount,
            TaxAmount = pricing.Tax,
            GrandTotal = pricing.GrandTotal,
            BalanceAmount = pricing.GrandTotal,

            Remarks = request.Remarks
        };

        foreach (var item in pricing.Items)
        {
            order.Items.Add(new OrderItem
            {
                ServiceCategoryId = item.ServiceCategoryId,
                GarmentTypeId = item.GarmentTypeId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                ExpressService = item.ExpressService,
                ExpressUnitPrice = item.ExpressUnitPrice,
                LineTotal = item.LineTotal
            });
        }

        await _orderRepository.AddAsync(order);
        await _orderRepository.SaveChangesAsync();

        var savedOrder =
            await _orderRepository.GetOrderWithItemsAsync(order.Id);

        if (savedOrder == null)
        {
            throw new InvalidOperationException(
                "Order was created but could not be loaded.");
        }

        return MapToOrderDto(savedOrder);
    }

    public async Task<OrderDto?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id));

        var order = await _orderRepository.GetOrderWithItemsAsync(id);

        return order == null ? null : MapToOrderDto(order);
    }

    public async Task<OrderDto?> GetByOrderNumberAsync(string orderNumber)
    {
        if (string.IsNullOrWhiteSpace(orderNumber))
            throw new ArgumentException(
                "Order number is required.",
                nameof(orderNumber));

        var order = await _orderRepository
            .GetByOrderNumberAsync(orderNumber.Trim());

        if (order == null)
            return null;

        var completeOrder =
            await _orderRepository.GetOrderWithItemsAsync(order.Id);

        return completeOrder == null
            ? null
            : MapToOrderDto(completeOrder);
    }

    public async Task<IEnumerable<OrderSummaryDto>> GetByCustomerAsync(
        Guid customerId)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException(
                "Customer Id is invalid.",
                nameof(customerId));

        var orders =
            await _orderRepository.GetOrdersByCustomerAsync(customerId);

        return orders
            .OrderByDescending(x => x.OrderDate)
            .Select(MapToSummaryDto)
            .ToList();
    }

    public async Task<IEnumerable<OrderSummaryDto>> GetByBranchAsync(
        int branchId)
    {
        if (branchId <= 0)
            throw new ArgumentOutOfRangeException(nameof(branchId));

        var orders =
            await _orderRepository.GetOrdersByBranchAsync(branchId);

        return orders
            .OrderByDescending(x => x.OrderDate)
            .Select(MapToSummaryDto)
            .ToList();
    }

    public async Task<bool> UpdateStatusAsync(
        int id,
        UpdateOrderDto request)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id));

        ArgumentNullException.ThrowIfNull(request);

        if (!Enum.IsDefined(typeof(OrderStatus), request.Status))
        {
            throw new InvalidOrderStatusTransitionException(
                $"Order status '{request.Status}' is not valid.");
        }

        var order = await _orderRepository.GetTrackedByIdAsync(id);

        if (order == null)
            return false;

        var newStatus = (OrderStatus)request.Status;

        if (order.Status == newStatus &&
            string.Equals(
                order.Remarks,
                request.Remarks,
                StringComparison.Ordinal))
        {
            return true;
        }

        ValidateStatusTransition(order.Status, newStatus);

        order.Status = newStatus;
        order.Remarks = request.Remarks;
        order.UpdatedOn = DateTime.UtcNow;

        _orderRepository.Update(order);

        await _orderRepository.SaveChangesAsync();

        return true;
    }

    private static void ValidateStatusTransition(
        OrderStatus currentStatus,
        OrderStatus newStatus)
    {
        if (currentStatus == OrderStatus.Delivered)
        {
            throw new InvalidOrderStatusTransitionException(
                "A delivered order cannot be moved to another status.");
        }

        if (currentStatus == OrderStatus.Cancelled)
        {
            throw new InvalidOrderStatusTransitionException(
                "A cancelled order cannot be moved to another status.");
        }

        if (newStatus == OrderStatus.Created)
        {
            throw new InvalidOrderStatusTransitionException(
                "An order cannot be moved back to Created status.");
        }

        if (newStatus == OrderStatus.Cancelled)
        {
            if (currentStatus == OrderStatus.Ready ||
                currentStatus == OrderStatus.OutForDelivery)
            {
                throw new InvalidOrderStatusTransitionException(
                    $"Order cannot be cancelled after it reaches {currentStatus}.");
            }

            return;
        }

        var isValid = currentStatus switch
        {
            OrderStatus.Created =>
                newStatus == OrderStatus.Received,

            OrderStatus.Received =>
                newStatus == OrderStatus.Washing ||
                newStatus == OrderStatus.DryCleaning,

            OrderStatus.Washing =>
                newStatus == OrderStatus.Ironing,

            OrderStatus.DryCleaning =>
                newStatus == OrderStatus.Ironing,

            OrderStatus.Ironing =>
                newStatus == OrderStatus.QualityCheck,

            OrderStatus.QualityCheck =>
                newStatus == OrderStatus.Ready,

            OrderStatus.Ready =>
                newStatus == OrderStatus.OutForDelivery,

            OrderStatus.OutForDelivery =>
                newStatus == OrderStatus.Delivered,

            _ => false
        };

        if (!isValid)
        {
            throw new InvalidOrderStatusTransitionException(
                $"Invalid order status transition: " +
                $"{currentStatus} -> {newStatus}.");
        }
    }

    private async Task ValidateCreateRequestAsync(
        CreateOrderDto request)
    {
        if (!await _branchRepository.ExistsAsync(request.BranchId))
        {
            throw new InvalidOperationException(
                $"Branch '{request.BranchId}' does not exist.");
        }

        if (!await _customerRepository.ExistsAsync(request.CustomerId))
        {
            throw new InvalidOperationException(
                $"Customer '{request.CustomerId}' does not exist.");
        }

        ValidateExpectedDeliveryDate(
            request.ExpectedDeliveryDate);

        if (request.Items == null || !request.Items.Any())
        {
            throw new InvalidOperationException(
                "An order must contain at least one item.");
        }
    }

    private static void ValidateExpectedDeliveryDate(
        DateTime expectedDeliveryDate)
    {
        if (expectedDeliveryDate.Date < DateTime.Today)
        {
            throw new InvalidOperationException(
                "Expected delivery date cannot be in the past.");
        }
    }

    private static OrderDto MapToOrderDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            BranchId = order.BranchId,
            CustomerId = order.CustomerId,
            OrderDate = order.OrderDate,
            ExpectedDeliveryDate = order.ExpectedDeliveryDate,
            Status = (int)order.Status,
            Subtotal = order.Subtotal,
            DiscountAmount = order.DiscountAmount,
            TaxAmount = order.TaxAmount,
            GrandTotal = order.GrandTotal,
            PaymentStatus = (int)order.PaymentStatus,
            BalanceAmount = order.BalanceAmount,
            Remarks = order.Remarks,

            Items = order.Items.Select(i => new OrderItemDto
            {
                Id = i.Id,
                ServiceCategoryId = i.ServiceCategoryId,
                ServiceCategoryName =
                    i.ServiceCategory?.Name ?? string.Empty,
                GarmentTypeId = i.GarmentTypeId,
                GarmentTypeName =
                    i.GarmentType?.Name ?? string.Empty,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                ExpressService = i.ExpressService,
                ExpressUnitPrice = i.ExpressUnitPrice,
                LineTotal = i.LineTotal,
                Notes = i.Notes
            }).ToList()
        };
    }

    private static OrderSummaryDto MapToSummaryDto(Order order)
    {
        return new OrderSummaryDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            CustomerId = order.CustomerId,

            CustomerName = order.Customer == null
                ? string.Empty
                : $"{order.Customer.FirstName} {order.Customer.LastName}".Trim(),

            OrderDate = order.OrderDate,
            ExpectedDeliveryDate = order.ExpectedDeliveryDate,
            Status = (int)order.Status,
            GrandTotal = order.GrandTotal
        };
    }
}
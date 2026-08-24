using Laundry.API.Data;
using Laundry.API.DTOs.Payment;
using Laundry.API.Entities;
using Laundry.API.Enums;
using Laundry.API.Interfaces;
using Laundry.API.Repositories.Interfaces;
using Laundry.API.Services.Interfaces;

namespace Laundry.API.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentNumberGenerator _paymentNumberGenerator;
    private readonly LaundryDbContext _context;

    public PaymentService(
        IPaymentRepository paymentRepository,
        IOrderRepository orderRepository,
        IPaymentNumberGenerator paymentNumberGenerator,
        LaundryDbContext context)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _paymentNumberGenerator = paymentNumberGenerator;
        _context = context;
    }

    public async Task<PaymentDto> CreateAsync(CreatePaymentDto request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!Enum.IsDefined(typeof(PaymentMethod), request.PaymentMethod))
            throw new InvalidOperationException("Invalid payment method.");

        var order = await _orderRepository.GetTrackedByIdAsync(request.OrderId);

        if (order == null)
            throw new InvalidOperationException("Order not found.");

        if (request.Amount <= 0)
            throw new InvalidOperationException(
                "Payment amount must be greater than zero.");

        if (request.Amount > order.BalanceAmount)
            throw new InvalidOperationException(
                "Payment amount cannot exceed the balance amount.");

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        var payment = new Payment
        {
            PaymentNumber = await _paymentNumberGenerator.GenerateAsync(),
            OrderId = request.OrderId,
            Amount = request.Amount,
            PaymentMethod = (PaymentMethod)request.PaymentMethod,
            PaymentStatus = PaymentStatus.Completed,
            TransactionReference = request.TransactionReference,
            Remarks = request.Remarks,
            ReceivedBy = request.ReceivedBy,
            PaidOn = DateTime.Now
        };

        await _paymentRepository.AddAsync(payment);

        var totalPaid = await _paymentRepository.GetTotalPaidAsync(order.Id);
        totalPaid += payment.Amount;

        UpdateOrderPaymentStatus(order, totalPaid);
        _orderRepository.Update(order);

        await _paymentRepository.SaveChangesAsync();
        await transaction.CommitAsync();

        return Map(payment);
    }

    public async Task<PaymentDto?> GetByIdAsync(int id)
    {
        var payment = await _paymentRepository.GetByIdAsync(id);

        return payment == null ? null : Map(payment);
    }

    public async Task<IEnumerable<PaymentDto>> GetByOrderIdAsync(int orderId)
    {
        var payments = await _paymentRepository.GetByOrderIdAsync(orderId);

        return payments.Select(Map).ToList();
    }

    public async Task<PaymentDto> UpdateAsync(
        int id,
        UpdatePaymentDto request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!Enum.IsDefined(typeof(PaymentStatus), request.PaymentStatus))
            throw new InvalidOperationException("Invalid payment status.");

        var payment = await _paymentRepository.GetByIdAsync(id);

        if (payment == null)
            throw new InvalidOperationException("Payment not found.");

        var newStatus = (PaymentStatus)request.PaymentStatus;

        ValidateStatusTransition(payment.PaymentStatus, newStatus);

        payment.PaymentStatus = newStatus;
        payment.Remarks = request.Remarks;

        _paymentRepository.Update(payment);

        await _paymentRepository.SaveChangesAsync();

        var order = await _orderRepository.GetTrackedByIdAsync(payment.OrderId);

        if (order != null)
        {
            var totalPaid = await _paymentRepository.GetTotalPaidAsync(order.Id);

            UpdateOrderPaymentStatus(order, totalPaid);
            _orderRepository.Update(order);

            await _paymentRepository.SaveChangesAsync();
        }

        return Map(payment);
    }

    private static void ValidateStatusTransition(
        PaymentStatus current,
        PaymentStatus requested)
    {
        if (current == requested)
            return;

        var valid = current switch
        {
            PaymentStatus.Pending =>
                requested == PaymentStatus.Completed ||
                requested == PaymentStatus.Cancelled,

            PaymentStatus.Completed =>
                requested == PaymentStatus.Refunded,

            PaymentStatus.Refunded => false,
            PaymentStatus.Cancelled => false,
            _ => false
        };

        if (!valid)
        {
            throw new InvalidOperationException(
                $"Invalid payment status transition: {current} -> {requested}.");
        }
    }

    private static void UpdateOrderPaymentStatus(
        Order order,
        decimal totalPaid)
    {
        order.BalanceAmount = Math.Max(0, order.GrandTotal - totalPaid);

        if (order.BalanceAmount == 0)
        {
            order.PaymentStatus = OrderPaymentStatus.Paid;
        }
        else if (totalPaid > 0)
        {
            order.PaymentStatus = OrderPaymentStatus.PartiallyPaid;
        }
        else
        {
            order.PaymentStatus = OrderPaymentStatus.Pending;
        }
    }

    private static PaymentDto Map(Payment payment)
    {
        return new PaymentDto
        {
            Id = payment.Id,
            PaymentNumber = payment.PaymentNumber,
            OrderId = payment.OrderId,
            Amount = payment.Amount,
            PaymentMethod = payment.PaymentMethod,
            PaymentStatus = payment.PaymentStatus,
            TransactionReference = payment.TransactionReference,
            Remarks = payment.Remarks,
            PaidOn = payment.PaidOn,
            ReceivedBy = payment.ReceivedBy
        };
    }
}

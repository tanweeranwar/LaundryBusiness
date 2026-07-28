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

    public PaymentService(
        IPaymentRepository paymentRepository,
        IOrderRepository orderRepository,
        IPaymentNumberGenerator paymentNumberGenerator)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _paymentNumberGenerator = paymentNumberGenerator;
    }

    public async Task<PaymentDto> CreateAsync(CreatePaymentDto request)
    {
        var order = await _orderRepository.GetTrackedByIdAsync(request.OrderId);

        if (order == null)
            throw new Exception("Order not found.");

        if (request.Amount <= 0)
            throw new Exception("Payment amount must be greater than zero.");

        if (request.Amount > order.BalanceAmount)
            throw new Exception("Payment amount cannot exceed the balance amount.");

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

        order.BalanceAmount = Math.Max(0, order.GrandTotal - totalPaid);

        if (order.BalanceAmount == 0)
            order.PaymentStatus = OrderPaymentStatus.Paid;
        else if (totalPaid > 0)
            order.PaymentStatus = OrderPaymentStatus.PartiallyPaid;
        else
            order.PaymentStatus = OrderPaymentStatus.Pending;

        _orderRepository.Update(order);

        await _paymentRepository.SaveChangesAsync();

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

    public async Task<PaymentDto> UpdateAsync(int id, UpdatePaymentDto request)
    {
        var payment = await _paymentRepository.GetByIdAsync(id);

        if (payment == null)
            throw new Exception("Payment not found.");

        payment.PaymentStatus = (PaymentStatus)request.PaymentStatus;
        payment.Remarks = request.Remarks;

        _paymentRepository.Update(payment);

        var order = await _orderRepository.GetTrackedByIdAsync(payment.OrderId);

        if (order != null)
        {
            var totalPaid = await _paymentRepository.GetTotalPaidAsync(order.Id);

            order.BalanceAmount = Math.Max(0, order.GrandTotal - totalPaid);

            order.PaymentStatus = order.BalanceAmount == 0
                ? OrderPaymentStatus.Paid
                : totalPaid > 0
                    ? OrderPaymentStatus.PartiallyPaid
                    : OrderPaymentStatus.Pending;

            _orderRepository.Update(order);
        }

        await _paymentRepository.SaveChangesAsync();

        return Map(payment);
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
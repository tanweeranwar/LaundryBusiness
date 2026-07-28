using Laundry.API.Enums;

namespace Laundry.API.DTOs.Payment;

public class PaymentDto
{
    public int Id { get; set; }

    public string PaymentNumber { get; set; } = string.Empty;

    public int OrderId { get; set; }

    public decimal Amount { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public PaymentStatus PaymentStatus { get; set; }

    public string? TransactionReference { get; set; }

    public string? Remarks { get; set; }

    public DateTime PaidOn { get; set; }

    public string? ReceivedBy { get; set; }
}
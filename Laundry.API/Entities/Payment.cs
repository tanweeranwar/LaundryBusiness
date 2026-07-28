using Laundry.API.Enums;

namespace Laundry.API.Entities;

public class Payment : BaseEntity
{
    public int Id { get; set; }

    public string PaymentNumber { get; set; } = string.Empty;

    public int OrderId { get; set; }

    public decimal Amount { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    public string? TransactionReference { get; set; }

    public string? Remarks { get; set; }

    public DateTime PaidOn { get; set; } = DateTime.Now;

    public string? ReceivedBy { get; set; }

    public virtual Order Order { get; set; } = null!;
}
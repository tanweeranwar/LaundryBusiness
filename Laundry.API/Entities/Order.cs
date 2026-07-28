using Laundry.API.Enums;

namespace Laundry.API.Entities;

public class Order : BaseEntity
{
    public int Id { get; set; }

    public string OrderNumber { get; set; } = string.Empty;

    public int BranchId { get; set; }

    public Guid CustomerId { get; set; }

    public DateTime OrderDate { get; set; }

    public DateTime ExpectedDeliveryDate { get; set; }

    public OrderStatus Status { get; set; }

    public decimal Subtotal { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal GrandTotal { get; set; }
    public decimal BalanceAmount { get; set; }
    //public PaymentStatus PaymentStatus { get; set; }
    public decimal TotalPaid => GrandTotal - BalanceAmount;
    public OrderPaymentStatus PaymentStatus { get; set; }

    public ICollection<Payment> Payments { get; set; }
        = new List<Payment>();

    public string? Remarks { get; set; }

    // Navigation Properties
    public Branch Branch { get; set; } = null!;

    public Customer Customer { get; set; } = null!;

    public ICollection<OrderItem> Items { get; set; }
        = new List<OrderItem>();
}
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

    public PaymentStatus PaymentStatus { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public string? Remarks { get; set; }

    // Navigation Properties
    public Branch Branch { get; set; } = null!;

    public Customer Customer { get; set; } = null!;

    public ICollection<OrderItem> Items { get; set; }
        = new List<OrderItem>();
}
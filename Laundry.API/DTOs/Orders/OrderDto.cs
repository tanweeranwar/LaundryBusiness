namespace Laundry.API.DTOs.Orders;

public class OrderDto
{
    public int Id { get; set; }

    public string OrderNumber { get; set; } = string.Empty;

    public int BranchId { get; set; }

    public Guid CustomerId { get; set; }

    public DateTime OrderDate { get; set; }

    public DateTime ExpectedDeliveryDate { get; set; }

    public int Status { get; set; }

    public decimal Subtotal { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal GrandTotal { get; set; }

    public decimal BalanceAmount { get; set; }

    public int PaymentStatus { get; set; }

    public string? Remarks { get; set; }

    public List<OrderItemDto> Items { get; set; }
        = new();
}
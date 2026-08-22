using Laundry.API.Enums;

namespace Laundry.API.Entities;

public class OrderStatusHistory
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public OrderStatus FromStatus { get; set; }

    public OrderStatus ToStatus { get; set; }

    public string? Remarks { get; set; }

    public string? ChangedBy { get; set; }

    public DateTime ChangedOn { get; set; }

    public Order Order { get; set; } = null!;
}
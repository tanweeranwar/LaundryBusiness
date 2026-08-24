namespace Laundry.API.Entities;

public class OrderItem : BaseEntity
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int ServiceCategoryId { get; set; }

    public int GarmentTypeId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public bool ExpressService { get; set; }

    public decimal? ExpressUnitPrice { get; set; }

    public decimal LineTotal { get; set; }

    public string? Notes { get; set; }

    // Navigation Properties
    public Order Order { get; set; } = null!;

    public ServiceCategory ServiceCategory { get; set; } = null!;

    public GarmentType GarmentType { get; set; } = null!;

    public OrderItemProcessing? OrderItemProcessing { get; set; }
}
namespace Laundry.API.DTOs.Orders;

public class OrderItemDto
{
    public int Id { get; set; }

    public int ServiceCategoryId { get; set; }

    public string ServiceCategoryName { get; set; } = string.Empty;

    public int GarmentTypeId { get; set; }

    public string GarmentTypeName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public bool ExpressService { get; set; }

    public decimal? ExpressUnitPrice { get; set; }

    public decimal LineTotal { get; set; }

    public string? Notes { get; set; }
}
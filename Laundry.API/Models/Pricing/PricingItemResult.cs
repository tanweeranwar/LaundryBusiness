namespace Laundry.API.Models.Pricing;

public class PricingItemResult
{
    public int ServiceCategoryId { get; set; }

    public int GarmentTypeId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public bool ExpressService { get; set; }

    public decimal? ExpressUnitPrice { get; set; }

    public decimal LineTotal { get; set; }
}
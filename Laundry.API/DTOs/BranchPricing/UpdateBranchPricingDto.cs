namespace Laundry.API.DTOs.BranchPricing;

public class UpdateBranchPricingDto
{
    public int BranchId { get; set; }

    public int ServiceCategoryId { get; set; }

    public int GarmentTypeId { get; set; }

    public decimal Price { get; set; }

    public bool IsExpressAvailable { get; set; }

    public decimal? ExpressPrice { get; set; }

    public int EstimatedProcessingHours { get; set; }
}
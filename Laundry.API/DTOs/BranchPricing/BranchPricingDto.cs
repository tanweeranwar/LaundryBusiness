namespace Laundry.API.DTOs.BranchPricing;

public class BranchPricingDto
{
    public int Id { get; set; }

    public int BranchId { get; set; }

    public string BranchName { get; set; } = string.Empty;

    public int ServiceCategoryId { get; set; }

    public string ServiceCategoryName { get; set; } = string.Empty;

    public int GarmentTypeId { get; set; }

    public string GarmentTypeName { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public bool IsExpressAvailable { get; set; }

    public decimal? ExpressPrice { get; set; }

    public int EstimatedProcessingHours { get; set; }

    // Add this property
    public string DisplayName { get; set; } = string.Empty;
}
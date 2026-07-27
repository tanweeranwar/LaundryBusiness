using System.ComponentModel.DataAnnotations.Schema;

namespace Laundry.API.Entities;

public class BranchPricing : BaseEntity
{
    public int Id { get; set; }

    public int BranchId { get; set; }

    public int ServiceCategoryId { get; set; }

    public int GarmentTypeId { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    public bool IsExpressAvailable { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal? ExpressPrice { get; set; }

    public int EstimatedProcessingHours { get; set; }

    public string? Notes { get; set; }

    // Navigation Properties
    public Branch Branch { get; set; } = null!;

    public ServiceCategory ServiceCategory { get; set; } = null!;

    public GarmentType GarmentType { get; set; } = null!;
}
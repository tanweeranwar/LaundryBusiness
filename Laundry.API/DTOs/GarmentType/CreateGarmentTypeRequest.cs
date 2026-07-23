using System.ComponentModel.DataAnnotations;

namespace Laundry.API.DTOs.GarmentType;

public class CreateGarmentTypeRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    public string? Icon { get; set; }
}
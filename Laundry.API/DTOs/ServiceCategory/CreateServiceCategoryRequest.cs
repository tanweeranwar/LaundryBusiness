using System.ComponentModel.DataAnnotations;

namespace Laundry.API.DTOs.ServiceCategory;

public class CreateServiceCategoryRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; set; }

    [MaxLength(100)]
    public string? Icon { get; set; }
}
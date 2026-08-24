using System.ComponentModel.DataAnnotations;

namespace Laundry.API.DTOs.Processing;

public class UpdateProcessingStepDto
{
    [Required]
    public int Status { get; set; }

    [MaxLength(100)]
    public string? AssignedTo { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace Laundry.API.DTOs.Orders;

public class UpdateOrderDto
{
    [Required]
    public int Status { get; set; }

    [MaxLength(1000)]
    public string? Remarks { get; set; }
}
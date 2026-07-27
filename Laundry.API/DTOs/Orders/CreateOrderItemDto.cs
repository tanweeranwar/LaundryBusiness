using System.ComponentModel.DataAnnotations;

namespace Laundry.API.DTOs.Orders;

public class CreateOrderItemDto
{
    [Required]
    public int ServiceCategoryId { get; set; }

    [Required]
    public int GarmentTypeId { get; set; }

    [Range(1, 1000)]
    public int Quantity { get; set; }

    public bool ExpressService { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace Laundry.API.DTOs.Orders;

public class CreateOrderDto
{
    [Required]
    public int BranchId { get; set; }

    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    public DateTime ExpectedDeliveryDate { get; set; }

    [Range(0, 100000)]
    public decimal DiscountAmount { get; set; }

    [MaxLength(1000)]
    public string? Remarks { get; set; }

    [Required]
    [MinLength(1)]
    public List<CreateOrderItemDto> Items { get; set; }
        = new();
}
using Laundry.API.Enums;

namespace Laundry.API.DTOs.Delivery;

public class DeliveryDto
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public DeliveryStatus Status { get; set; }

    public DateTime? ScheduledDate { get; set; }

    public DateTime? DeliveredOn { get; set; }

    public string? AssignedTo { get; set; }

    public string? Remarks { get; set; }
}
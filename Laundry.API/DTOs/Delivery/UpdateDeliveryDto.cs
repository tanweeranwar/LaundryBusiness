using Laundry.API.Enums;

namespace Laundry.API.DTOs.Delivery;

public class UpdateDeliveryDto
{
    public DeliveryStatus Status { get; set; }

    public DateTime? ScheduledDate { get; set; }

    public DateTime? DeliveredOn { get; set; }

    public string? AssignedTo { get; set; }

    public string? Remarks { get; set; }
}
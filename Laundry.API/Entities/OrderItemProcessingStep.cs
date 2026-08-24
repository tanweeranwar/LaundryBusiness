using Laundry.API.Enums;

namespace Laundry.API.Entities;

public class OrderItemProcessingStep : BaseEntity
{
    public int Id { get; set; }

    public int OrderItemProcessingId { get; set; }

    public int ProcessingWorkflowStepId { get; set; }

    public ProcessingStatus Status { get; set; } = ProcessingStatus.Pending;

    public DateTime? StartedOn { get; set; }

    public DateTime? CompletedOn { get; set; }

    public string? AssignedTo { get; set; }

    public string? Remarks { get; set; }

    // Navigation Properties
    public OrderItemProcessing OrderItemProcessing { get; set; } = null!;

    public ProcessingWorkflowStep ProcessingWorkflowStep { get; set; } = null!;
}
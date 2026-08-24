using Laundry.API.Enums;

namespace Laundry.API.Entities;

public class OrderItemProcessing : BaseEntity
{
    public int Id { get; set; }

    public int OrderItemId { get; set; }

    public int ProcessingWorkflowId { get; set; }

    public ProcessingStatus Status { get; set; } = ProcessingStatus.Pending;

    public DateTime? StartedOn { get; set; }

    public DateTime? CompletedOn { get; set; }

    public string? AssignedTo { get; set; }

    public string? Remarks { get; set; }

    // Navigation Properties
    public OrderItem OrderItem { get; set; } = null!;

    public ProcessingWorkflow ProcessingWorkflow { get; set; } = null!;

    public ICollection<OrderItemProcessingStep> Steps { get; set; }
        = new List<OrderItemProcessingStep>();
}
using Laundry.API.Enums;

namespace Laundry.API.DTOs.Processing;

public class ProcessingDto
{
    public int Id { get; set; }

    public int OrderItemId { get; set; }

    public int OrderId { get; set; }

    public string ServiceCategoryName { get; set; } = string.Empty;

    public string GarmentTypeName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public int ProcessingWorkflowId { get; set; }

    public string WorkflowName { get; set; } = string.Empty;

    public ProcessingStatus Status { get; set; }

    public DateTime? StartedOn { get; set; }

    public DateTime? CompletedOn { get; set; }

    public string? AssignedTo { get; set; }

    public string? Remarks { get; set; }

    public List<ProcessingStepDto> Steps { get; set; } = new();
}
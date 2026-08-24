using Laundry.API.Enums;

namespace Laundry.API.DTOs.Processing;

public class ProcessingStepDto
{
    public int Id { get; set; }

    public int ProcessingWorkflowStepId { get; set; }

    public int Sequence { get; set; }

    public ProcessingStepType StepType { get; set; }

    public ProcessingStatus Status { get; set; }

    public bool IsRequired { get; set; }

    public DateTime? StartedOn { get; set; }

    public DateTime? CompletedOn { get; set; }

    public string? AssignedTo { get; set; }

    public string? Remarks { get; set; }
}
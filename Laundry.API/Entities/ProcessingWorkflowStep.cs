using Laundry.API.Enums;

namespace Laundry.API.Entities;

public class ProcessingWorkflowStep : BaseEntity
{
    public int Id { get; set; }

    public int ProcessingWorkflowId { get; set; }

    public ProcessingStepType StepType { get; set; }

    public int Sequence { get; set; }

    public bool IsRequired { get; set; } = true;

    // Navigation Property
    public ProcessingWorkflow ProcessingWorkflow { get; set; } = null!;
}
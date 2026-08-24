namespace Laundry.API.Entities;

public class ProcessingWorkflow : BaseEntity
{
    public int Id { get; set; }

    public int ServiceCategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public ServiceCategory ServiceCategory { get; set; } = null!;

    public ICollection<ProcessingWorkflowStep> Steps { get; set; }
        = new List<ProcessingWorkflowStep>();
}
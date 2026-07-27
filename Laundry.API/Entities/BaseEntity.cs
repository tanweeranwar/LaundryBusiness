namespace Laundry.API.Entities;

public abstract class BaseEntity
{
    public bool IsActive { get; set; } = true;

    public DateTime CreatedOn { get; set; } = DateTime.SpecifyKind(
        DateTime.Now,
        DateTimeKind.Local);

    public DateTime? UpdatedOn { get; set; }
}
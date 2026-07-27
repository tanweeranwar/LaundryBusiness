namespace Laundry.API.Entities;

public class ServiceCategory : BaseEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public string? Icon { get; set; }
}
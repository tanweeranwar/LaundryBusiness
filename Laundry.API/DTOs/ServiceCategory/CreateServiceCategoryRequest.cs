namespace Laundry.API.DTOs.ServiceCategory;

public class CreateServiceCategoryRequest
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public string? Icon { get; set; }
}
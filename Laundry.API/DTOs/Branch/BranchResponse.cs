namespace Laundry.API.DTOs.Branch;

public class BranchResponse
{
    public int Id { get; set; }

    public string BranchCode { get; set; } = string.Empty;

    public string BranchName { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}
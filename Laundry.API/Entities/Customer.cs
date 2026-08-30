namespace Laundry.API.Entities;

public class Customer : BaseEntity
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string MobileNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = "Customer";

    /// <summary>
    /// Branch assigned to staff accounts. Customer accounts normally remain null;
    /// their branch is derived from their orders.
    /// </summary>
    public int? BranchId { get; set; }

    public Branch? Branch { get; set; }
}
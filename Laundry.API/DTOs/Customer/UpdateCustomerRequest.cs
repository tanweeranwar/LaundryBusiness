using System.ComponentModel.DataAnnotations;

namespace Laundry.API.DTOs.Customer;

public class UpdateCustomerRequest
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [RegularExpression(
        @"^\d{10}$",
        ErrorMessage = "Mobile number must contain exactly 10 digits.")]
    public string MobileNumber { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;
}
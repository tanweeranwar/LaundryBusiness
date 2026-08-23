using System.ComponentModel.DataAnnotations;

namespace Laundry.API.DTOs.Branch;

public class CreateBranchRequest
{
    [Required]
    [MaxLength(20)]
    public string BranchCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string BranchName { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string OwnerName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [RegularExpression(
        @"^\d{10}$",
        ErrorMessage = "Phone number must contain exactly 10 digits.")]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [MaxLength(250)]
    public string AddressLine1 { get; set; } = string.Empty;

    [MaxLength(250)]
    public string AddressLine2 { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string State { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Country { get; set; } = "India";

    [Required]
    [RegularExpression(
        @"^\d{6}$",
        ErrorMessage = "Pincode must contain exactly 6 digits.")]
    public string Pincode { get; set; } = string.Empty;

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public TimeOnly OpeningTime { get; set; }

    public TimeOnly ClosingTime { get; set; }

    [Range(
        0.1,
        100,
        ErrorMessage = "Pickup radius must be greater than zero.")]
    public decimal PickupRadiusKm { get; set; }
}
namespace Laundry.API.DTOs.Authentication;

public class LoginRequest
{
    public string MobileNumber { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
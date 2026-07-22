namespace Laundry.API.Services;

public interface IJwtService
{
    string GenerateToken(Guid customerId, string mobileNumber, string role);
}
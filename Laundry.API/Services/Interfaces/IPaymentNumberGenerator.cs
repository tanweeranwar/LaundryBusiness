namespace Laundry.API.Services.Interfaces;

public interface IPaymentNumberGenerator
{
    Task<string> GenerateAsync();
}
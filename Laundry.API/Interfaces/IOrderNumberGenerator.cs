namespace Laundry.API.Interfaces;

public interface IOrderNumberGenerator
{
    Task<string> GenerateAsync();
}
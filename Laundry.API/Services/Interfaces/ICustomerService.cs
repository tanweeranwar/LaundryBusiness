using Laundry.API.DTOs.Customer;

namespace Laundry.API.Services.Interfaces;

public interface ICustomerService
{
    Task<CustomerResponse> CreateAsync(CreateCustomerRequest request);

    Task<IEnumerable<CustomerResponse>> GetAllAsync();

    Task<CustomerResponse?> GetByIdAsync(Guid id);

    Task<bool> UpdateAsync(
        Guid id,
        UpdateCustomerRequest request);

    Task<bool> DeleteAsync(Guid id);
}
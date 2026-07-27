using Laundry.API.Entities;

namespace Laundry.API.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id);

    Task<bool> ExistsAsync(Guid id);

    Task AddAsync(Customer customer);

    Task UpdateAsync(Customer customer);

    Task DeleteAsync(Customer customer);

    Task SaveChangesAsync();
}
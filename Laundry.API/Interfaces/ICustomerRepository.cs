using Laundry.API.Entities;

namespace Laundry.API.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id);

    Task<IEnumerable<Customer>> GetAllAsync();

    Task<bool> ExistsAsync(Guid id);

    Task AddAsync(Customer customer);

    Task<bool> ExistsByMobileNumberAsync(
    string mobileNumber,
    Guid? excludeCustomerId = null);

    Task<bool> ExistsByEmailAsync(
        string email,
        Guid? excludeCustomerId = null);

    Task UpdateAsync(Customer customer);

    Task DeleteAsync(Customer customer);

    Task SaveChangesAsync();
}
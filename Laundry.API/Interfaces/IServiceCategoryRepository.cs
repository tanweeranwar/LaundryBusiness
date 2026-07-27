using Laundry.API.Entities;

namespace Laundry.API.Repositories;

public interface IServiceCategoryRepository
{
    Task<IEnumerable<ServiceCategory>> GetAllAsync();

    Task<ServiceCategory?> GetByIdAsync(int id);

    Task<ServiceCategory?> GetByNameAsync(string name);

    Task AddAsync(ServiceCategory category);

    Task UpdateAsync(ServiceCategory category);

    Task DeleteAsync(ServiceCategory category);

    Task SaveChangesAsync();
}
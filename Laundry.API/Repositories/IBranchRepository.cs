using Laundry.API.Entities;

namespace Laundry.API.Repositories;

public interface IBranchRepository
{
    Task<IEnumerable<Branch>> GetAllAsync();

    Task<Branch?> GetByIdAsync(int id);

    Task<Branch?> GetByCodeAsync(string branchCode);

    Task AddAsync(Branch branch);

    Task UpdateAsync(Branch branch);

    Task DeleteAsync(Branch branch);

    Task SaveChangesAsync();
}
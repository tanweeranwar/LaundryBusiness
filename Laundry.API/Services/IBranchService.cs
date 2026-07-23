using Laundry.API.DTOs.Branch;

namespace Laundry.API.Services;

public interface IBranchService
{
    Task<IEnumerable<BranchResponse>> GetAllAsync();

    Task<BranchResponse?> GetByIdAsync(int id);

    Task<BranchResponse> CreateAsync(CreateBranchRequest request);

    Task<bool> UpdateAsync(int id, CreateBranchRequest request);

    Task<bool> DeleteAsync(int id);
}
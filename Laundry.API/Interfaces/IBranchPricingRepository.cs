using Laundry.API.Entities;

namespace Laundry.API.Interfaces;

public interface IBranchPricingRepository
{
    Task<IEnumerable<BranchPricing>> GetAllAsync();

    Task<BranchPricing?> GetByIdAsync(int id);

    Task<IEnumerable<BranchPricing>> GetByBranchAsync(int branchId);

    Task<BranchPricing?> GetByCombinationAsync(
        int branchId,
        int serviceCategoryId,
        int garmentTypeId);

    Task<bool> ExistsAsync(
        int branchId,
        int serviceCategoryId,
        int garmentTypeId);

    Task AddAsync(BranchPricing pricing);

    void Update(BranchPricing pricing);

    void Delete(BranchPricing pricing);

    Task SaveChangesAsync();
}
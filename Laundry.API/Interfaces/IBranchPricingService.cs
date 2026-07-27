using Laundry.API.DTOs.BranchPricing;

namespace Laundry.API.Interfaces;

public interface IBranchPricingService
{
    Task<IEnumerable<BranchPricingDto>> GetAllAsync();
    Task<BranchPricingDto?> GetByIdAsync(int id);
    Task<IEnumerable<BranchPricingDto>> GetByBranchAsync(int branchId);
    Task<BranchPricingDto> CreateAsync(CreateBranchPricingDto dto);
    Task<BranchPricingDto> UpdateAsync(int id, UpdateBranchPricingDto dto);
    Task DeleteAsync(int id);
}

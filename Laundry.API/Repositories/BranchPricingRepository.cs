using Laundry.API.Data;
using Laundry.API.Entities;
using Laundry.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Laundry.API.Repositories
{
    public class BranchPricingRepository : IBranchPricingRepository
    {
        private readonly LaundryDbContext _context;

        public BranchPricingRepository(LaundryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BranchPricing>> GetAllAsync()
        {
            return await _context.BranchPricings
                .AsNoTracking()
                .Include(x => x.Branch)
                .Include(x => x.ServiceCategory)
                .Include(x => x.GarmentType)
                .OrderBy(x => x.Branch.BranchName)
                .ThenBy(x => x.ServiceCategory.Name)
                .ThenBy(x => x.GarmentType.Name)
                .ToListAsync();
        }

        public async Task<BranchPricing?> GetByIdAsync(int id)
        {
            return await _context.BranchPricings
                .AsNoTracking()
                .Include(x => x.Branch)
                .Include(x => x.ServiceCategory)
                .Include(x => x.GarmentType)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<BranchPricing>> GetByBranchAsync(int branchId)
        {
            return await _context.BranchPricings
                .AsNoTracking()
                .Where(x => x.BranchId == branchId)
                .Include(x => x.Branch)
                .Include(x => x.ServiceCategory)
                .Include(x => x.GarmentType)
                .OrderBy(x => x.ServiceCategory.Name)
                .ThenBy(x => x.GarmentType.Name)
                .ToListAsync();
        }

        public async Task<BranchPricing?> GetByCombinationAsync(
            int branchId,
            int serviceCategoryId,
            int garmentTypeId)
        {
            return await _context.BranchPricings
                .Include(x => x.Branch)
                .Include(x => x.ServiceCategory)
                .Include(x => x.GarmentType)
                .FirstOrDefaultAsync(x =>
                    x.BranchId == branchId &&
                    x.ServiceCategoryId == serviceCategoryId &&
                    x.GarmentTypeId == garmentTypeId);
        }

        public async Task<bool> ExistsAsync(
            int branchId,
            int serviceCategoryId,
            int garmentTypeId)
        {
            return await _context.BranchPricings.AnyAsync(x =>
                x.BranchId == branchId &&
                x.ServiceCategoryId == serviceCategoryId &&
                x.GarmentTypeId == garmentTypeId);
        }

        public async Task AddAsync(BranchPricing pricing)
        {
            await _context.BranchPricings.AddAsync(pricing);
        }

        public void Update(BranchPricing pricing)
        {
            _context.BranchPricings.Update(pricing);
        }

        public void Delete(BranchPricing pricing)
        {
            _context.BranchPricings.Remove(pricing);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
using Laundry.API.Data;
using Laundry.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Laundry.API.Repositories;

public class BranchRepository : IBranchRepository
{
    private readonly LaundryDbContext _context;

    public BranchRepository(LaundryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Branch>> GetAllAsync()
    {
        return await _context.Branches
            .OrderBy(x => x.BranchName)
            .ToListAsync();
    }

    public async Task<Branch?> GetByIdAsync(int id)
    {
        return await _context.Branches
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Branch?> GetByCodeAsync(string branchCode)
    {
        return await _context.Branches
            .FirstOrDefaultAsync(x => x.BranchCode == branchCode);
    }

    public async Task AddAsync(Branch branch)
    {
        await _context.Branches.AddAsync(branch);
    }

    public Task UpdateAsync(Branch branch)
    {
        _context.Branches.Update(branch);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Branch branch)
    {
        _context.Branches.Remove(branch);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
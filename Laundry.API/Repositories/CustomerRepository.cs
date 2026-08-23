using Laundry.API.Data;
using Laundry.API.Entities;
using Laundry.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Laundry.API.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly LaundryDbContext _context;

    public CustomerRepository(LaundryDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(Guid id)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await _context.Customers
            .AsNoTracking()
            .OrderBy(x => x.FirstName)
            .ThenBy(x => x.LastName)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Customers
            .AnyAsync(x => x.Id == id);
    }

    public async Task AddAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
    }

    public async Task<bool> ExistsByMobileNumberAsync(
    string mobileNumber,
    Guid? excludeCustomerId = null)
    {
        return await _context.Customers
            .AnyAsync(x =>
                x.MobileNumber == mobileNumber &&
                (!excludeCustomerId.HasValue ||
                 x.Id != excludeCustomerId.Value));
    }

    public async Task<bool> ExistsByEmailAsync(
        string email,
        Guid? excludeCustomerId = null)
    {
        return await _context.Customers
            .AnyAsync(x =>
                x.Email == email &&
                (!excludeCustomerId.HasValue ||
                 x.Id != excludeCustomerId.Value));
    }

    public Task UpdateAsync(Customer customer)
    {
        _context.Customers.Update(customer);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Customer customer)
    {
        _context.Customers.Remove(customer);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
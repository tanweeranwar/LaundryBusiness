using Laundry.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Laundry.API.Data;

public class LaundryDbContext : DbContext
{
    public LaundryDbContext(DbContextOptions<LaundryDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
}
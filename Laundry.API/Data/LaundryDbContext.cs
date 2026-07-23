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
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<ServiceCategory> ServiceCategories => Set<ServiceCategory>();
    public DbSet<GarmentType> GarmentTypes => Set<GarmentType>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Branch>()
            .HasIndex(b => b.BranchCode)
            .IsUnique();

        modelBuilder.Entity<ServiceCategory>()
            .HasIndex(x => x.Name)
            .IsUnique();

        modelBuilder.Entity<GarmentType>()
            .HasIndex(x => x.Name)
            .IsUnique();
    }
}

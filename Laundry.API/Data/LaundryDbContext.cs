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
    public DbSet<BranchPricing> BranchPricings => Set<BranchPricing>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) ||
                    property.ClrType == typeof(DateTime?))
                {
                    property.SetColumnType("timestamp without time zone");
                }
            }
        }

        ConfigureBranch(modelBuilder);
        ConfigureServiceCategory(modelBuilder);
        ConfigureGarmentType(modelBuilder);
        ConfigureBranchPricing(modelBuilder);
        ConfigureOrder(modelBuilder);
        ConfigureOrderItem(modelBuilder);
    }

    #region Configuration Methods

    private static void ConfigureBranch(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Branch>()
            .HasIndex(x => x.BranchCode)
            .IsUnique();
    }

    private static void ConfigureServiceCategory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ServiceCategory>()
            .HasIndex(x => x.Name)
            .IsUnique();
    }

    private static void ConfigureGarmentType(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GarmentType>()
            .HasIndex(x => x.Name)
            .IsUnique();
    }

    private static void ConfigureBranchPricing(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BranchPricing>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Price)
                  .HasPrecision(18, 2);

            entity.Property(x => x.ExpressPrice)
                  .HasPrecision(18, 2);

            entity.HasOne(x => x.Branch)
                  .WithMany()
                  .HasForeignKey(x => x.BranchId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.ServiceCategory)
                  .WithMany()
                  .HasForeignKey(x => x.ServiceCategoryId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.GarmentType)
                  .WithMany()
                  .HasForeignKey(x => x.GarmentTypeId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => new
            {
                x.BranchId,
                x.ServiceCategoryId,
                x.GarmentTypeId
            }).IsUnique();
        });
    }

    private static void ConfigureOrder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.OrderNumber)
                  .IsRequired()
                  .HasMaxLength(30);

            entity.Property(x => x.Subtotal)
                  .HasPrecision(18, 2);

            entity.Property(x => x.DiscountAmount)
                  .HasPrecision(18, 2);

            entity.Property(x => x.TaxAmount)
                  .HasPrecision(18, 2);

            entity.Property(x => x.GrandTotal)
                  .HasPrecision(18, 2);

            entity.HasIndex(x => x.OrderNumber)
                  .IsUnique();

            entity.HasIndex(x => x.CustomerId);

            entity.HasIndex(x => x.BranchId);

            entity.HasIndex(x => x.OrderDate);

            entity.HasIndex(x => x.Status);

            entity.HasOne(x => x.Branch)
                  .WithMany()
                  .HasForeignKey(x => x.BranchId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Customer)
                  .WithMany()
                  .HasForeignKey(x => x.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureOrderItem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.UnitPrice)
                  .HasPrecision(18, 2);

            entity.Property(x => x.ExpressUnitPrice)
                  .HasPrecision(18, 2);

            entity.Property(x => x.LineTotal)
                  .HasPrecision(18, 2);

            entity.HasIndex(x => new
            {
                x.OrderId,
                x.GarmentTypeId
            });

            entity.HasOne(x => x.Order)
                  .WithMany(x => x.Items)
                  .HasForeignKey(x => x.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.ServiceCategory)
                  .WithMany()
                  .HasForeignKey(x => x.ServiceCategoryId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.GarmentType)
                  .WithMany()
                  .HasForeignKey(x => x.GarmentTypeId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    #endregion
}
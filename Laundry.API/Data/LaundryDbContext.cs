using Laundry.API.Configurations;
using Laundry.API.Entities;
using Laundry.API.Enums;
using Microsoft.EntityFrameworkCore;

namespace Laundry.API.Data;

public class LaundryDbContext : DbContext
{
    public LaundryDbContext(DbContextOptions<LaundryDbContext> options)
        : base(options)
    {
    }

    private static readonly DateTime ProcessingSeedCreatedOn =
    new(2026, 8, 24, 0, 0, 0, DateTimeKind.Unspecified);

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<ServiceCategory> ServiceCategories => Set<ServiceCategory>();
    public DbSet<GarmentType> GarmentTypes => Set<GarmentType>();
    public DbSet<BranchPricing> BranchPricings => Set<BranchPricing>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderStatusHistory> OrderStatusHistories => Set<OrderStatusHistory>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Pickup> Pickups => Set<Pickup>();
    public DbSet<Delivery> Deliveries => Set<Delivery>();

    public DbSet<ProcessingWorkflow> ProcessingWorkflows
    => Set<ProcessingWorkflow>();

    public DbSet<ProcessingWorkflowStep> ProcessingWorkflowSteps
        => Set<ProcessingWorkflowStep>();

    public DbSet<OrderItemProcessing> OrderItemProcessings
        => Set<OrderItemProcessing>();

    public DbSet<OrderItemProcessingStep> OrderItemProcessingSteps
        => Set<OrderItemProcessingStep>();

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
        ConfigureOrderStatusHistory(modelBuilder);
        ConfigureOrderItem(modelBuilder);
        ConfigurePickup(modelBuilder);
        ConfigureDelivery(modelBuilder);
        ConfigureProcessingWorkflow(modelBuilder);
        ConfigureProcessingWorkflowStep(modelBuilder);
        ConfigureOrderItemProcessing(modelBuilder);
        ConfigureOrderItemProcessingStep(modelBuilder);
        SeedProcessingWorkflows(modelBuilder);

        modelBuilder.ApplyConfiguration(new PaymentConfiguration());
    }

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

            entity.Property(x => x.BalanceAmount)
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

    private static void ConfigureOrderStatusHistory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderStatusHistory>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Remarks)
                .HasMaxLength(500);

            entity.Property(x => x.ChangedBy)
                .HasMaxLength(100);

            entity.HasOne(x => x.Order)
                .WithMany()
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.OrderId);
            entity.HasIndex(x => x.ChangedOn);
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

    private static void ConfigurePickup(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pickup>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.AssignedTo)
                .HasMaxLength(100);

            entity.Property(x => x.Remarks)
                .HasMaxLength(500);

            entity.HasOne(x => x.Order)
                .WithMany()
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.OrderId)
                .IsUnique();

            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.ScheduledDate);
        });
    }

    private static void ConfigureDelivery(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Delivery>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.AssignedTo)
                .HasMaxLength(100);

            entity.Property(x => x.Remarks)
                .HasMaxLength(500);

            entity.HasOne(x => x.Order)
                .WithMany()
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.OrderId)
                .IsUnique();

            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.ScheduledDate);
        });
    }

    private static void ConfigureProcessingWorkflow(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProcessingWorkflow>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            entity.HasOne(x => x.ServiceCategory)
                .WithMany()
                .HasForeignKey(x => x.ServiceCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.ServiceCategoryId)
                .IsUnique();
        });
    }

    private static void ConfigureProcessingWorkflowStep(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProcessingWorkflowStep>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.ProcessingWorkflow)
                .WithMany(x => x.Steps)
                .HasForeignKey(x => x.ProcessingWorkflowId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => new
            {
                x.ProcessingWorkflowId,
                x.Sequence
            })
            .IsUnique();
        });
    }

    private static void ConfigureOrderItemProcessing(
    ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderItemProcessing>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.AssignedTo)
                .HasMaxLength(100);

            entity.Property(x => x.Remarks)
                .HasMaxLength(500);

            entity.HasOne(x => x.OrderItem)
                .WithOne(x => x.OrderItemProcessing)
                .HasForeignKey<OrderItemProcessing>(
                    x => x.OrderItemId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.ProcessingWorkflow)
                .WithMany()
                .HasForeignKey(x => x.ProcessingWorkflowId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.OrderItemId)
                .IsUnique();

            entity.HasIndex(x => x.Status);
        });
    }

    private static void ConfigureOrderItemProcessingStep(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderItemProcessingStep>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.AssignedTo)
                .HasMaxLength(100);

            entity.Property(x => x.Remarks)
                .HasMaxLength(500);

            entity.HasOne(x => x.OrderItemProcessing)
                .WithMany(x => x.Steps)
                .HasForeignKey(x => x.OrderItemProcessingId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.ProcessingWorkflowStep)
                .WithMany()
                .HasForeignKey(x => x.ProcessingWorkflowStepId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => new
            {
                x.OrderItemProcessingId,
                x.ProcessingWorkflowStepId
            })
            .IsUnique();

            entity.HasIndex(x => x.Status);
        });
    }

    private static void SeedProcessingWorkflows(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProcessingWorkflow>().HasData(
            new ProcessingWorkflow
            {
                Id = 1,
                ServiceCategoryId = 2,
                Name = "Wash",
                IsActive = true,
                CreatedOn = ProcessingSeedCreatedOn
            },
            new ProcessingWorkflow
            {
                Id = 2,
                ServiceCategoryId = 3,
                Name = "Dry Clean",
                IsActive = true,
                CreatedOn = ProcessingSeedCreatedOn
            },
            new ProcessingWorkflow
            {
                Id = 3,
                ServiceCategoryId = 4,
                Name = "Iron",
                IsActive = true,
                CreatedOn = ProcessingSeedCreatedOn
            }
        );

        modelBuilder.Entity<ProcessingWorkflowStep>().HasData(
            // Wash
            new ProcessingWorkflowStep
            {
                Id = 1,
                ProcessingWorkflowId = 1,
                StepType = ProcessingStepType.Washing,
                Sequence = 1,
                IsRequired = true,
                CreatedOn = ProcessingSeedCreatedOn
            },
            new ProcessingWorkflowStep
            {
                Id = 2,
                ProcessingWorkflowId = 1,
                StepType = ProcessingStepType.QualityCheck,
                Sequence = 2,
                IsRequired = true,
                CreatedOn = ProcessingSeedCreatedOn
            },

            // Dry Clean
            new ProcessingWorkflowStep
            {
                Id = 3,
                ProcessingWorkflowId = 2,
                StepType = ProcessingStepType.DryCleaning,
                Sequence = 1,
                IsRequired = true,
                CreatedOn = ProcessingSeedCreatedOn
            },
            new ProcessingWorkflowStep
            {
                Id = 4,
                ProcessingWorkflowId = 2,
                StepType = ProcessingStepType.QualityCheck,
                Sequence = 2,
                IsRequired = true,
                CreatedOn = ProcessingSeedCreatedOn
            },

            // Iron
            new ProcessingWorkflowStep
            {
                Id = 5,
                ProcessingWorkflowId = 3,
                StepType = ProcessingStepType.Ironing,
                Sequence = 1,
                IsRequired = true,
                CreatedOn = ProcessingSeedCreatedOn
            },
            new ProcessingWorkflowStep
            {
                Id = 6,
                ProcessingWorkflowId = 3,
                StepType = ProcessingStepType.QualityCheck,
                Sequence = 2,
                IsRequired = true,
                CreatedOn = ProcessingSeedCreatedOn
            }
        );
    }
}
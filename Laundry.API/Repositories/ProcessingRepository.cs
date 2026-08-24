using Laundry.API.Data;
using Laundry.API.Entities;
using Laundry.API.Enums;
using Laundry.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Laundry.API.Repositories;

public class ProcessingRepository : IProcessingRepository
{
    private readonly LaundryDbContext _context;

    public ProcessingRepository(LaundryDbContext context)
    {
        _context = context;
    }

    public async Task<OrderItem?> GetOrderItemWithProcessingAsync(
        int orderItemId)
    {
        return await _context.OrderItems
            .Include(x => x.Order)
            .Include(x => x.ServiceCategory)
            .Include(x => x.GarmentType)
            .Include(x => x.OrderItemProcessing)
                .ThenInclude(x => x!.Steps)
                    .ThenInclude(x => x.ProcessingWorkflowStep)
            .FirstOrDefaultAsync(x => x.Id == orderItemId);
    }

    public async Task<Order?> GetOrderWithItemsAsync(int orderId)
    {
        return await _context.Orders
            .Include(x => x.Items)
                .ThenInclude(x => x.ServiceCategory)
            .Include(x => x.Items)
                .ThenInclude(x => x.GarmentType)
            .Include(x => x.Items)
                .ThenInclude(x => x.OrderItemProcessing)
                    .ThenInclude(x => x!.Steps)
                        .ThenInclude(x => x.ProcessingWorkflowStep)
            .FirstOrDefaultAsync(x => x.Id == orderId);
    }

    public async Task<ProcessingWorkflow?> GetWorkflowByServiceCategoryAsync(
        int serviceCategoryId)
    {
        return await _context.ProcessingWorkflows
            .Include(x => x.Steps)
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.ServiceCategoryId == serviceCategoryId &&
                     x.IsActive);
    }

    public async Task<OrderItemProcessing?> GetByIdAsync(int id)
    {
        return await _context.OrderItemProcessings
            .Include(x => x.OrderItem)
                .ThenInclude(x => x.ServiceCategory)
            .Include(x => x.OrderItem)
                .ThenInclude(x => x.GarmentType)
            .Include(x => x.ProcessingWorkflow)
            .Include(x => x.Steps)
                .ThenInclude(x => x.ProcessingWorkflowStep)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<OrderItemProcessing?> GetByOrderItemIdAsync(
        int orderItemId)
    {
        return await _context.OrderItemProcessings
            .Include(x => x.OrderItem)
                .ThenInclude(x => x.ServiceCategory)
            .Include(x => x.OrderItem)
                .ThenInclude(x => x.GarmentType)
            .Include(x => x.ProcessingWorkflow)
            .Include(x => x.Steps)
                .ThenInclude(x => x.ProcessingWorkflowStep)
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.OrderItemId == orderItemId);
    }

    public async Task<List<OrderItemProcessing>> GetByOrderIdAsync(
        int orderId)
    {
        return await _context.OrderItemProcessings
            .Include(x => x.OrderItem)
                .ThenInclude(x => x.ServiceCategory)
            .Include(x => x.OrderItem)
                .ThenInclude(x => x.GarmentType)
            .Include(x => x.ProcessingWorkflow)
            .Include(x => x.Steps)
                .ThenInclude(x => x.ProcessingWorkflowStep)
            .Where(x => x.OrderItem.OrderId == orderId)
            .OrderBy(x => x.OrderItemId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<OrderItemProcessing> AddAsync(
        OrderItemProcessing processing)
    {
        await _context.OrderItemProcessings.AddAsync(processing);
        return processing;
    }

    public async Task UpdateAsync(
        OrderItemProcessing processing)
    {
        _context.Entry(processing).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    public async Task<OrderItemProcessing?> GetForUpdateAsync(
    int processingId)
    {
        return await _context.OrderItemProcessings
            .Include(x => x.OrderItem)
            .Include(x => x.ProcessingWorkflow)
            .Include(x => x.Steps)
                .ThenInclude(x => x.ProcessingWorkflowStep)
            .FirstOrDefaultAsync(x => x.Id == processingId);
    }

    public async Task<bool> AreAllOrderItemsProcessingCompletedAsync(
    int orderId)
    {
        var processing = await _context.OrderItemProcessings
            .Include(x => x.OrderItem)
            .Where(x => x.OrderItem.OrderId == orderId)
            .ToListAsync();

        if (!processing.Any())
            return false;

        return processing.All(x =>
            x.Status == ProcessingStatus.Completed);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
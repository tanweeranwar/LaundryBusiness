using System.Security.Claims;
using Laundry.API.Data;
using Laundry.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Laundry.API.Services;

public class BranchAuthorizationService : IBranchAuthorizationService
{
    private readonly LaundryDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BranchAuthorizationService(
        LaundryDbContext context,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal User =>
        _httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal();

    public bool IsSuperAdmin => User.IsInRole("Super Admin");

    public bool IsCustomer => User.IsInRole("Customer");

    public bool IsBranchScopedStaff =>
        User.IsInRole("Branch Admin") ||
        User.IsInRole("Employee") ||
        User.IsInRole("Delivery Agent");

    public int? CurrentBranchId
    {
        get
        {
            var value = User.FindFirstValue("branch_id");
            return int.TryParse(value, out var branchId)
                ? branchId
                : null;
        }
    }

    public bool CanAccessBranch(int branchId)
    {
        if (IsSuperAdmin)
            return true;

        return IsBranchScopedStaff && CurrentBranchId == branchId;
    }

    public async Task<bool> CanAccessCustomerAsync(Guid customerId)
    {
        var customer = await _context.Customers
            .AsNoTracking()
            .Select(x => new { x.Id, x.BranchId })
            .FirstOrDefaultAsync(x => x.Id == customerId);

        if (customer == null)
            return false;

        if (IsSuperAdmin)
            return true;

        if (IsCustomer)
        {
            var userId = GetCurrentUserId();
            return userId.HasValue && userId.Value == customerId;
        }

        if (!IsBranchScopedStaff || !CurrentBranchId.HasValue)
            return false;

        if (customer.BranchId == CurrentBranchId.Value)
            return true;

        return await _context.Orders
            .AsNoTracking()
            .AnyAsync(x =>
                x.CustomerId == customerId &&
                x.BranchId == CurrentBranchId.Value);
    }

    public async Task<bool> CanAccessOrderAsync(int orderId)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .Select(x => new { x.Id, x.CustomerId, x.BranchId })
            .FirstOrDefaultAsync(x => x.Id == orderId);

        if (order == null)
            return false;

        if (IsSuperAdmin)
            return true;

        if (IsCustomer)
        {
            var userId = GetCurrentUserId();
            return userId.HasValue && userId.Value == order.CustomerId;
        }

        return IsBranchScopedStaff &&
               CurrentBranchId.HasValue &&
               order.BranchId == CurrentBranchId.Value;
    }

    public async Task<bool> CanAccessPaymentAsync(int paymentId)
    {
        var orderId = await _context.Payments
            .AsNoTracking()
            .Where(x => x.Id == paymentId)
            .Select(x => (int?)x.OrderId)
            .FirstOrDefaultAsync();

        return orderId.HasValue && await CanAccessOrderAsync(orderId.Value);
    }

    public async Task<bool> CanAccessPickupAsync(int pickupId)
    {
        var orderId = await _context.Pickups
            .AsNoTracking()
            .Where(x => x.Id == pickupId)
            .Select(x => (int?)x.OrderId)
            .FirstOrDefaultAsync();

        return orderId.HasValue && await CanAccessOrderAsync(orderId.Value);
    }

    public async Task<bool> CanAccessDeliveryAsync(int deliveryId)
    {
        var orderId = await _context.Deliveries
            .AsNoTracking()
            .Where(x => x.Id == deliveryId)
            .Select(x => (int?)x.OrderId)
            .FirstOrDefaultAsync();

        return orderId.HasValue && await CanAccessOrderAsync(orderId.Value);
    }

    public async Task<bool> CanAccessProcessingAsync(int processingId)
    {
        var orderId = await _context.OrderItemProcessings
            .AsNoTracking()
            .Where(x => x.Id == processingId)
            .Select(x => (int?)x.OrderItem.OrderId)
            .FirstOrDefaultAsync();

        return orderId.HasValue && await CanAccessOrderAsync(orderId.Value);
    }

    public async Task<bool> CanAccessOrderItemAsync(int orderItemId)
    {
        var orderId = await _context.OrderItems
            .AsNoTracking()
            .Where(x => x.Id == orderItemId)
            .Select(x => (int?)x.OrderId)
            .FirstOrDefaultAsync();

        return orderId.HasValue && await CanAccessOrderAsync(orderId.Value);
    }

    private Guid? GetCurrentUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("sub");

        return Guid.TryParse(claim, out var userId)
            ? userId
            : null;
    }
}
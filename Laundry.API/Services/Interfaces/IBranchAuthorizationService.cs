namespace Laundry.API.Services.Interfaces;

public interface IBranchAuthorizationService
{
    bool IsSuperAdmin { get; }
    bool IsCustomer { get; }
    bool IsBranchScopedStaff { get; }
    int? CurrentBranchId { get; }

    bool CanAccessBranch(int branchId);

    Task<bool> CanAccessCustomerAsync(Guid customerId);
    Task<bool> CanAccessOrderAsync(int orderId);
    Task<bool> CanAccessPaymentAsync(int paymentId);
    Task<bool> CanAccessPickupAsync(int pickupId);
    Task<bool> CanAccessDeliveryAsync(int deliveryId);
    Task<bool> CanAccessProcessingAsync(int processingId);
    Task<bool> CanAccessOrderItemAsync(int orderItemId);
}
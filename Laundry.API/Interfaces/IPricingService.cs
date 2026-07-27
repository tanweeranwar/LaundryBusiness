using Laundry.API.DTOs.Orders;
using Laundry.API.Models.Pricing;

namespace Laundry.API.Interfaces;

public interface IPricingService
{
    Task<PricingResult> CalculateAsync(
        int branchId,
        List<CreateOrderItemDto> items,
        decimal discountAmount);
}
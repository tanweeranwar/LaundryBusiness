using Laundry.API.DTOs.Orders;
using Laundry.API.Interfaces;
using Laundry.API.Models.Pricing;

namespace Laundry.API.Services;

public class PricingService : IPricingService
{
    private readonly IBranchPricingRepository _pricingRepository;

    public PricingService(IBranchPricingRepository pricingRepository)
    {
        _pricingRepository = pricingRepository;
    }

    public async Task<PricingResult> CalculateAsync(
        int branchId,
        List<CreateOrderItemDto> items,
        decimal discountAmount)
    {
        var result = new PricingResult();

        foreach (var item in items)
        {
            var pricing = await _pricingRepository.GetPricingAsync(
                branchId,
                item.ServiceCategoryId,
                item.GarmentTypeId);

            if (pricing == null)
            {
                throw new Exception(
                    $"Pricing not configured for ServiceCategoryId={item.ServiceCategoryId}, GarmentTypeId={item.GarmentTypeId}");
            }

            decimal unitPrice;

            if (item.ExpressService)
            {
                if (!pricing.IsExpressAvailable)
                {
                    throw new Exception(
                        "Express service is not available for this garment.");
                }

                unitPrice = pricing.ExpressPrice ?? pricing.Price;
            }
            else
            {
                unitPrice = pricing.Price;
            }

            var lineTotal = unitPrice * item.Quantity;

            result.Items.Add(new PricingItemResult
            {
                ServiceCategoryId = item.ServiceCategoryId,
                GarmentTypeId = item.GarmentTypeId,
                Quantity = item.Quantity,
                UnitPrice = pricing.Price,
                ExpressService = item.ExpressService,
                ExpressUnitPrice = pricing.ExpressPrice,
                LineTotal = lineTotal
            });

            result.Subtotal += lineTotal;
        }

        result.Discount = discountAmount;

        result.Tax = Math.Round(
            (result.Subtotal - result.Discount) * 0.18m,
            2);

        result.GrandTotal =
            result.Subtotal -
            result.Discount +
            result.Tax;

        return result;
    }
}
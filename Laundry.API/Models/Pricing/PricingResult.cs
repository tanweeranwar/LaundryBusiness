namespace Laundry.API.Models.Pricing;

public class PricingResult
{
    public decimal Subtotal { get; set; }

    public decimal Discount { get; set; }

    public decimal Tax { get; set; }

    public decimal GrandTotal { get; set; }

    public List<PricingItemResult> Items { get; set; } = new();
}
namespace Laundry.API.Exceptions;

public class DuplicateBranchPricingException : Exception
{
    public DuplicateBranchPricingException()
        : base("Pricing already exists for the selected Branch, Service Category and Garment Type.")
    {
    }
}

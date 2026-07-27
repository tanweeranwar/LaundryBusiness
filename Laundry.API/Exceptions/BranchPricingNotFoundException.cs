namespace Laundry.API.Exceptions;

public class BranchPricingNotFoundException : Exception
{
    public BranchPricingNotFoundException(int id)
        : base($"Branch pricing with Id {id} was not found.")
    {
    }
}

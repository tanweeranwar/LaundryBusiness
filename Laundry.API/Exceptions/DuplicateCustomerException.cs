namespace Laundry.API.Exceptions;

public class DuplicateCustomerException : Exception
{
    public DuplicateCustomerException(string message)
        : base(message)
    {
    }
}
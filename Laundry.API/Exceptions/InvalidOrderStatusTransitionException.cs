namespace Laundry.API.Exceptions;

public class InvalidOrderStatusTransitionException : Exception
{
    public InvalidOrderStatusTransitionException(string message)
        : base(message)
    {
    }
}
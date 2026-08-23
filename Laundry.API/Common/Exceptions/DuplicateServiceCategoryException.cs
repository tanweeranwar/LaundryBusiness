namespace Laundry.API.Common.Exceptions;

public class DuplicateServiceCategoryException : Exception
{
    public DuplicateServiceCategoryException(string message)
        : base(message)
    {
    }
}
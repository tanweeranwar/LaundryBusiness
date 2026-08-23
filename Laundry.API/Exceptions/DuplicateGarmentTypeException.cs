namespace Laundry.API.Exceptions;

public class DuplicateGarmentTypeException : Exception
{
    public DuplicateGarmentTypeException(string name)
        : base($"Garment type '{name}' already exists.")
    {
    }
}
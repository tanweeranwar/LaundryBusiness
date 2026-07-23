namespace Laundry.API.Common.Exceptions;

public class DuplicateBranchCodeException : Exception
{
    public DuplicateBranchCodeException(string branchCode)
        : base($"Branch code '{branchCode}' already exists.")
    {
    }
}
using Laundry.API.Common.Exceptions;
using Laundry.API.DTOs.Branch;
using Laundry.API.Entities;
using Laundry.API.Repositories;

namespace Laundry.API.Services;

public class BranchService : IBranchService
{
    private readonly IBranchRepository _repository;

    public BranchService(IBranchRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<BranchResponse>> GetAllAsync()
    {
        var branches = await _repository.GetAllAsync();

        return branches.Select(x => new BranchResponse
        {
            Id = x.Id,
            BranchCode = x.BranchCode,
            BranchName = x.BranchName,
            City = x.City,
            State = x.State,
            IsActive = x.IsActive
        });
    }

    public async Task<BranchResponse?> GetByIdAsync(int id)
    {
        var branch = await _repository.GetByIdAsync(id);

        if (branch == null)
            return null;

        return new BranchResponse
        {
            Id = branch.Id,
            BranchCode = branch.BranchCode,
            BranchName = branch.BranchName,
            City = branch.City,
            State = branch.State,
            IsActive = branch.IsActive
        };
    }

    public async Task<BranchResponse> CreateAsync(CreateBranchRequest request)
    {
        var existing = await _repository.GetByCodeAsync(request.BranchCode);

        if (existing != null)
            throw new DuplicateBranchCodeException(request.BranchCode);

        var branch = new Branch
        {
            BranchCode = request.BranchCode,
            BranchName = request.BranchName,
            OwnerName = request.OwnerName,
            Email = request.Email,
            Phone = request.Phone,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            State = request.State,
            Country = request.Country,
            Pincode = request.Pincode,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            OpeningTime = request.OpeningTime,
            ClosingTime = request.ClosingTime,
            PickupRadiusKm = request.PickupRadiusKm
        };

        await _repository.AddAsync(branch);
        await _repository.SaveChangesAsync();

        return new BranchResponse
        {
            Id = branch.Id,
            BranchCode = branch.BranchCode,
            BranchName = branch.BranchName,
            City = branch.City,
            State = branch.State,
            IsActive = branch.IsActive
        };
    }

    public async Task<bool> UpdateAsync(int id, CreateBranchRequest request)
    {
        var branch = await _repository.GetByIdAsync(id);

        if (branch == null)
            return false;

        branch.BranchName = request.BranchName;
        branch.OwnerName = request.OwnerName;
        branch.Email = request.Email;
        branch.Phone = request.Phone;
        branch.AddressLine1 = request.AddressLine1;
        branch.AddressLine2 = request.AddressLine2;
        branch.City = request.City;
        branch.State = request.State;
        branch.Country = request.Country;
        branch.Pincode = request.Pincode;
        branch.Latitude = request.Latitude;
        branch.Longitude = request.Longitude;
        branch.OpeningTime = request.OpeningTime;
        branch.ClosingTime = request.ClosingTime;
        branch.PickupRadiusKm = request.PickupRadiusKm;
        branch.UpdatedOn = DateTime.UtcNow;

        await _repository.UpdateAsync(branch);
        await _repository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var branch = await _repository.GetByIdAsync(id);

        if (branch == null)
            return false;

        await _repository.DeleteAsync(branch);
        await _repository.SaveChangesAsync();

        return true;
    }
}
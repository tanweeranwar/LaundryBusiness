using Laundry.API.DTOs.GarmentType;
using Laundry.API.Entities;
using Laundry.API.Exceptions;
using Laundry.API.Interfaces;
using Laundry.API.Services.Interfaces;

namespace Laundry.API.Services;

public class GarmentTypeService : IGarmentTypeService
{
    private readonly IGarmentTypeRepository _repository;

    public GarmentTypeService(
        IGarmentTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<GarmentTypeResponse>> GetAllAsync()
    {
        var garmentTypes = await _repository.GetAllAsync();

        return garmentTypes.Select(Map);
    }

    public async Task<GarmentTypeResponse?> GetByIdAsync(int id)
    {
        var garmentType = await _repository.GetByIdAsync(id);

        return garmentType == null
            ? null
            : Map(garmentType);
    }

    public async Task<GarmentTypeResponse> CreateAsync(
        CreateGarmentTypeRequest request)
    {
        var name = request.Name.Trim();

        var existing = await _repository.GetByNameAsync(name);

        if (existing != null)
        {
            throw new DuplicateGarmentTypeException(name);
        }

        var garmentType = new GarmentType
        {
            Name = name,
            Description = request.Description?.Trim() ?? string.Empty,
            Icon = string.IsNullOrWhiteSpace(request.Icon)
                ? null
                : request.Icon.Trim()
        };

        await _repository.AddAsync(garmentType);
        await _repository.SaveChangesAsync();

        return Map(garmentType);
    }

    public async Task<bool> UpdateAsync(
        int id,
        CreateGarmentTypeRequest request)
    {
        var garmentType = await _repository.GetByIdAsync(id);

        if (garmentType == null)
        {
            return false;
        }

        var name = request.Name.Trim();

        var existing = await _repository.GetByNameAsync(name);

        if (existing != null && existing.Id != id)
        {
            throw new DuplicateGarmentTypeException(name);
        }

        garmentType.Name = name;

        garmentType.Description =
            request.Description?.Trim() ?? string.Empty;

        garmentType.Icon =
            string.IsNullOrWhiteSpace(request.Icon)
                ? null
                : request.Icon.Trim();

        garmentType.UpdatedOn = DateTime.Now;

        await _repository.UpdateAsync(garmentType);
        await _repository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var garmentType = await _repository.GetByIdAsync(id);

        if (garmentType == null)
        {
            return false;
        }

        await _repository.DeleteAsync(garmentType);
        await _repository.SaveChangesAsync();

        return true;
    }

    private static GarmentTypeResponse Map(
        GarmentType garmentType)
    {
        return new GarmentTypeResponse
        {
            Id = garmentType.Id,
            Name = garmentType.Name,
            Description = garmentType.Description,
            Icon = garmentType.Icon,
            IsActive = garmentType.IsActive
        };
    }
}
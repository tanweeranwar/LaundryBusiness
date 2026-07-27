using Laundry.API.DTOs.GarmentType;
using Laundry.API.Entities;
using Laundry.API.Interfaces;

namespace Laundry.API.Services;

public class GarmentTypeService : IGarmentTypeService
{
    private readonly IGarmentTypeRepository _repository;

    public GarmentTypeService(IGarmentTypeRepository repository)
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

        return garmentType == null ? null : Map(garmentType);
    }

    public async Task<GarmentTypeResponse> CreateAsync(CreateGarmentTypeRequest request)
    {
        var existing = await _repository.GetByNameAsync(request.Name);

        if (existing != null)
            throw new InvalidOperationException("Garment Type already exists.");

        var garmentType = new GarmentType
        {
            Name = request.Name,
            Description = request.Description,
            Icon = request.Icon
        };

        await _repository.AddAsync(garmentType);
        await _repository.SaveChangesAsync();

        return Map(garmentType);
    }

    public async Task<bool> UpdateAsync(int id, CreateGarmentTypeRequest request)
    {
        var garmentType = await _repository.GetByIdAsync(id);

        if (garmentType == null)
            return false;

        garmentType.Name = request.Name;
        garmentType.Description = request.Description;
        garmentType.Icon = request.Icon;

        await _repository.UpdateAsync(garmentType);
        await _repository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var garmentType = await _repository.GetByIdAsync(id);

        if (garmentType == null)
            return false;

        await _repository.DeleteAsync(garmentType);
        await _repository.SaveChangesAsync();

        return true;
    }

    private static GarmentTypeResponse Map(GarmentType garmentType)
    {
        return new GarmentTypeResponse
        {
            Id = garmentType.Id,
            Name = garmentType.Name,
            Description = garmentType.Description,
            IsActive = garmentType.IsActive
        };
    }
}
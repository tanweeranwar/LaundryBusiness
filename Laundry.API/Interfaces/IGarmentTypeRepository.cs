using Laundry.API.Entities;

namespace Laundry.API.Interfaces;

public interface IGarmentTypeRepository
{
    Task<List<GarmentType>> GetAllAsync();

    Task<GarmentType?> GetByIdAsync(int id);

    Task<GarmentType?> GetByNameAsync(string name);

    Task<GarmentType> AddAsync(GarmentType garmentType);

    Task UpdateAsync(GarmentType garmentType);

    Task DeleteAsync(GarmentType garmentType);

    Task<bool> ExistsAsync(int id);

    Task SaveChangesAsync();
}
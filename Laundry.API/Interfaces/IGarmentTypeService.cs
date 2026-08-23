using Laundry.API.DTOs.GarmentType;

namespace Laundry.API.Services.Interfaces;

public interface IGarmentTypeService
{
    Task<IEnumerable<GarmentTypeResponse>> GetAllAsync();

    Task<GarmentTypeResponse?> GetByIdAsync(int id);

    Task<GarmentTypeResponse> CreateAsync(
        CreateGarmentTypeRequest request);

    Task<bool> UpdateAsync(
        int id,
        CreateGarmentTypeRequest request);

    Task<bool> DeleteAsync(int id);
}
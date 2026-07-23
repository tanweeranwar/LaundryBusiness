using Laundry.API.DTOs.ServiceCategory;

namespace Laundry.API.Services;

public interface IServiceCategoryService
{
    Task<IEnumerable<ServiceCategoryResponse>> GetAllAsync();

    Task<ServiceCategoryResponse?> GetByIdAsync(int id);

    Task<ServiceCategoryResponse> CreateAsync(CreateServiceCategoryRequest request);

    Task<bool> UpdateAsync(int id, CreateServiceCategoryRequest request);

    Task<bool> DeleteAsync(int id);
}
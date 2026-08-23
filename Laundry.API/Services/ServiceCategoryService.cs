using Laundry.API.DTOs.ServiceCategory;
using Laundry.API.Entities;
using Laundry.API.Interfaces;
using Laundry.API.Repositories;
using Laundry.API.Common.Exceptions;

namespace Laundry.API.Services;

public class ServiceCategoryService : IServiceCategoryService
{
    private readonly IServiceCategoryRepository _repository;

    public ServiceCategoryService(IServiceCategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ServiceCategoryResponse>> GetAllAsync()
    {
        var categories = await _repository.GetAllAsync();

        return categories.Select(MapToResponse).ToList();
    }

    public async Task<ServiceCategoryResponse?> GetByIdAsync(int id)
    {
        var category = await _repository.GetByIdAsync(id);

        return category == null
            ? null
            : MapToResponse(category);
    }

    public async Task<ServiceCategoryResponse> CreateAsync(
        CreateServiceCategoryRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var name = request.Name.Trim();

        var existing = await _repository.GetByNameAsync(name);

        if (existing != null)
        {
            throw new DuplicateServiceCategoryException(
                $"Service category '{name}' already exists.");
        }

        var category = new ServiceCategory
        {
            Name = name,
            Description = request.Description?.Trim() ?? string.Empty,
            DisplayOrder = request.DisplayOrder,
            Icon = string.IsNullOrWhiteSpace(request.Icon)
                ? null
                : request.Icon.Trim(),
            IsActive = true,
            CreatedOn = DateTime.UtcNow
        };

        await _repository.AddAsync(category);
        await _repository.SaveChangesAsync();

        return MapToResponse(category);
    }

    public async Task<bool> UpdateAsync(
        int id,
        CreateServiceCategoryRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var category = await _repository.GetByIdAsync(id);

        if (category == null)
            return false;

        var name = request.Name.Trim();

        var existing = await _repository.GetByNameAsync(name);

        if (existing != null && existing.Id != id)
        {
            throw new DuplicateServiceCategoryException(
                $"Service category '{name}' already exists.");
        }

        category.Name = name;
        category.Description = request.Description?.Trim() ?? string.Empty;
        category.DisplayOrder = request.DisplayOrder;
        category.Icon = string.IsNullOrWhiteSpace(request.Icon)
            ? null
            : request.Icon.Trim();
        category.UpdatedOn = DateTime.UtcNow;

        await _repository.UpdateAsync(category);
        await _repository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _repository.GetByIdAsync(id);

        if (category == null)
            return false;

        await _repository.DeleteAsync(category);
        await _repository.SaveChangesAsync();

        return true;
    }

    private static ServiceCategoryResponse MapToResponse(
        ServiceCategory category)
    {
        return new ServiceCategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            DisplayOrder = category.DisplayOrder,
            IsActive = category.IsActive
        };
    }
}
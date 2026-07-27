using Laundry.API.DTOs.BranchPricing;
using Laundry.API.Entities;
using Laundry.API.Interfaces;
using Laundry.API.Exceptions;

namespace Laundry.API.Services;

public class BranchPricingService : IBranchPricingService
{
    private readonly IBranchPricingRepository _repository;

    public BranchPricingService(IBranchPricingRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<BranchPricingDto>> GetAllAsync()
        => (await _repository.GetAllAsync()).Select(MapToDto);

    public async Task<BranchPricingDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : MapToDto(entity);
    }

    public async Task<IEnumerable<BranchPricingDto>> GetByBranchAsync(int branchId)
        => (await _repository.GetByBranchAsync(branchId)).Select(MapToDto);

    public async Task<BranchPricingDto> CreateAsync(CreateBranchPricingDto dto)
    {
        Validate(dto);

        if (await _repository.ExistsAsync(dto.BranchId, dto.ServiceCategoryId, dto.GarmentTypeId))
            throw new DuplicateBranchPricingException();

        var entity = new BranchPricing
        {
            BranchId = dto.BranchId,
            ServiceCategoryId = dto.ServiceCategoryId,
            GarmentTypeId = dto.GarmentTypeId,
            Price = dto.Price,
            IsExpressAvailable = dto.IsExpressAvailable,
            ExpressPrice = dto.IsExpressAvailable ? dto.ExpressPrice : null,
            EstimatedProcessingHours = dto.EstimatedProcessingHours,
            IsActive = true,
            CreatedOn = DateTime.UtcNow
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        entity = await _repository.GetByIdAsync(entity.Id)
                 ?? throw new BranchPricingNotFoundException(entity.Id);

        return MapToDto(entity);
    }

    public async Task<BranchPricingDto> UpdateAsync(int id, UpdateBranchPricingDto dto)
    {
        Validate(dto);

        var entity = await _repository.GetByIdAsync(id)
                     ?? throw new BranchPricingNotFoundException(id);

        var duplicate = await _repository.GetByCombinationAsync(dto.BranchId, dto.ServiceCategoryId, dto.GarmentTypeId);

        if (duplicate != null && duplicate.Id != id)
            throw new DuplicateBranchPricingException();

        entity.BranchId = dto.BranchId;
        entity.ServiceCategoryId = dto.ServiceCategoryId;
        entity.GarmentTypeId = dto.GarmentTypeId;
        entity.Price = dto.Price;
        entity.IsExpressAvailable = dto.IsExpressAvailable;
        entity.ExpressPrice = dto.IsExpressAvailable ? dto.ExpressPrice : null;
        entity.EstimatedProcessingHours = dto.EstimatedProcessingHours;
        entity.UpdatedOn = DateTime.UtcNow;

        _repository.Update(entity);
        await _repository.SaveChangesAsync();

        entity = await _repository.GetByIdAsync(id)
                 ?? throw new BranchPricingNotFoundException(id);

        return MapToDto(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id)
                     ?? throw new BranchPricingNotFoundException(id);

        _repository.Delete(entity);
        await _repository.SaveChangesAsync();
    }

    private static void Validate(CreateBranchPricingDto dto)
    {
        if (dto.Price <= 0) throw new ArgumentException("Price must be greater than zero.");
        if (dto.EstimatedProcessingHours <= 0) throw new ArgumentException("Estimated processing hours must be greater than zero.");
        if (dto.IsExpressAvailable)
        {
            if (!dto.ExpressPrice.HasValue)
                throw new ArgumentException("Express price is required.");
            if (dto.ExpressPrice.Value < dto.Price)
                throw new ArgumentException("Express price cannot be less than regular price.");
        }
    }

    private static void Validate(UpdateBranchPricingDto dto)
    {
        if (dto.Price <= 0) throw new ArgumentException("Price must be greater than zero.");
        if (dto.EstimatedProcessingHours <= 0) throw new ArgumentException("Estimated processing hours must be greater than zero.");
        if (dto.IsExpressAvailable)
        {
            if (!dto.ExpressPrice.HasValue)
                throw new ArgumentException("Express price is required.");
            if (dto.ExpressPrice.Value < dto.Price)
                throw new ArgumentException("Express price cannot be less than regular price.");
        }
    }

    private static BranchPricingDto MapToDto(BranchPricing entity)
    {
        return new BranchPricingDto
        {
            Id = entity.Id,
            BranchId = entity.BranchId,
            BranchName = entity.Branch.BranchName,
            ServiceCategoryId = entity.ServiceCategoryId,
            ServiceCategoryName = entity.ServiceCategory.Name,
            GarmentTypeId = entity.GarmentTypeId,
            GarmentTypeName = entity.GarmentType.Name,
            Price = entity.Price,
            IsExpressAvailable = entity.IsExpressAvailable,
            ExpressPrice = entity.ExpressPrice,
            EstimatedProcessingHours = entity.EstimatedProcessingHours,
            DisplayName = $"{entity.ServiceCategory.Name} - {entity.GarmentType.Name}"
        };
    }
}

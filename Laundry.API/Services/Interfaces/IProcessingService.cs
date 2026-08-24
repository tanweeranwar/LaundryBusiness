using Laundry.API.DTOs.Processing;

namespace Laundry.API.Services.Interfaces;

public interface IProcessingService
{
    Task<IEnumerable<ProcessingDto>> StartProcessingAsync(
        int orderId,
        StartProcessingDto request);

    Task<ProcessingDto?> GetByIdAsync(int id);

    Task<ProcessingDto?> GetByOrderItemIdAsync(
        int orderItemId);

    Task<IEnumerable<ProcessingDto>> GetByOrderIdAsync(
        int orderId);

    Task<ProcessingDto> UpdateStepStatusAsync(
    int processingId,
    int stepId,
    UpdateProcessingStepDto request);
}
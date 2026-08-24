using Laundry.API.DTOs.Processing;
using Laundry.API.Entities;
using Laundry.API.Enums;
using Laundry.API.Interfaces;
using Laundry.API.Services.Interfaces;
using Laundry.API.DTOs.Orders;

namespace Laundry.API.Services;

public class ProcessingService : IProcessingService
{
    private readonly IProcessingRepository _processingRepository;
    private readonly IOrderService _orderService;

    public ProcessingService(
        IProcessingRepository processingRepository,
        IOrderService orderService)
    {
        _processingRepository = processingRepository;
        _orderService = orderService;
    }

    public async Task<IEnumerable<ProcessingDto>> StartProcessingAsync(
        int orderId,
        StartProcessingDto request)
    {
        var order = await _processingRepository
            .GetOrderWithItemsAsync(orderId);

        if (order == null)
        {
            throw new KeyNotFoundException(
                $"Order '{orderId}' does not exist.");
        }

        if (order.Status != OrderStatus.Received)
        {
            throw new InvalidOperationException(
                $"Order '{orderId}' cannot start processing " +
                $"while its status is '{order.Status}'.");
        }

        if (!order.Items.Any())
        {
            throw new InvalidOperationException(
                $"Order '{orderId}' has no items to process.");
        }

        var existingProcessing =
            await _processingRepository.GetByOrderIdAsync(orderId);

        if (existingProcessing.Count > 0)
        {
            throw new InvalidOperationException(
                $"Processing already exists for order '{orderId}'.");
        }

        var results = new List<OrderItemProcessing>();

        foreach (var orderItem in order.Items)
        {
            var workflow =
                await _processingRepository
                    .GetWorkflowByServiceCategoryAsync(
                        orderItem.ServiceCategoryId);

            if (workflow == null)
            {
                throw new InvalidOperationException(
                    $"No active processing workflow exists for " +
                    $"service category '{orderItem.ServiceCategoryId}'.");
            }

            var processing = new OrderItemProcessing
            {
                OrderItemId = orderItem.Id,
                ProcessingWorkflowId = workflow.Id,
                Status = ProcessingStatus.Pending,
                AssignedTo = request.AssignedTo,
                Remarks = request.Remarks,

                Steps = workflow.Steps
                    .OrderBy(x => x.Sequence)
                    .Select(step => new OrderItemProcessingStep
                    {
                        ProcessingWorkflowStepId = step.Id,
                        Status = ProcessingStatus.Pending
                    })
                    .ToList()
            };

            await _processingRepository.AddAsync(processing);

            results.Add(processing);
        }

        await _processingRepository.SaveChangesAsync();

        var processingResults = new List<ProcessingDto>();

        foreach (var processing in results)
        {
            var saved =
                await _processingRepository.GetByIdAsync(
                    processing.Id);

            if (saved != null)
            {
                processingResults.Add(MapToDto(saved));
            }
        }

        return processingResults;
    }

    public async Task<ProcessingDto?> GetByIdAsync(int id)
    {
        var processing =
            await _processingRepository.GetByIdAsync(id);

        return processing == null
            ? null
            : MapToDto(processing);
    }

    public async Task<ProcessingDto?> GetByOrderItemIdAsync(
        int orderItemId)
    {
        var processing =
            await _processingRepository
                .GetByOrderItemIdAsync(orderItemId);

        return processing == null
            ? null
            : MapToDto(processing);
    }

    public async Task<IEnumerable<ProcessingDto>> GetByOrderIdAsync(
        int orderId)
    {
        var processing =
            await _processingRepository
                .GetByOrderIdAsync(orderId);

        return processing.Select(MapToDto);
    }

    public async Task<ProcessingDto> UpdateStepStatusAsync(
        int processingId,
        int stepId,
        UpdateProcessingStepDto request)
    {
        var processing =
            await _processingRepository
                .GetForUpdateAsync(processingId);

        if (processing == null)
        {
            throw new KeyNotFoundException(
                $"Processing '{processingId}' does not exist.");
        }

        var step = processing.Steps
            .FirstOrDefault(x => x.Id == stepId);

        if (step == null)
        {
            throw new KeyNotFoundException(
                $"Processing step '{stepId}' does not exist.");
        }

        if (!Enum.IsDefined(
                typeof(ProcessingStatus),
                request.Status))
        {
            throw new InvalidOperationException(
                "Invalid processing status.");
        }

        var newStatus =
            (ProcessingStatus)request.Status;

        var currentStatus = step.Status;

        ValidateStepTransition(
            currentStatus,
            newStatus);

        // A step can only start after all previous
        // required steps have been completed.
        if (newStatus == ProcessingStatus.InProgress)
        {
            var previousRequiredSteps = processing.Steps
                .Where(x =>
                    x.ProcessingWorkflowStep.Sequence <
                    step.ProcessingWorkflowStep.Sequence &&
                    x.ProcessingWorkflowStep.IsRequired)
                .OrderByDescending(x =>
                    x.ProcessingWorkflowStep.Sequence)
                .ToList();

            var incompletePreviousStep =
                previousRequiredSteps.FirstOrDefault(
                    x => x.Status != ProcessingStatus.Completed);

            if (incompletePreviousStep != null)
            {
                throw new InvalidOperationException(
                    $"Previous required processing step " +
                    $"'{incompletePreviousStep.ProcessingWorkflowStep.StepType}' " +
                    $"must be completed before starting " +
                    $"'{step.ProcessingWorkflowStep.StepType}'.");
            }
        }

        // Start step
        if (newStatus == ProcessingStatus.InProgress)
        {
            step.StartedOn ??= DateTime.UtcNow;

            // The parent processing starts when its
            // first step starts.
            processing.StartedOn ??= step.StartedOn;
        }

        // Complete step
        if (newStatus == ProcessingStatus.Completed)
        {
            if (step.StartedOn == null)
            {
                throw new InvalidOperationException(
                    "A processing step must be started before it can be completed.");
            }

            step.CompletedOn = DateTime.UtcNow;
        }

        // Update step status
        step.Status = newStatus;

        if (!string.IsNullOrWhiteSpace(request.AssignedTo))
        {
            step.AssignedTo = request.AssignedTo;
        }

        if (!string.IsNullOrWhiteSpace(request.Remarks))
        {
            step.Remarks = request.Remarks;
        }

        // Check whether all required workflow steps
        // have now been completed.
        var allRequiredStepsCompleted = processing.Steps
            .Where(x => x.ProcessingWorkflowStep.IsRequired)
            .All(x => x.Status == ProcessingStatus.Completed);

        if (allRequiredStepsCompleted)
        {
            processing.Status = ProcessingStatus.Completed;
            processing.CompletedOn ??= DateTime.UtcNow;

            var allOrderItemsCompleted =
                await _processingRepository
                    .AreAllOrderItemsProcessingCompletedAsync(
                        processing.OrderItem.OrderId);

            if (allOrderItemsCompleted)
            {
                await _orderService.MarkReadyAfterProcessingAsync(
                    processing.OrderItem.OrderId);
            }
        }

        await _processingRepository.SaveChangesAsync();

        var updated =
            await _processingRepository.GetByIdAsync(
                processingId);

        if (updated == null)
        {
            throw new InvalidOperationException(
                "Processing could not be reloaded after update.");
        }

        return MapToDto(updated);
    }

    private static void ValidateStepTransition(
        ProcessingStatus currentStatus,
        ProcessingStatus newStatus)
    {
        if (currentStatus == newStatus)
        {
            throw new InvalidOperationException(
                $"Processing step is already '{currentStatus}'.");
        }

        var valid = currentStatus switch
        {
            ProcessingStatus.Pending =>
                newStatus == ProcessingStatus.InProgress,

            ProcessingStatus.InProgress =>
                newStatus == ProcessingStatus.Completed,

            ProcessingStatus.Completed =>
                false,

            ProcessingStatus.Failed =>
                false,

            ProcessingStatus.Cancelled =>
                false,

            _ => false
        };

        if (!valid)
        {
            throw new InvalidOperationException(
                $"Processing step cannot transition from " +
                $"'{currentStatus}' to '{newStatus}'.");
        }
    }

    private static ProcessingDto MapToDto(
        OrderItemProcessing processing)
    {
        return new ProcessingDto
        {
            Id = processing.Id,

            OrderItemId = processing.OrderItemId,

            OrderId = processing.OrderItem.OrderId,

            ServiceCategoryName =
                processing.OrderItem.ServiceCategory.Name,

            GarmentTypeName =
                processing.OrderItem.GarmentType.Name,

            Quantity =
                processing.OrderItem.Quantity,

            ProcessingWorkflowId =
                processing.ProcessingWorkflowId,

            WorkflowName =
                processing.ProcessingWorkflow.Name,

            Status =
                processing.Status,

            StartedOn =
                processing.StartedOn,

            CompletedOn =
                processing.CompletedOn,

            AssignedTo =
                processing.AssignedTo,

            Remarks =
                processing.Remarks,

            Steps = processing.Steps
                .OrderBy(x =>
                    x.ProcessingWorkflowStep.Sequence)
                .Select(x => new ProcessingStepDto
                {
                    Id =
                        x.Id,

                    ProcessingWorkflowStepId =
                        x.ProcessingWorkflowStepId,

                    Sequence =
                        x.ProcessingWorkflowStep.Sequence,

                    StepType =
                        x.ProcessingWorkflowStep.StepType,

                    Status =
                        x.Status,

                    IsRequired =
                        x.ProcessingWorkflowStep.IsRequired,

                    StartedOn =
                        x.StartedOn,

                    CompletedOn =
                        x.CompletedOn,

                    AssignedTo =
                        x.AssignedTo,

                    Remarks =
                        x.Remarks
                })
                .ToList()
        };
    }
}
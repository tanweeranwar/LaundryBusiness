using Laundry.API.DTOs.Processing;
using Laundry.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Laundry.API.Controllers;

[Authorize(Roles = "Super Admin,Branch Admin,Employee")]
[ApiController]
[Route("api/[controller]")]
public class ProcessingController : ControllerBase
{
    private readonly IProcessingService _processingService;
    private readonly IBranchAuthorizationService _branchAuthorization;

    public ProcessingController(
        IProcessingService processingService,
        IBranchAuthorizationService branchAuthorization)
    {
        _processingService = processingService;
        _branchAuthorization = branchAuthorization;
    }

    [HttpPost("order/{orderId:int}/start")]
    public async Task<ActionResult<IEnumerable<ProcessingDto>>> Start(
        int orderId,
        StartProcessingDto request)
    {
        if (!await _branchAuthorization.CanAccessOrderAsync(orderId))
            return Forbid();

        try
        {
            var processing = await _processingService.StartProcessingAsync(orderId, request);
            return StatusCode(StatusCodes.Status201Created, processing);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                Success = false,
                Message = ex.Message,
                Data = (object?)null,
                Errors = (object?)null
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                Success = false,
                Message = ex.Message,
                Data = (object?)null,
                Errors = (object?)null
            });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProcessingDto>> GetById(int id)
    {
        var processing = await _processingService.GetByIdAsync(id);

        if (processing == null)
            return NotFound();

        if (!await _branchAuthorization.CanAccessProcessingAsync(id))
            return Forbid();

        return Ok(processing);
    }

    [HttpGet("order-item/{orderItemId:int}")]
    public async Task<ActionResult<ProcessingDto>> GetByOrderItemId(int orderItemId)
    {
        if (!await _branchAuthorization.CanAccessOrderItemAsync(orderItemId))
            return Forbid();

        var processing = await _processingService.GetByOrderItemIdAsync(orderItemId);

        if (processing == null)
            return NotFound();

        return Ok(processing);
    }

    [HttpGet("order/{orderId:int}")]
    public async Task<ActionResult<IEnumerable<ProcessingDto>>> GetByOrderId(int orderId)
    {
        if (!await _branchAuthorization.CanAccessOrderAsync(orderId))
            return Forbid();

        var processing = await _processingService.GetByOrderIdAsync(orderId);
        return Ok(processing);
    }

    [HttpPut("{processingId:int}/steps/{stepId:int}/status")]
    public async Task<ActionResult<ProcessingDto>> UpdateStepStatus(
        int processingId,
        int stepId,
        UpdateProcessingStepDto request)
    {
        if (!await _branchAuthorization.CanAccessProcessingAsync(processingId))
            return Forbid();

        try
        {
            var processing = await _processingService.UpdateStepStatusAsync(
                processingId,
                stepId,
                request);

            return Ok(processing);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                Success = false,
                Message = ex.Message,
                Data = (object?)null,
                Errors = (object?)null
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                Success = false,
                Message = ex.Message,
                Data = (object?)null,
                Errors = (object?)null
            });
        }
    }
}
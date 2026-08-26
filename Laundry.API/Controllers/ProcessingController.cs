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

    public ProcessingController(IProcessingService processingService)
    {
        _processingService = processingService;
    }

    [HttpPost("order/{orderId:int}/start")]
    [ProducesResponseType(typeof(IEnumerable<ProcessingDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<ProcessingDto>>> Start(
        int orderId,
        StartProcessingDto request)
    {
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
    [ProducesResponseType(typeof(ProcessingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProcessingDto>> GetById(int id)
    {
        var processing = await _processingService.GetByIdAsync(id);

        if (processing == null)
            return NotFound();

        return Ok(processing);
    }

    [HttpGet("order-item/{orderItemId:int}")]
    [ProducesResponseType(typeof(ProcessingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProcessingDto>> GetByOrderItemId(int orderItemId)
    {
        var processing = await _processingService.GetByOrderItemIdAsync(orderItemId);

        if (processing == null)
            return NotFound();

        return Ok(processing);
    }

    [HttpGet("order/{orderId:int}")]
    [ProducesResponseType(typeof(IEnumerable<ProcessingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProcessingDto>>> GetByOrderId(int orderId)
    {
        var processing = await _processingService.GetByOrderIdAsync(orderId);
        return Ok(processing);
    }

    [HttpPut("{processingId:int}/steps/{stepId:int}/status")]
    [ProducesResponseType(typeof(ProcessingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProcessingDto>> UpdateStepStatus(
        int processingId,
        int stepId,
        UpdateProcessingStepDto request)
    {
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
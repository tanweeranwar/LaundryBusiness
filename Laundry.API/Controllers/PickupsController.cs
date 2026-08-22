using Laundry.API.DTOs.Pickup;
using Laundry.API.Enums;
using Laundry.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Laundry.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PickupsController : ControllerBase
{
    private readonly IPickupService _pickupService;

    public PickupsController(IPickupService pickupService)
    {
        _pickupService = pickupService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(PickupDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<PickupDto>> Create(
        CreatePickupDto request)
    {
        var pickup = await _pickupService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = pickup.Id },
            pickup);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PickupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PickupDto>> GetById(int id)
    {
        var pickup = await _pickupService.GetByIdAsync(id);

        if (pickup == null)
            return NotFound();

        return Ok(pickup);
    }

    [HttpGet("order/{orderId:int}")]
    [ProducesResponseType(typeof(PickupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PickupDto>> GetByOrderId(int orderId)
    {
        var pickup = await _pickupService.GetByOrderIdAsync(orderId);

        if (pickup == null)
            return NotFound();

        return Ok(pickup);
    }

    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(IEnumerable<PickupDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PickupDto>>> GetByStatus(
        PickupStatus status)
    {
        var pickups = await _pickupService.GetByStatusAsync(status);

        return Ok(pickups);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(PickupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PickupDto>> Update(
        int id,
        UpdatePickupDto request)
    {
        try
        {
            var pickup = await _pickupService.UpdateAsync(id, request);

            return Ok(pickup);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
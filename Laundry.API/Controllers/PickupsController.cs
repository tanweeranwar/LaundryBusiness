using System.Security.Claims;
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
    private readonly IOrderService _orderService;

    public PickupsController(
        IPickupService pickupService,
        IOrderService orderService)
    {
        _pickupService = pickupService;
        _orderService = orderService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(PickupDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PickupDto>> Create(CreatePickupDto request)
    {
        if (!await CanAccessOrderAsync(request.OrderId))
            return Forbid();

        var pickup = await _pickupService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = pickup.Id },
            pickup);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PickupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PickupDto>> GetById(int id)
    {
        var pickup = await _pickupService.GetByIdAsync(id);

        if (pickup == null)
            return NotFound();

        if (!await CanAccessOrderAsync(pickup.OrderId))
            return Forbid();

        return Ok(pickup);
    }

    [HttpGet("order/{orderId:int}")]
    [ProducesResponseType(typeof(PickupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PickupDto>> GetByOrderId(int orderId)
    {
        if (!await CanAccessOrderAsync(orderId))
            return Forbid();

        var pickup = await _pickupService.GetByOrderIdAsync(orderId);

        if (pickup == null)
            return NotFound();

        return Ok(pickup);
    }

    [HttpGet("status/{status}")]
    [Authorize(Roles = "Super Admin,Branch Admin,Employee")]
    [ProducesResponseType(typeof(IEnumerable<PickupDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PickupDto>>> GetByStatus(PickupStatus status)
    {
        var pickups = await _pickupService.GetByStatusAsync(status);
        return Ok(pickups);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Super Admin,Branch Admin,Employee")]
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

    private async Task<bool> CanAccessOrderAsync(int orderId)
    {
        if (IsStaff())
            return true;

        if (!User.IsInRole("Customer"))
            return false;

        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(claim, out var userId))
            return false;

        var order = await _orderService.GetByIdAsync(orderId);
        return order?.CustomerId == userId;
    }

    private bool IsStaff() =>
        User.IsInRole("Super Admin") ||
        User.IsInRole("Branch Admin") ||
        User.IsInRole("Employee");
}
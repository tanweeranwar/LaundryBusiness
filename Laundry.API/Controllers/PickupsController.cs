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
    private readonly IBranchAuthorizationService _branchAuthorization;

    public PickupsController(
        IPickupService pickupService,
        IBranchAuthorizationService branchAuthorization)
    {
        _pickupService = pickupService;
        _branchAuthorization = branchAuthorization;
    }

    [HttpPost]
    [Authorize(Roles = "Super Admin,Branch Admin,Employee")]
    public async Task<ActionResult<PickupDto>> Create(CreatePickupDto request)
    {
        if (!await _branchAuthorization.CanAccessOrderAsync(request.OrderId))
            return Forbid();

        var pickup = await _pickupService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = pickup.Id },
            pickup);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PickupDto>> GetById(int id)
    {
        var pickup = await _pickupService.GetByIdAsync(id);

        if (pickup == null)
            return NotFound();

        if (!await _branchAuthorization.CanAccessPickupAsync(id))
            return Forbid();

        return Ok(pickup);
    }

    [HttpGet("order/{orderId:int}")]
    public async Task<ActionResult<PickupDto>> GetByOrderId(int orderId)
    {
        if (!await _branchAuthorization.CanAccessOrderAsync(orderId))
            return Forbid();

        var pickup = await _pickupService.GetByOrderIdAsync(orderId);

        if (pickup == null)
            return NotFound();

        return Ok(pickup);
    }

    [HttpGet("status/{status}")]
    [Authorize(Roles = "Super Admin,Branch Admin,Employee,Delivery Agent")]
    public async Task<ActionResult<IEnumerable<PickupDto>>> GetByStatus(PickupStatus status)
    {
        var pickups = await _pickupService.GetByStatusAsync(status);

        if (_branchAuthorization.IsSuperAdmin)
            return Ok(pickups);

        var scoped = new List<PickupDto>();
        foreach (var pickup in pickups)
        {
            if (await _branchAuthorization.CanAccessOrderAsync(pickup.OrderId))
                scoped.Add(pickup);
        }

        return Ok(scoped);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Super Admin,Branch Admin,Employee,Delivery Agent")]
    public async Task<ActionResult<PickupDto>> Update(
        int id,
        UpdatePickupDto request)
    {
        if (!await _branchAuthorization.CanAccessPickupAsync(id))
            return Forbid();

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
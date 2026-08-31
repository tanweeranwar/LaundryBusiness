using Laundry.API.DTOs.Delivery;
using Laundry.API.Enums;
using Laundry.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Laundry.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DeliveriesController : ControllerBase
{
    private readonly IDeliveryService _deliveryService;
    private readonly IBranchAuthorizationService _branchAuthorization;

    public DeliveriesController(
        IDeliveryService deliveryService,
        IBranchAuthorizationService branchAuthorization)
    {
        _deliveryService = deliveryService;
        _branchAuthorization = branchAuthorization;
    }

    [HttpPost]
    [Authorize(Roles = "Super Admin,Branch Admin,Employee")]
    public async Task<ActionResult<DeliveryDto>> Create(CreateDeliveryDto request)
    {
        if (!await _branchAuthorization.CanAccessOrderAsync(request.OrderId))
            return Forbid();

        var delivery = await _deliveryService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = delivery.Id },
            delivery);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DeliveryDto>> GetById(int id)
    {
        var delivery = await _deliveryService.GetByIdAsync(id);

        if (delivery == null)
            return NotFound();

        if (!await _branchAuthorization.CanAccessDeliveryAsync(id))
            return Forbid();

        return Ok(delivery);
    }

    [HttpGet("order/{orderId:int}")]
    public async Task<ActionResult<DeliveryDto>> GetByOrderId(int orderId)
    {
        if (!await _branchAuthorization.CanAccessOrderAsync(orderId))
            return Forbid();

        var delivery = await _deliveryService.GetByOrderIdAsync(orderId);

        if (delivery == null)
            return NotFound();

        return Ok(delivery);
    }

    [HttpGet("status/{status}")]
    [Authorize(Roles = "Super Admin,Branch Admin,Employee,Delivery Agent")]
    public async Task<ActionResult<IEnumerable<DeliveryDto>>> GetByStatus(DeliveryStatus status)
    {
        var deliveries = await _deliveryService.GetByStatusAsync(status);

        if (_branchAuthorization.IsSuperAdmin)
            return Ok(deliveries);

        var scoped = new List<DeliveryDto>();
        foreach (var delivery in deliveries)
        {
            if (await _branchAuthorization.CanAccessOrderAsync(delivery.OrderId))
                scoped.Add(delivery);
        }

        return Ok(scoped);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Super Admin,Branch Admin,Employee,Delivery Agent")]
    public async Task<ActionResult<DeliveryDto>> Update(
        int id,
        UpdateDeliveryDto request)
    {
        if (!await _branchAuthorization.CanAccessDeliveryAsync(id))
            return Forbid();

        try
        {
            var delivery = await _deliveryService.UpdateAsync(id, request);
            return Ok(delivery);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
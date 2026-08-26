using System.Security.Claims;
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
    private readonly IOrderService _orderService;

    public DeliveriesController(
        IDeliveryService deliveryService,
        IOrderService orderService)
    {
        _deliveryService = deliveryService;
        _orderService = orderService;
    }

    [HttpPost]
    [Authorize(Roles = "Super Admin,Branch Admin,Employee")]
    [ProducesResponseType(typeof(DeliveryDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<DeliveryDto>> Create(CreateDeliveryDto request)
    {
        var delivery = await _deliveryService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = delivery.Id },
            delivery);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(DeliveryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DeliveryDto>> GetById(int id)
    {
        var delivery = await _deliveryService.GetByIdAsync(id);

        if (delivery == null)
            return NotFound();

        if (!await CanAccessOrderAsync(delivery.OrderId))
            return Forbid();

        return Ok(delivery);
    }

    [HttpGet("order/{orderId:int}")]
    [ProducesResponseType(typeof(DeliveryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DeliveryDto>> GetByOrderId(int orderId)
    {
        if (!await CanAccessOrderAsync(orderId))
            return Forbid();

        var delivery = await _deliveryService.GetByOrderIdAsync(orderId);

        if (delivery == null)
            return NotFound();

        return Ok(delivery);
    }

    [HttpGet("status/{status}")]
    [Authorize(Roles = "Super Admin,Branch Admin,Employee")]
    [ProducesResponseType(typeof(IEnumerable<DeliveryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<DeliveryDto>>> GetByStatus(DeliveryStatus status)
    {
        var deliveries = await _deliveryService.GetByStatusAsync(status);
        return Ok(deliveries);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Super Admin,Branch Admin,Employee")]
    [ProducesResponseType(typeof(DeliveryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DeliveryDto>> Update(
        int id,
        UpdateDeliveryDto request)
    {
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
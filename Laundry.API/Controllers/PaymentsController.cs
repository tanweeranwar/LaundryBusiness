using System.Security.Claims;
using Laundry.API.DTOs.Payment;
using Laundry.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Laundry.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IOrderService _orderService;

    public PaymentsController(
        IPaymentService paymentService,
        IOrderService orderService)
    {
        _paymentService = paymentService;
        _orderService = orderService;
    }

    /// <summary>
    /// Records a payment against an order. Payment recording is restricted to staff.
    /// Customer-facing online payment integration should create payments through a
    /// trusted payment workflow rather than accepting arbitrary payment records.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Super Admin,Branch Admin,Employee")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaymentDto>> Create(CreatePaymentDto request)
    {
        var payment = await _paymentService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = payment.Id },
            payment);
    }

    /// <summary>
    /// Gets a payment by Id. Customers may only access payments belonging to their orders.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentDto>> GetById(int id)
    {
        var payment = await _paymentService.GetByIdAsync(id);

        if (payment == null)
            return NotFound();

        if (!await CanAccessOrderAsync(payment.OrderId))
            return Forbid();

        return Ok(payment);
    }

    /// <summary>
    /// Gets all payments for an order. Customer tokens are restricted to their own order.
    /// </summary>
    [HttpGet("order/{orderId:int}")]
    [ProducesResponseType(typeof(IEnumerable<PaymentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetByOrder(int orderId)
    {
        if (User.IsInRole("Customer"))
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var order = await _orderService.GetByIdAsync(orderId);

            if (order == null)
                return NotFound();

            if (order.CustomerId != userId)
                return Forbid();
        }
        else if (!IsStaff())
        {
            return Forbid();
        }

        var payments = await _paymentService.GetByOrderIdAsync(orderId);
        return Ok(payments);
    }

    /// <summary>
    /// Updates payment details. Restricted to staff.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Super Admin,Branch Admin,Employee")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentDto>> Update(
        int id,
        UpdatePaymentDto request)
    {
        var payment = await _paymentService.UpdateAsync(id, request);
        return Ok(payment);
    }

    private async Task<bool> CanAccessOrderAsync(int orderId)
    {
        if (IsStaff())
            return true;

        if (!User.IsInRole("Customer"))
            return false;

        if (!TryGetCurrentUserId(out var userId))
            return false;

        var order = await _orderService.GetByIdAsync(orderId);
        return order?.CustomerId == userId;
    }

    private bool IsStaff() =>
        User.IsInRole("Super Admin") ||
        User.IsInRole("Branch Admin") ||
        User.IsInRole("Employee");

    private bool TryGetCurrentUserId(out Guid userId)
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("sub");

        return Guid.TryParse(claim, out userId);
    }
}
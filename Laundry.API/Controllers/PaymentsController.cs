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

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    /// <summary>
    /// Records a payment against an order.
    /// </summary>
    [HttpPost]
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
    /// Gets a payment by Id.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentDto>> GetById(int id)
    {
        var payment = await _paymentService.GetByIdAsync(id);

        if (payment == null)
            return NotFound();

        return Ok(payment);
    }

    /// <summary>
    /// Gets all payments for an order.
    /// </summary>
    [HttpGet("order/{orderId:int}")]
    [ProducesResponseType(typeof(IEnumerable<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetByOrder(int orderId)
    {
        var payments = await _paymentService.GetByOrderIdAsync(orderId);

        return Ok(payments);
    }

    /// <summary>
    /// Updates payment details.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentDto>> Update(
        int id,
        UpdatePaymentDto request)
    {
        var payment = await _paymentService.UpdateAsync(id, request);

        return Ok(payment);
    }
}
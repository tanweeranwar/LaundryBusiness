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
    private readonly IBranchAuthorizationService _branchAuthorization;

    public PaymentsController(
        IPaymentService paymentService,
        IBranchAuthorizationService branchAuthorization)
    {
        _paymentService = paymentService;
        _branchAuthorization = branchAuthorization;
    }

    [HttpPost]
    [Authorize(Roles = "Super Admin,Branch Admin,Employee")]
    public async Task<ActionResult<PaymentDto>> Create(CreatePaymentDto request)
    {
        if (!await _branchAuthorization.CanAccessOrderAsync(request.OrderId))
            return Forbid();

        var payment = await _paymentService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = payment.Id },
            payment);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Customer,Super Admin,Branch Admin,Employee")]
    public async Task<ActionResult<PaymentDto>> GetById(int id)
    {
        var payment = await _paymentService.GetByIdAsync(id);

        if (payment == null)
            return NotFound();

        if (!await _branchAuthorization.CanAccessPaymentAsync(id))
            return Forbid();

        return Ok(payment);
    }

    [HttpGet("order/{orderId:int}")]
    [Authorize(Roles = "Customer,Super Admin,Branch Admin,Employee")]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetByOrder(int orderId)
    {
        if (!await _branchAuthorization.CanAccessOrderAsync(orderId))
            return Forbid();

        var payments = await _paymentService.GetByOrderIdAsync(orderId);
        return Ok(payments);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Super Admin,Branch Admin,Employee")]
    public async Task<ActionResult<PaymentDto>> Update(
        int id,
        UpdatePaymentDto request)
    {
        if (!await _branchAuthorization.CanAccessPaymentAsync(id))
            return Forbid();

        var payment = await _paymentService.UpdateAsync(id, request);
        return Ok(payment);
    }
}
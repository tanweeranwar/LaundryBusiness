using System.Security.Claims;
using Laundry.API.DTOs.Orders;
using Laundry.API.Interfaces;
using Laundry.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Laundry.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IOrderStatusHistoryService _orderStatusHistoryService;
    private readonly IBranchAuthorizationService _branchAuthorization;

    public OrdersController(
        IOrderService orderService,
        IOrderStatusHistoryService orderStatusHistoryService,
        IBranchAuthorizationService branchAuthorization)
    {
        _orderService = orderService;
        _orderStatusHistoryService = orderStatusHistoryService;
        _branchAuthorization = branchAuthorization;
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderDto>> Create(CreateOrderDto request)
    {
        if (IsCustomer())
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            request.CustomerId = userId;
        }
        else if (IsBranchScopedStaff())
        {
            if (request.BranchId != _branchAuthorization.CurrentBranchId ||
                !await _branchAuthorization.CanAccessCustomerAsync(request.CustomerId))
            {
                return Forbid();
            }
        }

        var order = await _orderService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = order.Id },
            order);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var order = await _orderService.GetByIdAsync(id);

        if (order == null)
            return NotFound();

        if (!await CanAccessCustomerResourceAsync(order.CustomerId))
            return Forbid();

        return Ok(order);
    }

    [HttpGet("number/{orderNumber}")]
    public async Task<ActionResult<OrderDto>> GetByOrderNumber(string orderNumber)
    {
        var order = await _orderService.GetByOrderNumberAsync(orderNumber);

        if (order == null)
            return NotFound();

        if (!await CanAccessCustomerResourceAsync(order.CustomerId))
            return Forbid();

        return Ok(order);
    }

    [HttpGet("customer/{customerId:guid}")]
    public async Task<ActionResult<IEnumerable<OrderSummaryDto>>> GetByCustomer(Guid customerId)
    {
        if (IsCustomer())
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            customerId = userId;
        }
        else if (!await _branchAuthorization.CanAccessCustomerAsync(customerId))
        {
            return Forbid();
        }

        var orders = await _orderService.GetByCustomerAsync(customerId);
        return Ok(orders);
    }

    [HttpGet("branch/{branchId:int}")]
    [Authorize(Roles = "Super Admin,Branch Admin,Employee")]
    public async Task<ActionResult<IEnumerable<OrderSummaryDto>>> GetByBranch(int branchId)
    {
        if (!_branchAuthorization.CanAccessBranch(branchId))
            return Forbid();

        var orders = await _orderService.GetByBranchAsync(branchId);
        return Ok(orders);
    }

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Super Admin,Branch Admin,Employee")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        UpdateOrderDto request)
    {
        if (!await _branchAuthorization.CanAccessOrderAsync(id))
            return Forbid();

        var updated = await _orderService.UpdateStatusAsync(id, request);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpGet("{id:int}/status-history")]
    public async Task<ActionResult<IEnumerable<OrderStatusHistoryDto>>> GetStatusHistory(int id)
    {
        var order = await _orderService.GetByIdAsync(id);

        if (order == null)
            return NotFound();

        if (!await _branchAuthorization.CanAccessOrderAsync(id))
            return Forbid();

        var history = await _orderStatusHistoryService.GetByOrderIdAsync(id);
        return Ok(history);
    }

    private bool IsCustomer() => User.IsInRole("Customer");

    private bool IsBranchScopedStaff() =>
        User.IsInRole("Branch Admin") ||
        User.IsInRole("Employee");

    private async Task<bool> CanAccessCustomerResourceAsync(Guid customerId)
    {
        if (User.IsInRole("Customer"))
            return TryGetCurrentUserId(out var userId) && userId == customerId;

        return await _branchAuthorization.CanAccessCustomerAsync(customerId);
    }

    private bool TryGetCurrentUserId(out Guid userId)
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("sub");

        return Guid.TryParse(claim, out userId);
    }
}
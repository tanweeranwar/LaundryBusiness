using System.Security.Claims;
using Laundry.API.DTOs.Order;
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

    public OrdersController(
        IOrderService orderService,
        IOrderStatusHistoryService orderStatusHistoryService)
    {
        _orderService = orderService;
        _orderStatusHistoryService = orderStatusHistoryService;
    }

    /// <summary>
    /// Creates a new laundry order.
    /// Customers can only create an order for themselves. Staff can create
    /// orders for any customer.
    /// </summary>
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

        var order = await _orderService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = order.Id },
            order);
    }

    /// <summary>
    /// Gets an order by Id. Customers may only access their own orders.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var order = await _orderService.GetByIdAsync(id);

        if (order == null)
            return NotFound();

        if (!CanAccessCustomerResource(order.CustomerId))
            return Forbid();

        return Ok(order);
    }

    /// <summary>
    /// Gets an order by order number. Customers may only access their own orders.
    /// </summary>
    [HttpGet("number/{orderNumber}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDto>> GetByOrderNumber(string orderNumber)
    {
        var order = await _orderService.GetByOrderNumberAsync(orderNumber);

        if (order == null)
            return NotFound();

        if (!CanAccessCustomerResource(order.CustomerId))
            return Forbid();

        return Ok(order);
    }

    /// <summary>
    /// Gets all orders for a customer. Customer tokens are always restricted
    /// to their own customer id, regardless of the route value supplied.
    /// </summary>
    [HttpGet("customer/{customerId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<OrderSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<OrderSummaryDto>>> GetByCustomer(Guid customerId)
    {
        if (IsCustomer())
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            customerId = userId;
        }
        else if (!IsStaff())
        {
            return Forbid();
        }

        var orders = await _orderService.GetByCustomerAsync(customerId);
        return Ok(orders);
    }

    /// <summary>
    /// Gets all orders for a branch. This is an operational endpoint and is
    /// restricted to staff. Branch-level scoping will be added once branch
    /// ownership/assignment is persisted for staff accounts.
    /// </summary>
    [HttpGet("branch/{branchId:int}")]
    [Authorize(Roles = "Super Admin,Branch Admin,Employee")]
    [ProducesResponseType(typeof(IEnumerable<OrderSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrderSummaryDto>>> GetByBranch(int branchId)
    {
        var orders = await _orderService.GetByBranchAsync(branchId);
        return Ok(orders);
    }

    /// <summary>
    /// Updates order status. Customers cannot change order state.
    /// </summary>
    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Super Admin,Branch Admin,Employee")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(
        int id,
        UpdateOrderDto request)
    {
        var updated = await _orderService.UpdateStatusAsync(id, request);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Gets the status history of an order. Customers may only see their own history.
    /// </summary>
    [HttpGet("{id:int}/status-history")]
    [ProducesResponseType(
        typeof(IEnumerable<OrderStatusHistoryDto>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<OrderStatusHistoryDto>>> GetStatusHistory(int id)
    {
        var order = await _orderService.GetByIdAsync(id);

        if (order == null)
            return NotFound();

        if (!CanAccessCustomerResource(order.CustomerId))
            return Forbid();

        var history = await _orderStatusHistoryService.GetByOrderIdAsync(id);
        return Ok(history);
    }

    private bool IsCustomer() => User.IsInRole("Customer");

    private bool IsStaff() =>
        User.IsInRole("Super Admin") ||
        User.IsInRole("Branch Admin") ||
        User.IsInRole("Employee");

    private bool CanAccessCustomerResource(Guid customerId)
    {
        if (IsStaff())
            return true;

        if (!IsCustomer())
            return false;

        return TryGetCurrentUserId(out var userId) && userId == customerId;
    }

    private bool TryGetCurrentUserId(out Guid userId)
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("sub");

        return Guid.TryParse(claim, out userId);
    }
}
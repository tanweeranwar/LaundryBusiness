//using Laundry.API.DTOs.Order;
using Laundry.API.DTOs.Order;
using Laundry.API.DTOs.Orders;
using Laundry.API.Interfaces;
using Laundry.API.Services;
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
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderDto>> Create(CreateOrderDto request)
    {
        var order = await _orderService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = order.Id },
            order);
    }

    /// <summary>
    /// Gets an order by Id.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var order = await _orderService.GetByIdAsync(id);

        if (order == null)
            return NotFound();

        return Ok(order);
    }

    /// <summary>
    /// Gets an order by order number.
    /// </summary>
    [HttpGet("number/{orderNumber}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDto>> GetByOrderNumber(string orderNumber)
    {
        var order = await _orderService.GetByOrderNumberAsync(orderNumber);

        if (order == null)
            return NotFound();

        return Ok(order);
    }

    /// <summary>
    /// Gets all orders for a customer.
    /// </summary>
    [HttpGet("customer/{customerId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<OrderSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrderSummaryDto>>> GetByCustomer(Guid customerId)
    {
        var orders = await _orderService.GetByCustomerAsync(customerId);

        return Ok(orders);
    }

    /// <summary>
    /// Gets all orders for a branch.
    /// </summary>
    [HttpGet("branch/{branchId:int}")]
    [ProducesResponseType(typeof(IEnumerable<OrderSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrderSummaryDto>>> GetByBranch(int branchId)
    {
        var orders = await _orderService.GetByBranchAsync(branchId);

        return Ok(orders);
    }

    /// <summary>
    /// Updates order status.
    /// </summary>
    [HttpPut("{id:int}/status")]
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
    /// Gets the status history of an order.
    /// </summary>
    [HttpGet("{id:int}/status-history")]
    [ProducesResponseType(
        typeof(IEnumerable<OrderStatusHistoryDto>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<OrderStatusHistoryDto>>>
        GetStatusHistory(int id)
    {
        var order = await _orderService.GetByIdAsync(id);

        if (order == null)
            return NotFound();

        var history =
            await _orderStatusHistoryService.GetByOrderIdAsync(id);

        return Ok(history);
    }
}
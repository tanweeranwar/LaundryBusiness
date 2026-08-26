using System.Security.Claims;
using Laundry.API.DTOs.Customer;
using Laundry.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Laundry.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController : ControllerBase
{
    private const string StaffRoles = "Super Admin,Branch Admin,Employee";

    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    /// <summary>
    /// Creates a new customer. Customer self-registration is handled by /api/auth/register.
    /// This endpoint is restricted to staff to prevent arbitrary customer creation through
    /// an authenticated customer token.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = StaffRoles)]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerResponse>> Create(CreateCustomerRequest request)
    {
        var customer = await _customerService.CreateAsync(request);
        return Ok(customer);
    }

    /// <summary>
    /// Gets all customers. Customer accounts cannot enumerate other customers.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = StaffRoles)]
    [ProducesResponseType(typeof(IEnumerable<CustomerResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CustomerResponse>>> GetAll()
    {
        var customers = await _customerService.GetAllAsync();
        return Ok(customers);
    }

    /// <summary>
    /// Gets a customer by Id. Customers may only access their own profile.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerResponse>> GetById(Guid id)
    {
        if (!IsStaff() && !IsCurrentUser(id))
            return Forbid();

        var customer = await _customerService.GetByIdAsync(id);

        if (customer == null)
            return NotFound();

        return Ok(customer);
    }

    /// <summary>
    /// Updates a customer. Customers may only update their own profile.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateCustomerRequest request)
    {
        if (!IsStaff() && !IsCurrentUser(id))
            return Forbid();

        var updated = await _customerService.UpdateAsync(id, request);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Deletes a customer. Customers may only delete their own profile.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!IsStaff() && !IsCurrentUser(id))
            return Forbid();

        var deleted = await _customerService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    private bool IsStaff()
    {
        return User.IsInRole("Super Admin") ||
               User.IsInRole("Branch Admin") ||
               User.IsInRole("Employee");
    }

    private bool IsCurrentUser(Guid customerId)
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("sub");

        return Guid.TryParse(claim, out var userId) &&
               userId == customerId;
    }
}
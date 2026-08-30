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
    private readonly IBranchAuthorizationService _branchAuthorization;

    public CustomersController(
        ICustomerService customerService,
        IBranchAuthorizationService branchAuthorization)
    {
        _customerService = customerService;
        _branchAuthorization = branchAuthorization;
    }

    [HttpPost]
    [Authorize(Roles = StaffRoles)]
    public async Task<ActionResult<CustomerResponse>> Create(CreateCustomerRequest request)
    {
        if (_branchAuthorization.IsBranchScopedStaff)
        {
            if (!_branchAuthorization.CurrentBranchId.HasValue)
                return Forbid();

            request.BranchId = _branchAuthorization.CurrentBranchId.Value;
        }

        var customer = await _customerService.CreateAsync(request);
        return Ok(customer);
    }

    [HttpGet]
    [Authorize(Roles = StaffRoles)]
    public async Task<ActionResult<IEnumerable<CustomerResponse>>> GetAll()
    {
        var customers = await _customerService.GetAllAsync();

        if (_branchAuthorization.IsSuperAdmin)
            return Ok(customers);

        var scoped = new List<CustomerResponse>();
        foreach (var customer in customers)
        {
            if (await _branchAuthorization.CanAccessCustomerAsync(customer.Id))
                scoped.Add(customer);
        }

        return Ok(scoped);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerResponse>> GetById(Guid id)
    {
        if (User.IsInRole("Customer"))
        {
            if (!IsCurrentUser(id))
                return Forbid();
        }
        else if (!await _branchAuthorization.CanAccessCustomerAsync(id))
        {
            return Forbid();
        }

        var customer = await _customerService.GetByIdAsync(id);

        if (customer == null)
            return NotFound();

        return Ok(customer);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateCustomerRequest request)
    {
        if (User.IsInRole("Customer"))
        {
            if (!IsCurrentUser(id))
                return Forbid();
        }
        else if (!await _branchAuthorization.CanAccessCustomerAsync(id))
        {
            return Forbid();
        }

        var updated = await _customerService.UpdateAsync(id, request);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (User.IsInRole("Customer"))
        {
            if (!IsCurrentUser(id))
                return Forbid();
        }
        else if (!await _branchAuthorization.CanAccessCustomerAsync(id))
        {
            return Forbid();
        }

        var deleted = await _customerService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    private bool IsCurrentUser(Guid customerId)
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("sub");

        return Guid.TryParse(claim, out var userId) &&
               userId == customerId;
    }
}
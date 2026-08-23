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
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    /// <summary>
    /// Creates a new customer.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(
        typeof(CustomerResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerResponse>> Create(
        CreateCustomerRequest request)
    {
        var customer =
            await _customerService.CreateAsync(request);

        return Ok(customer);
    }

    /// <summary>
    /// Gets all customers.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(
        typeof(IEnumerable<CustomerResponse>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CustomerResponse>>> GetAll()
    {
        var customers =
            await _customerService.GetAllAsync();

        return Ok(customers);
    }

    /// <summary>
    /// Gets a customer by Id.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(
        typeof(CustomerResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerResponse>> GetById(Guid id)
    {
        var customer =
            await _customerService.GetByIdAsync(id);

        if (customer == null)
            return NotFound();

        return Ok(customer);
    }

    /// <summary>
    /// Updates an existing customer.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateCustomerRequest request)
    {
        var updated =
            await _customerService.UpdateAsync(id, request);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Deletes an existing customer.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted =
            await _customerService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
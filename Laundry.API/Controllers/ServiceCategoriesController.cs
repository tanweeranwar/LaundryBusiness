using Laundry.API.DTOs.ServiceCategory;
using Laundry.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Laundry.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ServiceCategoriesController : ControllerBase
{
    private readonly IServiceCategoryService _service;

    public ServiceCategoriesController(IServiceCategoryService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceCategoryResponse>>> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ServiceCategoryResponse>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Super Admin")]
    public async Task<ActionResult<ServiceCategoryResponse>> Create(CreateServiceCategoryRequest request)
    {
        var result = await _service.CreateAsync(request);

        return CreatedAtAction(nameof(GetById),
            new { id = result.Id },
            result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Super Admin")]
    public async Task<IActionResult> Update(
        int id,
        CreateServiceCategoryRequest request)
    {
        var updated = await _service.UpdateAsync(id, request);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Super Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
using Laundry.API.DTOs.GarmentType;
using Laundry.API.Services;
using Laundry.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Laundry.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GarmentTypesController : ControllerBase
{
    private readonly IGarmentTypeService _service;

    public GarmentTypesController(IGarmentTypeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GarmentTypeResponse>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GarmentTypeResponse>> GetById(int id)
    {
        var garmentType = await _service.GetByIdAsync(id);

        if (garmentType == null)
            return NotFound();

        return Ok(garmentType);
    }

    [HttpPost]
    public async Task<ActionResult<GarmentTypeResponse>> Create(CreateGarmentTypeRequest request)
    {
        var garmentType = await _service.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = garmentType.Id },
            garmentType);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        CreateGarmentTypeRequest request)
    {
        var updated = await _service.UpdateAsync(id, request);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
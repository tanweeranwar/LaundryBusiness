using Laundry.API.DTOs.BranchPricing;
using Laundry.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Laundry.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BranchPricingController : ControllerBase
{
    private readonly IBranchPricingService _service;

    public BranchPricingController(IBranchPricingService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("branch/{branchId:int}")]
    public async Task<IActionResult> GetByBranch(int branchId)
    {
        var result = await _service.GetByBranchAsync(branchId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBranchPricingDto dto)
    {
        var result = await _service.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateBranchPricingDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);

        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);

        return NoContent();
    }
}
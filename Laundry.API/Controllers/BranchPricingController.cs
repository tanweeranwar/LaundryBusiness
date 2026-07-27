using Laundry.API.DTOs.BranchPricing;
using Laundry.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Laundry.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BranchPricingController : ControllerBase
{
    private readonly IBranchPricingService _service;

    public BranchPricingController(IBranchPricingService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BranchPricingDto>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BranchPricingDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("branch/{branchId:int}")]
    public async Task<ActionResult<IEnumerable<BranchPricingDto>>> GetByBranch(int branchId)
    {
        return Ok(await _service.GetByBranchAsync(branchId));
    }

    [HttpPost]
    public async Task<ActionResult<BranchPricingDto>> Create(CreateBranchPricingDto request)
    {
        var result = await _service.CreateAsync(request);

        return CreatedAtAction(nameof(GetById),
            new { id = result.Id },
            result);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<BranchPricingDto>> Update(
        int id,
        UpdateBranchPricingDto request)
    {
        var result = await _service.UpdateAsync(id, request);

        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);

        return NoContent();
    }
}
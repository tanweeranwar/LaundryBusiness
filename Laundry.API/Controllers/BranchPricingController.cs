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
    private readonly IBranchAuthorizationService _branchAuthorization;

    public BranchPricingController(
        IBranchPricingService service,
        IBranchAuthorizationService branchAuthorization)
    {
        _service = service;
        _branchAuthorization = branchAuthorization;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BranchPricingDto>>> GetAll()
    {
        var pricing = await _service.GetAllAsync();

        if (_branchAuthorization.IsSuperAdmin ||
            !_branchAuthorization.IsBranchScopedStaff)
            return Ok(pricing);

        return Ok(pricing.Where(x =>
            x.BranchId == _branchAuthorization.CurrentBranchId));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BranchPricingDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);

        if (result == null)
            return NotFound();

        if (_branchAuthorization.IsBranchScopedStaff &&
            !_branchAuthorization.CanAccessBranch(result.BranchId))
            return Forbid();

        return Ok(result);
    }

    [HttpGet("branch/{branchId:int}")]
    public async Task<ActionResult<IEnumerable<BranchPricingDto>>> GetByBranch(int branchId)
    {
        if (_branchAuthorization.IsBranchScopedStaff &&
            !_branchAuthorization.CanAccessBranch(branchId))
            return Forbid();

        return Ok(await _service.GetByBranchAsync(branchId));
    }

    [HttpPost]
    [Authorize(Roles = "Super Admin,Branch Admin")]
    public async Task<ActionResult<BranchPricingDto>> Create(CreateBranchPricingDto request)
    {
        if (!_branchAuthorization.CanAccessBranch(request.BranchId))
            return Forbid();

        var result = await _service.CreateAsync(request);

        return CreatedAtAction(nameof(GetById),
            new { id = result.Id },
            result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Super Admin,Branch Admin")]
    public async Task<ActionResult<BranchPricingDto>> Update(
        int id,
        UpdateBranchPricingDto request)
    {
        var existing = await _service.GetByIdAsync(id);

        if (existing == null)
            return NotFound();

        if (!_branchAuthorization.CanAccessBranch(existing.BranchId) ||
            !_branchAuthorization.CanAccessBranch(request.BranchId))
            return Forbid();

        var result = await _service.UpdateAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Super Admin,Branch Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _service.GetByIdAsync(id);

        if (existing == null)
            return NotFound();

        if (!_branchAuthorization.CanAccessBranch(existing.BranchId))
            return Forbid();

        await _service.DeleteAsync(id);
        return NoContent();
    }
}
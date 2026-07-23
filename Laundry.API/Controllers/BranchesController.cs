using Laundry.API.DTOs.Branch;
using Laundry.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Laundry.API.Controllers;

////[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BranchesController : ControllerBase
{
    private readonly IBranchService _branchService;

    public BranchesController(IBranchService branchService)
    {
        _branchService = branchService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var branches = await _branchService.GetAllAsync();
        return Ok(branches);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var branch = await _branchService.GetByIdAsync(id);

        if (branch == null)
            return NotFound();

        return Ok(branch);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBranchRequest request)
    {
        var branch = await _branchService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = branch.Id },
            branch);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CreateBranchRequest request)
    {
        var updated = await _branchService.UpdateAsync(id, request);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _branchService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
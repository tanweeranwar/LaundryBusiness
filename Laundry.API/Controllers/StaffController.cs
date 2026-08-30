using BCrypt.Net;
using Laundry.API.Data;
using Laundry.API.DTOs.Staff;
using Laundry.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Laundry.API.Controllers;

[Authorize(Roles = "Super Admin")]
[ApiController]
[Route("api/[controller]")]
public class StaffController : ControllerBase
{
    private static readonly HashSet<string> AllowedRoles =
        new(StringComparer.Ordinal)
        {
            "Super Admin",
            "Branch Admin",
            "Employee",
            "Delivery Agent"
        };

    private readonly LaundryDbContext _context;

    public StaffController(LaundryDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StaffResponse>>> GetAll()
    {
        var staff = await _context.Customers
            .AsNoTracking()
            .Where(x => x.Role != "Customer")
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Select(x => new StaffResponse
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                MobileNumber = x.MobileNumber,
                Email = x.Email,
                Role = x.Role,
                BranchId = x.BranchId
            })
            .ToListAsync();

        return Ok(staff);
    }

    [HttpPost]
    public async Task<ActionResult<StaffResponse>> Create(CreateStaffRequest request)
    {
        if (!AllowedRoles.Contains(request.Role))
            return BadRequest("Invalid staff role.");

        if (request.Role != "Super Admin" && !request.BranchId.HasValue)
            return BadRequest("BranchId is required for branch-scoped staff.");

        if (request.Role == "Super Admin")
            request.BranchId = null;

        if (request.BranchId.HasValue &&
            !await _context.Branches.AnyAsync(x => x.Id == request.BranchId.Value))
            return BadRequest("Assigned branch does not exist.");

        var exists = await _context.Customers.AnyAsync(x =>
            x.MobileNumber == request.MobileNumber ||
            x.Email == request.Email);

        if (exists)
            return BadRequest("A user with this mobile number or email already exists.");

        var staff = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            MobileNumber = request.MobileNumber.Trim(),
            Email = request.Email.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            BranchId = request.BranchId,
            CreatedOn = DateTime.UtcNow
        };

        _context.Customers.Add(staff);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetAll),
            null,
            new StaffResponse
            {
                Id = staff.Id,
                FirstName = staff.FirstName,
                LastName = staff.LastName,
                MobileNumber = staff.MobileNumber,
                Email = staff.Email,
                Role = staff.Role,
                BranchId = staff.BranchId
            });
    }
}
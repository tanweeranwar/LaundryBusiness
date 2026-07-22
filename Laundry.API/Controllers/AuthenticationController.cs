using BCrypt.Net;
using Laundry.API.Data;
using Laundry.API.DTOs.Authentication;
using Laundry.API.Entities;
using Laundry.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Laundry.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly LaundryDbContext _context;

    public AuthenticationController(LaundryDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var existingCustomer = await _context.Customers
            .FirstOrDefaultAsync(c =>
                c.MobileNumber == request.MobileNumber ||
                c.Email == request.Email);

        if (existingCustomer != null)
        {
            return BadRequest("Customer already exists.");
        }

        var customer = new Customer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            MobileNumber = request.MobileNumber,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "Customer"
        };

        _context.Customers.Add(customer);

        await _context.SaveChangesAsync();

        return Ok("Customer registered successfully.");
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginRequest request,
        [FromServices] IJwtService jwtService)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.MobileNumber == request.MobileNumber);

        if (customer == null)
            return Unauthorized("Invalid mobile number or password.");

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(
            request.Password,
            customer.PasswordHash);

        if (!isPasswordValid)
            return Unauthorized("Invalid mobile number or password.");

        var token = jwtService.GenerateToken(
            customer.Id,
            customer.MobileNumber,
            customer.Role);

        return Ok(new LoginResponse
        {
            Token = token,
            Expiry = DateTime.UtcNow.AddMinutes(60)
        });
    }
}
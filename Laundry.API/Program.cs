using System.Text;
using Laundry.API.Data;
using Laundry.API.Interfaces;
using Laundry.API.Middleware;
using Laundry.API.Repositories;
using Laundry.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<LaundryDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("LaundryDb")));

builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddScoped<IBranchRepository, BranchRepository>();

builder.Services.AddScoped<IBranchService, BranchService>();

builder.Services.AddScoped<IBranchPricingRepository, BranchPricingRepository>();

builder.Services.AddScoped<IBranchPricingService, BranchPricingService>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddScoped<IPricingService, PricingService>();

builder.Services.AddScoped<IOrderNumberGenerator, OrderNumberGenerator>();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<IGarmentTypeRepository, GarmentTypeRepository>();

builder.Services.AddScoped<IGarmentTypeService, GarmentTypeService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
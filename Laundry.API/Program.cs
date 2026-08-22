using System.Text;
using Laundry.API.Data;
using Laundry.API.Interfaces;
using Laundry.API.Middleware;
using Laundry.API.Repositories;
using Laundry.API.Repositories.Interfaces;
using Laundry.API.Services;
using Laundry.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

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
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<LaundryDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("LaundryDb")));

// Authentication
builder.Services.AddScoped<IJwtService, JwtService>();

// Branch
builder.Services.AddScoped<IBranchRepository, BranchRepository>();
builder.Services.AddScoped<IBranchService, BranchService>();

// Branch Pricing
builder.Services.AddScoped<IBranchPricingRepository, BranchPricingRepository>();
builder.Services.AddScoped<IBranchPricingService, BranchPricingService>();

// Customer
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

// Garment Type
builder.Services.AddScoped<IGarmentTypeRepository, GarmentTypeRepository>();
builder.Services.AddScoped<IGarmentTypeService, GarmentTypeService>();

// Order
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPricingService, PricingService>();
builder.Services.AddScoped<IOrderNumberGenerator, OrderNumberGenerator>();

// Payment
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPaymentNumberGenerator, PaymentNumberGenerator>();

// Pickup
builder.Services.AddScoped<IPickupRepository, PickupRepository>();
builder.Services.AddScoped<IPickupService, PickupService>();

// Delivery
builder.Services.AddScoped<IDeliveryRepository, DeliveryRepository>();
builder.Services.AddScoped<IDeliveryService, DeliveryService>();

builder.Services.AddScoped<
    IOrderStatusHistoryRepository,
    OrderStatusHistoryRepository>();

builder.Services.AddScoped<
    IOrderStatusHistoryService,
    OrderStatusHistoryService>();

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
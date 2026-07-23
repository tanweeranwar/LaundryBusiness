using System.Text.Json;
using Laundry.API.Common.Exceptions;
using Laundry.API.Common.Responses;

namespace Laundry.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DuplicateBranchCodeException ex)
        {
            _logger.LogWarning(ex, ex.Message);

            context.Response.StatusCode = StatusCodes.Status409Conflict;

            context.Response.ContentType = "application/json";

            var response = ApiResponse<string>.FailureResponse(ex.Message);

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            context.Response.ContentType = "application/json";

            var response =
                ApiResponse<string>.FailureResponse(
                    "An unexpected error occurred.");

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}
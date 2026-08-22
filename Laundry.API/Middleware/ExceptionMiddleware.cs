using System.Text.Json;
using Laundry.API.Common.Exceptions;
using Laundry.API.Common.Responses;
using Laundry.API.Exceptions;

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
            await WriteErrorResponse(
                context,
                StatusCodes.Status409Conflict,
                ex);
        }
        catch (InvalidOrderStatusTransitionException ex)
        {
            await WriteErrorResponse(
                context,
                StatusCodes.Status400BadRequest,
                ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            await WriteErrorResponse(
                context,
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.");
        }
    }

    private async Task WriteErrorResponse(
        HttpContext context,
        int statusCode,
        Exception exception)
    {
        _logger.LogWarning(exception, exception.Message);

        await WriteErrorResponse(
            context,
            statusCode,
            exception.Message);
    }

    private static async Task WriteErrorResponse(
        HttpContext context,
        int statusCode,
        string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response =
            ApiResponse<string>.FailureResponse(message);

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}
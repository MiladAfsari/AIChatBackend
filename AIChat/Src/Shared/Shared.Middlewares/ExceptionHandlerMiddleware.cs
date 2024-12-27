using Microsoft.AspNetCore.Http;
using Serilog;
using Shared.Exception.Abstraction.Domain;
using Shared.Exception.Abstraction.Infrastructure;
using Shared.Middlewares.Models;
using System.Diagnostics;
using System.Text.Json;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            await HandleExceptionAsync(httpContext, ex, stopwatch.ElapsedMilliseconds);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, long elapsedMilliseconds)
    {
        context.Response.ContentType = "application/json";

        var statusCode = exception switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedException => StatusCodes.Status401Unauthorized,
            ForbiddenException => StatusCodes.Status403Forbidden,
            ValidationException or ModelStateValidationException => StatusCodes.Status422UnprocessableEntity,
            InfrastructureDbOperationException => StatusCodes.Status500InternalServerError,
            TaskCanceledException => StatusCodes.Status504GatewayTimeout,
            _ => StatusCodes.Status500InternalServerError
        };

        context.Response.StatusCode = statusCode;

        var errorResponse = new ErrorResponse
        {
            Type = exception.GetType().Name.Replace("Exception", string.Empty),
            Message = exception.Message,
            ElapsedMilliseconds = elapsedMilliseconds
        };

        // Log to Serilog (console, file, database, etc.)
        Log.Error(exception, "An error occurred: {Message}", exception.Message);

        var result = JsonSerializer.Serialize(errorResponse);
        await context.Response.WriteAsync(result);
    }
}
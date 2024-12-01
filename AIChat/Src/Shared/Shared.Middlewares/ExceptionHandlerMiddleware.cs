using Domain.Core.Exception;
using Domain.Core.UnitOfWorkContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
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

    public async Task InvokeAsync(HttpContext httpContext, IServiceProvider serviceProvider)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            var exceptionLogRepository = serviceProvider.GetRequiredService<IExceptionLogRepository>();
            var applicationDbContextUnitOfWork = serviceProvider.GetRequiredService<IApplicationDbContextUnitOfWork>();
            await HandleExceptionAsync(httpContext, ex, stopwatch.ElapsedMilliseconds, exceptionLogRepository, applicationDbContextUnitOfWork);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, long elapsedMilliseconds, IExceptionLogRepository exceptionLogRepository, IApplicationDbContextUnitOfWork applicationDbContextUnitOfWork)
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

        var errorResponse = new
        {
            Type = exception.GetType().Name.Replace("Exception", string.Empty),
            Message = exception.Message,
            ElapsedMilliseconds = elapsedMilliseconds
        };

        await LogExceptionAsync(context, exception, statusCode, elapsedMilliseconds, exceptionLogRepository, applicationDbContextUnitOfWork);

        var result = JsonSerializer.Serialize(errorResponse);
        await context.Response.WriteAsync(result);
    }

    private async Task LogExceptionAsync(HttpContext context, Exception exception, int statusCode, long elapsedMilliseconds, IExceptionLogRepository exceptionLogRepository, IApplicationDbContextUnitOfWork applicationDbContextUnitOfWork)
    {
        var exceptionLog = new ExceptionLog
        {
            ExceptionType = exception.GetType().Name,
            Message = exception.Message,
            StackTrace = exception.StackTrace ?? string.Empty,
            InnerException = exception.InnerException?.Message,
            Path = context.Request.Path,
            QueryString = context.Request.QueryString.ToString(),
            StatusCode = statusCode,
            ElapsedMilliseconds = elapsedMilliseconds,
            Timestamp = DateTime.UtcNow
        };

        await exceptionLogRepository.LogExceptionAsync(exceptionLog);
        await applicationDbContextUnitOfWork.SaveChangesAsync();
    }
}

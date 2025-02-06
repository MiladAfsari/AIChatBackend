using Application.Command.ApiLogCommands;
using MediatR;
using Microsoft.IO;
using Service.Rest.Attributes;
using Service.Rest.Extensions;

namespace Service.Rest.Middlewares;

public class LogRequestResponseMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;
    private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager = new();

    private RequestResponseData RequestResponseData { get; set; }

    public async Task Invoke(HttpContext context, IMediator mediator)
    {
        if (!context.HasLogAttribute(out var logAttribute))
        {
            await _next(context);
            return;
        }

        LogRequest(context);
        await LogResponse(context);
        SetResponseTime();
        await CreateApiLog(context, mediator);
    }

    private void LogRequest(HttpContext context)
    {
        RequestResponseData = new RequestResponseData
        {
            ControllerName = context.GetEndpoint()!.ToString()!.Split()[0],
            RequestTimestamp = DateTime.UtcNow,
            IP = context.GetClientIP(),
            Authorization = context.Request.Headers.Authorization!,
            RequestHeaders = string.Join(",", context.Request.Headers)
        };
    }

    private async Task LogResponse(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;
        await using var responseBody = _recyclableMemoryStreamManager.GetStream();
        context.Response.Body = responseBody;

        await _next(context);

        RequestResponseData.RequestBody = context.Items[AttributeConstants.LOG_REQUEST_BODY] != null ? context.Items[AttributeConstants.LOG_REQUEST_BODY]!.ToString()! : string.Empty;
        RequestResponseData.ResponseTimestamp = DateTime.UtcNow;
        RequestResponseData.ResponseBody = await context.Response.ReadAsStringAsync();
        RequestResponseData.ResponseStatusCode = context.Response.StatusCode;

        await responseBody.CopyToAsync(originalBodyStream);
    }

    private void SetResponseTime()
    {
        TimeSpan differentTimespan = RequestResponseData.ResponseTimestamp - RequestResponseData.RequestTimestamp;
        RequestResponseData.ResponseTime = (int)differentTimespan.TotalMilliseconds;
    }

    private async Task CreateApiLog(HttpContext context, IMediator mediator)
    {
        var createCommand = new CreateApiLogCommand(RequestResponseData.ControllerName, RequestResponseData.RequestHeaders, RequestResponseData.Authorization, RequestResponseData.RequestBody, RequestResponseData.ResponseBody, RequestResponseData.ResponseStatusCode, "", DateTime.UtcNow, context.GetClientIP(), RequestResponseData.ResponseTime);
        await mediator!.Send(createCommand);
    }
}

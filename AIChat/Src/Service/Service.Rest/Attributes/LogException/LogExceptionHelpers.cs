using Application.Command.ErrorLogCommands;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Service.Rest.Attributes.LogException
{
    public class LogExceptionHelpers
    {
        public static void CreateErrorLog(ActionExecutedContext context)
        {
            var mediator = context.HttpContext.RequestServices.GetService<IMediator>();
            string action = context.ActionDescriptor.RouteValues["action"]!;
            string controllerName = $"{context.Controller}.{action}";
            string exception = $"{context.Exception!.Message}";

            var createCommand = new CreateErrorLogCommand(controllerName, action, exception, DateTime.UtcNow);
            _ = mediator!.Send(createCommand).Result;
        }
    }
}

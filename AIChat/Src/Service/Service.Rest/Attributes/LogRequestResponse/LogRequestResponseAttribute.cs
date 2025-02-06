using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Service.Rest.Attributes.LogRequestResponse
{
    public class LogRequestResponseAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor)
            {
                var requestBody = LogRequestResponseHelpers.FormatRequestBody(context.ActionArguments);
                context.HttpContext.Items[AttributeConstants.LOG_REQUEST_BODY] = requestBody;
            }
        }
    }
}

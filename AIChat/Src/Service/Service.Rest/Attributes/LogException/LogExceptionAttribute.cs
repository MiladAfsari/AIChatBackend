using Microsoft.AspNetCore.Mvc.Filters;

namespace Service.Rest.Attributes.LogException
{
    public class LogExceptionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                LogExceptionHelpers.CreateErrorLog(context);
            }
        }
    }
}

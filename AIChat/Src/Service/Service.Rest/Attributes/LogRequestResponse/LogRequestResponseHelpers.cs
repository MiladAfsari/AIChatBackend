using Newtonsoft.Json;

namespace Service.Rest.Attributes.LogRequestResponse
{
    public static class LogRequestResponseHelpers
    {
        public static string FormatRequestBody(IDictionary<string, object?> actionArguments)
        {
            return actionArguments != null ? JsonConvert.SerializeObject(actionArguments) : string.Empty;
        }
    }
}

using Service.Rest.Attributes.LogRequestResponse;

namespace Service.Rest.Extensions;

public static class HttpContextExtension
{
    public static bool HasLogAttribute(this HttpContext context, out LogRequestResponseAttribute? logRequestResponseAttribute)
    {
        logRequestResponseAttribute = context.GetEndpoint()?.Metadata.GetMetadata<LogRequestResponseAttribute>();
        return logRequestResponseAttribute is not null;
    }

    public static string GetKey(this HttpContext context, string ip)
    {
        string projectName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
        string path = $"{projectName}{context.Request.Path.ToString().Replace("/", "-")}";
        return $"{path}-{ip}";
    }

    public static string GetClientIP(this HttpContext context)
    {
        try
        {
            var request = context.Request;
            if (request == null)
            {
                return string.Empty;
            }

            string ip = string.Empty;
            if (request.Headers.ContainsKey("True-Client-IP"))
            {
                return request.Headers.FirstOrDefault(x => x.Key == "True-Client-IP").Value.FirstOrDefault()!;
            }
            if (request.Headers.ContainsKey("X-Real-IP"))
            {
                ip = request.Headers.FirstOrDefault(x => x.Key == "X-Real-IP").Value.FirstOrDefault()!;
            }
            if (request.Headers.ContainsKey("X-Forwarded-For") && string.IsNullOrEmpty(ip))
            {
                ip = request.Headers.FirstOrDefault(x => x.Key == "X-Forwarded-For").Value.FirstOrDefault()!;
            }
            if (request.Headers.ContainsKey("ar-real-ip") && string.IsNullOrEmpty(ip))
            {
                ip = request.Headers.FirstOrDefault(x => x.Key == "ar-real-ip").Value.FirstOrDefault()!;
            }
            if (string.IsNullOrEmpty(ip))
            {
                ip = request.HttpContext.Connection.RemoteIpAddress!.ToString();
            }
            return ip;
        }
        catch
        {
            return string.Empty;
        }
    }
}

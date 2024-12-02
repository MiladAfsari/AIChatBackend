namespace Shared.Middlewares.Models
{
    public class ErrorResponse
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public long ElapsedMilliseconds { get; set; }
    }
}

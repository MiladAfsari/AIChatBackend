namespace Domain.Core.Exception
{
    public class ExceptionLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string ExceptionType { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string StackTrace { get; set; } = string.Empty;
        public string? InnerException { get; set; }
        public string Path { get; set; } = string.Empty;
        public string QueryString { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public long ElapsedMilliseconds { get; set; }
    }
}

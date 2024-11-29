namespace Domain.Core.Exception
{
    public interface IExceptionLogRepository
    {
        Task LogExceptionAsync(ExceptionLog logEntry);
    }
}

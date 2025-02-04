namespace Domain.Core.Error
{
    public interface IErrorLogRepository
    {
        Task CreateErrorLog(ErrorLog apiLog, CancellationToken cancellationToken);
    }
}

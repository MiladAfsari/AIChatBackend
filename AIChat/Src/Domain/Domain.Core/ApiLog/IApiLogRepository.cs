namespace Domain.Core.ApiLog
{
    public interface IApiLogRepository
    {
        Task CreateApiLog(ApiLog apiLog, CancellationToken cancellationToken);
    }
}

using Domain.Core.ApiLog;
using Infrastructure.Data.Repository.EfCore.DatabaseContexts;

namespace Infrastructure.Data.Repository.EfCore.Repositories
{
    public class ApiLogRepository : IApiLogRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ApiLogRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task CreateApiLog(ApiLog apiLog, CancellationToken cancellationToken)
        {
            if (apiLog == null) throw new ArgumentNullException(nameof(apiLog));

            await _dbContext.ApiLogs.AddAsync(apiLog, cancellationToken);
        }
    }
}

using Domain.Core.Error;
using Infrastructure.Data.Repository.EfCore.DatabaseContexts;

namespace Infrastructure.Data.Repository.EfCore.Repositories
{
    public class ErrorLogRepository : IErrorLogRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ErrorLogRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateErrorLog(ErrorLog errorLog, CancellationToken cancellationToken)
        {
            if (errorLog == null) throw new ArgumentNullException(nameof(errorLog));

            await _dbContext.ErrorLogs.AddAsync(errorLog, cancellationToken);
        }
    }
}

using Domain.Core.Exception;
using Infrastructure.Data.Repository.EfCore.DatabaseContexts;

namespace Infrastructure.Data.Repository.EfCore.Repositories
{
    public class ExceptionLogRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ExceptionLogRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task LogExceptionAsync(ExceptionLog logEntry)
        {
            await _dbContext.ExceptionLogs.AddAsync(logEntry);
        }
    }
}

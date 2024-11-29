using Domain.Core.UnitOfWorkContracts;

namespace Infrastructure.Data.Repository.EfCore.DatabaseContexts
{
    public class ApplicationDbContextUnitOfWork : IApplicationDbContextUnitOfWork
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public ApplicationDbContextUnitOfWork(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _applicationDbContext.SaveChangesAsync();
        }
    }
}

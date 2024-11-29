namespace Domain.Core.UnitOfWorkContracts
{
    public interface IApplicationDbContextUnitOfWork
    {
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

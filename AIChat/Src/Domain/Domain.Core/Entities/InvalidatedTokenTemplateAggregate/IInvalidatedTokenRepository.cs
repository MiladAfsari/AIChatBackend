namespace Domain.Core.Entities.InvalidatedTokenTemplateAggregate
{
    public interface IInvalidatedTokenRepository
    {
        void InvalidateToken(string token);
        string GetInvalidatedToken(string token);
    }
}

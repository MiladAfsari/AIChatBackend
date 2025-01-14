namespace Domain.Core.Entities.InvalidatedTokenTemplateAggregate
{
    public interface IInvalidatedTokenRepository
    {
        void InvalidateToken(string token, Guid userId);
        InvalidatedToken GetInvalidatedToken(string token);
        bool IsTokenInvalidated(string token);
    }
}

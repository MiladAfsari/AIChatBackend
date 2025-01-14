namespace Domain.Core.Entities.UserTokenTemplateAggregate
{
    public interface IUserTokenRepository
    {
        Task AddTokenAsync(UserToken token);
        Task<UserToken> GetTokenAsync(string token);
        Task RemoveTokenAsync(string token);
    }
}

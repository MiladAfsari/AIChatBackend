using Domain.Core.Entities.UserTemplateAggregate;

namespace Application.Service.Common
{
    public interface ITokenService
    {
        Task<string> GenerateTokenAsync(ApplicationUser user);
        Task InvalidateTokenAsync(string token, Guid userId);
        Task<bool> IsTokenValidAsync(string token);
        Task<string> GetTokenFromRequestAsync();
        Task<Guid?> GetUserIdFromTokenAsync(string token);
    }
}

using Domain.Core.Entities.UserTemplateAggregate;

namespace Application.Service.Common
{
    public interface ITokenService
    {
        string GenerateToken(ApplicationUser user);
        void InvalidateToken(string token, Guid userId);
        bool IsTokenValid(string token);
        string GetTokenFromRequest();
    }
}

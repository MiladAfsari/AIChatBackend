using Domain.Core.Entities.UserTemplateAggregate;

namespace Application.Service.Common
{
    public interface ITokenService
    {
        string GenerateToken(ApplicationUser user);
    }
}

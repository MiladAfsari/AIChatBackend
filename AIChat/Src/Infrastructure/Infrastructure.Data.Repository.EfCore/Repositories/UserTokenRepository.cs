using Domain.Core.Entities.UserTokenTemplateAggregate;
using Infrastructure.Data.Repository.EfCore.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repository.EfCore.Repositories
{
    public class UserTokenRepository : IUserTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public UserTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddTokenAsync(UserToken token)
        {
            await _context.UserTokens.AddAsync(token);
        }

        public async Task<UserToken> GetTokenAsync(string token)
        {
            return await _context.UserTokens
                .FirstOrDefaultAsync(t => t.Token == token);
        }

        public async Task RemoveTokenAsync(string token)
        {
            var userToken = await _context.UserTokens
                .FirstOrDefaultAsync(t => t.Token == token);
            if (userToken != null)
            {
                _context.UserTokens.Remove(userToken);
            }
        }
    }
}

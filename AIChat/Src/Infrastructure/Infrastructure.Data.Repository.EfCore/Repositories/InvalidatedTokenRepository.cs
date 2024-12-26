using Domain.Core.Entities.InvalidatedTokenTemplateAggregate;
using Infrastructure.Data.Repository.EfCore.DatabaseContexts;

namespace Infrastructure.Data.Repository.EfCore.Repositories
{
    public class InvalidatedTokenRepository : IInvalidatedTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public InvalidatedTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GetInvalidatedToken(string token)
        {
            var invalidatedToken = _context.InvalidatedTokens
                .FirstOrDefault(t => t.Token == token);
            return invalidatedToken?.Token;
        }

        public void InvalidateToken(string token, string username)
        {
            var invalidatedToken = new InvalidatedToken
            {
                Token = token,
                InvalidatedAt = DateTime.UtcNow,
                Username = username
            };
            _context.InvalidatedTokens.Add(invalidatedToken);
        }
    }
}

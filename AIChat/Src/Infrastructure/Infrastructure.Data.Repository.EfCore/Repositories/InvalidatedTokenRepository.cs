using Domain.Core.Entities.InvalidatedTokenTemplateAggregate;
using Infrastructure.Data.Repository.EfCore.DatabaseContexts;

namespace Infrastructure.Data.Repository.EfCore.Repositories
{
    public class InvalidatedTokenRepository : IInvalidatedTokenRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public InvalidatedTokenRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void InvalidateToken(string token, Guid userId)
        {
            if (!IsTokenInvalidated(token))
            {
                var invalidatedToken = new InvalidatedToken
                {
                    Id = Guid.CreateVersion7(),
                    ApplicationUserId = userId,
                    Token = token,
                    InvalidatedAt = DateTime.UtcNow
                };

                _dbContext.Set<InvalidatedToken>().Add(invalidatedToken);
                _dbContext.SaveChanges();
            }
        }

        public InvalidatedToken GetInvalidatedToken(string token)
        {
            return _dbContext.Set<InvalidatedToken>().FirstOrDefault(t => t.Token == token);
        }

        public bool IsTokenInvalidated(string token)
        {
            return _dbContext.Set<InvalidatedToken>().Any(t => t.Token == token);
        }
    }
}

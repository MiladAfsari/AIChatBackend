using Domain.Base;
using Domain.Core.Entities.UserTemplateAggregate;

namespace Domain.Core.Entities.UserTokenTemplateAggregate
{
    public class UserToken 
    {
        public Guid Id { get; set; }
        public Guid ApplicationUserId { get; set; }
        public string Token { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}

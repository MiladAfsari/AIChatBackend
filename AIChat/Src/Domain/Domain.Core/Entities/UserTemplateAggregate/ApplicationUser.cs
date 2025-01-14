using Domain.Core.Entities.ChatSessionTemplateAggregate;
using Domain.Core.Entities.UserTokenTemplateAggregate;
using Microsoft.AspNetCore.Identity;

namespace Domain.Core.Entities.UserTemplateAggregate
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; set; }
        public short DepartmentId { get; set; }
        public ICollection<ChatSession> ChatSessions { get; set; }
        public ICollection<UserToken> UserTokens { get; set; } = new List<UserToken>();
    }
}

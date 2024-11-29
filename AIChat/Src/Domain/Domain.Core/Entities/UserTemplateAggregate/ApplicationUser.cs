using Domain.Core.Entities.ChatSessionTemplateAggregate;
using Microsoft.AspNetCore.Identity;

namespace Domain.Core.Entities.UserTemplateAggregate
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();
    }
}

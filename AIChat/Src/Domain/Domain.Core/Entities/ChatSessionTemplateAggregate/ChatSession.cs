using Domain.Base;
using Domain.Core.Entities.ChatMessageTemplateAggregate;
using Domain.Core.Entities.UserTemplateAggregate;

namespace Domain.Core.Entities.ChatSessionTemplateAggregate
{
    public class ChatSession : BaseEntity
    {
        public string SessionName { get; set; }
        public string Description { get; set; } // Optional metadata about the session
        public Guid ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
    }
}

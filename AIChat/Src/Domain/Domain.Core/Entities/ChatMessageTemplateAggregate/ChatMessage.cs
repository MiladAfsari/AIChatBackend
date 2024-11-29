using Domain.Base;
using Domain.Core.Entities.ChatSessionTemplateAggregate;
using Domain.Core.Entities.FeedbackTemplateAggregate;

namespace Domain.Core.Entities.ChatMessageTemplateAggregate
{
    public class ChatMessage : BaseEntity
    {
        public Guid ChatSessionId { get; set; }
        public ChatSession ChatSession { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public Feedback Feedback { get; set; }
    }
}

using Domain.Base;
using Domain.Core.Entities.ChatMessageTemplateAggregate;
using Domain.Core.Entities.UserTemplateAggregate;

namespace Domain.Core.Entities.FeedbackTemplateAggregate
{
    public class Feedback : BaseEntity
    {
        public Guid ChatMessageId { get; set; }
        public ChatMessage ChatMessage { get; set; }
        // Reference to the User who provided the feedback (direct relationship)
        public Guid ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public short Rating { get; set; } 
    }
}

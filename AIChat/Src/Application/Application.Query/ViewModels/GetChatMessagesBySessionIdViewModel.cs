namespace Application.Query.ViewModels
{
    using System;

    namespace Application.Query.ViewModels
    {
        public class GetChatMessagesBySessionIdViewModel
        {
            public Guid ChatSessionId { get; set; }
            public Guid ChatMessageId { get; set; }
            public string Question { get; set; }
            public string Answer { get; set; }
            public FeedbackViewModel Feedback { get; set; }
        }

        public class FeedbackViewModel
        {
            public short Rating { get; set; }
        }
    }
}

namespace Service.Rest.V1.RequestModels
{
    public class AddFeedbackModel
    {
        public Guid ChatMessageId { get; set; }
        public string ApplicationUserId { get; set; }
        public bool IsLiked { get; set; }
    }
}

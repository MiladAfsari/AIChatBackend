namespace Service.Rest.V1.RequestModels
{
    public class AddFeedbackModel
    {
        public Guid ChatMessageId { get; set; }
        public short Rating { get; set; }
    }
}

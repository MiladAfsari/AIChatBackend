namespace Service.Rest.V1.RequestModels
{
    public class AddChatMessageModel
    {
        public Guid ChatSessionId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}

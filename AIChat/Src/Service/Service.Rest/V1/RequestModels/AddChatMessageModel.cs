namespace Service.Rest.V1.RequestModels
{
    public class AddChatMessageModel
    {
        public Guid? ChatSessionId { get; set; }
        public string? Description { get; set; }
        public string SessionName { get; set; }
        public string Question { get; set; }
    }
}

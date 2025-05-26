namespace Service.Rest.V1.RequestModels
{
    public class UpdateChatSessionNameModel
    {
        public Guid chatSessionId { get; set; }
        public string NewSessionName { get; set; } = string.Empty;
    }
}

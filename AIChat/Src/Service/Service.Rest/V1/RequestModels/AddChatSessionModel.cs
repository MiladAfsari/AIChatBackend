namespace Service.Rest.V1.RequestModels
{
    public class AddChatSessionModel
    {
        public string SessionName { get; set; }
        public string Description { get; set; }
        public Guid ApplicationUserId { get; set; }
    }
}

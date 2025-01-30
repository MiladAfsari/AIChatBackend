namespace Application.Query.ViewModels
{
    public class GetChatSessionsByUserIdViewModel
    {
        public Guid Id { get; set; }
        public string SessionName { get; set; }
        public string Description { get; set; }
    }
}

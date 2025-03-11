namespace Application.Service.Common
{
    public interface IExternalChatBotService
    {
        Task<string> GetChatResponseAsync(string question);
    }
}

namespace Domain.Core.Entities.ChatMessageTemplateAggregate
{
    public interface IChatMessageRepository
    {
        Task<ChatMessage> GetByIdAsync(Guid id);
        Task<IEnumerable<ChatMessage>> GetAllAsync();
        Task<IEnumerable<ChatMessage>> GetBySessionIdAsync(Guid sessionId);

        // Return all chat messages of a specific user
        Task<IEnumerable<ChatMessage>> GetAllChatsByUserIdAsync(Guid userId);

        // Return chat messages of a specific session
        Task<IEnumerable<ChatMessage>> GetChatsBySessionIdAsync(Guid sessionId);

        Task AddAsync(ChatMessage chatMessage);
        Task UpdateAsync(ChatMessage chatMessage);
        Task DeleteAsync(Guid id);
    }
}

namespace Domain.Core.Entities.ChatMessageTemplateAggregate
{
    public interface IChatMessageRepository
    {
        Task<ChatMessage> GetByIdAsync(Guid id);
        Task<IEnumerable<ChatMessage>> GetAllAsync();
        Task<IEnumerable<ChatMessage>> GetAllChatsByUserIdAsync(Guid userId);
        Task<IQueryable<ChatMessage>> GetChatsWithFeedbackBySessionIdAsync(Guid sessionId);
        Task AddAsync(ChatMessage chatMessage);
        Task UpdateAsync(ChatMessage chatMessage);
        Task DeleteAsync(Guid id);
    }
}

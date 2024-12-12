namespace Domain.Core.Entities.ChatSessionTemplateAggregate
{
    public interface IChatSessionRepository
    {
        Task<ChatSession> GetByIdAsync(Guid id);
        Task<IEnumerable<ChatSession>> GetAllAsync();
        Task<IEnumerable<ChatSession>> GetByUserIdAsync(Guid userId);

        // Get a ChatSession along with its related messages based on sessionId
        Task<ChatSession> GetSessionWithMessagesByIdAsync(Guid sessionId);

        Task<Guid> AddAsync(ChatSession chatSession);
        Task UpdateAsync(ChatSession chatSession);
        Task DeleteAsync(Guid id);
    }
}

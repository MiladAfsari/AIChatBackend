using Domain.Core.Entities.ChatSessionTemplateAggregate;
using Infrastructure.Data.Repository.EfCore.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repository.EfCore.Repositories
{
    public class ChatSessionRepository : IChatSessionRepository
    {
        private readonly ApplicationDbContext _context;

        public ChatSessionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> AddAsync(ChatSession chatSession)
        {
            _context.ChatSessions.Add(chatSession);
            return chatSession.Id;
        }

        public async Task<IEnumerable<ChatSession>> GetAllAsync()
        {
            return await _context.ChatSessions.ToListAsync();
        }

        public async Task<ChatSession> GetByIdAsync(Guid id)
        {
            return await _context.ChatSessions.FindAsync(id);
        }

        public async Task<IEnumerable<ChatSession>> GetByUserIdAsync(Guid userId)
        {
            return await _context.ChatSessions
                .Where(cs => cs.ApplicationUserId == userId)
                .OrderBy(cs => cs.CreatedAt)
                .ToListAsync();
        }

        public async Task<ChatSession> GetSessionWithMessagesByIdAsync(Guid sessionId)
        {
            return await _context.ChatSessions
                .Include(cs => cs.ChatMessages)
                .FirstOrDefaultAsync(cs => cs.Id == sessionId);
        }

        public async Task UpdateAsync(ChatSession chatSession)
        {
            _context.ChatSessions.Update(chatSession);
        }
        public async Task DeleteChatSessionWithMessagesAsync(Guid sessionId)
        {
            var chatSession = await _context.ChatSessions
                .Where(cs => cs.Id == sessionId)
                .Select(cs => new { ChatSession = cs, ChatMessages = cs.ChatMessages })
                .FirstOrDefaultAsync();

            if (chatSession != null)
            {
                _context.ChatMessages.RemoveRange(chatSession.ChatMessages);
                _context.ChatSessions.Remove(chatSession.ChatSession);
            }
        }
    }
}

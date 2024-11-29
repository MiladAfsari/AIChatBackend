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

        public async Task<ChatSession> GetByIdAsync(Guid id)
        {
            return await _context.ChatSessions
                .Include(cs => cs.ChatMessages) // Eager load related messages
                .FirstOrDefaultAsync(cs => cs.Id == id);
        }

        public async Task<IEnumerable<ChatSession>> GetAllAsync()
        {
            return await _context.ChatSessions.ToListAsync();
        }

        public async Task<IEnumerable<ChatSession>> GetByUserIdAsync(string userId)
        {
            return await _context.ChatSessions
                .Where(cs => cs.ApplicationUserId == userId)
                .ToListAsync();
        }

        // Get a ChatSession along with its related messages based on sessionId
        public async Task<ChatSession> GetSessionWithMessagesByIdAsync(Guid sessionId)
        {
            return await _context.ChatSessions
                .Include(cs => cs.ChatMessages) // Eager load related messages
                .FirstOrDefaultAsync(cs => cs.Id == sessionId);
        }

        public async Task AddAsync(ChatSession chatSession)
        {
            await _context.ChatSessions.AddAsync(chatSession);
        }

        public async Task UpdateAsync(ChatSession chatSession)
        {
            var existingSession = await _context.ChatSessions.FindAsync(chatSession.Id);
            if (existingSession != null)
            {
                existingSession.SessionName = chatSession.SessionName;
                existingSession.Description = chatSession.Description;
                existingSession.ApplicationUserId = chatSession.ApplicationUserId;
                existingSession.ChatMessages = chatSession.ChatMessages; // Update related messages
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var session = await _context.ChatSessions.FindAsync(id);
            if (session != null)
            {
                _context.ChatSessions.Remove(session);
            }
        }
    }
}

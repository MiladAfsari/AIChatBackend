using Domain.Core.Entities.ChatMessageTemplateAggregate;
using Infrastructure.Data.Repository.EfCore.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repository.EfCore.Repositories
{
    public class ChatMessageRepository : IChatMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public ChatMessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ChatMessage> GetByIdAsync(Guid id)
        {
            return await _context.ChatMessages.FindAsync(id);
        }

        public async Task<IEnumerable<ChatMessage>> GetAllAsync()
        {
            return await _context.ChatMessages.ToListAsync();
        }

        // Get all chat messages for a specific user
        public async Task<IEnumerable<ChatMessage>> GetAllChatsByUserIdAsync(Guid userId)
        {
            return await _context.ChatMessages
                .Where(message => message.ChatSession.ApplicationUserId == userId)
                .ToListAsync();
        }

        // Get all chat messages by specific sessionId
        public async Task<IEnumerable<ChatMessage>> GetChatsWithFeedbackBySessionIdAsync(Guid sessionId)
        {
            return await _context.ChatMessages
                .Where(message => message.ChatSessionId == sessionId)
                .Include(message => message.Feedback)
                .ToListAsync();
        }

        public async Task AddAsync(ChatMessage chatMessage)
        {
            await _context.ChatMessages.AddAsync(chatMessage);
        }

        public async Task UpdateAsync(ChatMessage chatMessage)
        {
            var existingMessage = await _context.ChatMessages.FindAsync(chatMessage.Id);
            if (existingMessage != null)
            {
                existingMessage.Question = chatMessage.Question;
                existingMessage.Answer = chatMessage.Answer;
                existingMessage.ChatSessionId = chatMessage.ChatSessionId;
                existingMessage.Feedback = chatMessage.Feedback;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var message = await _context.ChatMessages.FindAsync(id);
            if (message != null)
            {
                _context.ChatMessages.Remove(message);
            }
        }
    }
}

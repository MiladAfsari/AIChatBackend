using Domain.Core.Entities.FeedbackTemplateAggregate;
using Infrastructure.Data.Repository.EfCore.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repository.EfCore.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly ApplicationDbContext _context;

        public FeedbackRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Feedback> GetByIdAsync(Guid id)
        {
            return await _context.Feedbacks.FindAsync(id);
        }

        public async Task<IEnumerable<Feedback>> GetAllAsync()
        {
            return await _context.Feedbacks.ToListAsync();
        }

        // Get feedback by specific user (via ApplicationUserId)
        public async Task<IEnumerable<Feedback>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Feedbacks
                .Where(feedback => feedback.ApplicationUserId == userId)
                .ToListAsync();
        }

        // Get feedback by specific message ID
        public async Task<IEnumerable<Feedback>> GetByMessageIdAsync(Guid messageId)
        {
            return await _context.Feedbacks
                .Where(feedback => feedback.ChatMessageId == messageId)
                .ToListAsync();
        }

        public async Task AddAsync(Feedback feedback)
        {
            await _context.Feedbacks.AddAsync(feedback);
        }

        public async Task UpdateAsync(Feedback feedback)
        {
            var existingFeedback = await _context.Feedbacks.FindAsync(feedback.Id);
            if (existingFeedback != null)
            {
                existingFeedback.Rating = feedback.Rating;
                existingFeedback.ChatMessageId = feedback.ChatMessageId;
                existingFeedback.ApplicationUserId = feedback.ApplicationUserId;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback != null)
            {
                _context.Feedbacks.Remove(feedback);
            }
        }
    }
}

namespace Domain.Core.Entities.FeedbackTemplateAggregate
{
    public interface IFeedbackRepository
    {
        Task<Feedback> GetByIdAsync(Guid id);
        Task<IEnumerable<Feedback>> GetAllAsync();
        Task<IEnumerable<Feedback>> GetByUserIdAsync(string userId); // Get feedback by user
        Task<IEnumerable<Feedback>> GetByMessageIdAsync(Guid messageId); // Get feedback by message ID
        Task AddAsync(Feedback feedback);
        Task UpdateAsync(Feedback feedback);
        Task DeleteAsync(Guid id);
    }
}

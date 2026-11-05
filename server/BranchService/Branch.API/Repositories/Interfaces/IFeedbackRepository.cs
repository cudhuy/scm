using Domain.Entities;

namespace Branch.API.Services.Interfaces
{
    public interface IFeedbackRepository
    {
        Task<Domain.Entities.Order> GetOrderByIdAsync(Guid orderId, Guid userId);
        Task<bool> FeedbackExistsForOrderAsync(Guid orderId);
        Task AddFeedbackAsync(Feedback feedback);
        Task<List<Feedback>> GetAllFeedbacksAsync(Guid? branchId);
    }
}

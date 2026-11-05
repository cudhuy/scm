using Branch.API.Dtos.Feedback;

namespace Branch.API.Services.Interfaces
{
    public interface IFeedbackService
    {
        Task<(bool Success, string Message)> CreateFeedbackAsync(CreateFeedbackDto feedbackDto);
        Task<FeedbackResponse> GetAllFeedbacksAsync(int page, int pageSize, Guid? branchId);
    }
}

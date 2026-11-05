using AutoMapper;
using Branch.API.Dtos.Feedback;
using Branch.API.Services.Interfaces;
using Domain.Entities;

namespace Branch.API.Services.Behaviours
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<FeedbackService> _logger;

        public FeedbackService(IFeedbackRepository feedbackRepository, IMapper mapper, ILogger<FeedbackService> logger)
        {
            _feedbackRepository = feedbackRepository;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<(bool Success, string Message)> CreateFeedbackAsync(CreateFeedbackDto feedbackDto)
        {
            if (feedbackDto == null || feedbackDto.UserId == Guid.Empty || feedbackDto.OrderId == Guid.Empty)
                return (false, "Dữ liệu yêu cầu không hợp lệ.");

            var order = await _feedbackRepository.GetOrderByIdAsync(feedbackDto.OrderId.Value, feedbackDto.UserId.Value);
            if (order == null)
                return (false, "Đơn hàng khôgn tồn tại .");
            if (order.Status != 2)
                return (false, $"Đơn hàng chưa hoàn thành nên khôgn thể phản hồi .");

            bool feedbackExists = await _feedbackRepository.FeedbackExistsForOrderAsync(feedbackDto.OrderId.Value);
            if (feedbackExists)
            {
                return (false, "Đã có phản hồi cho đơn hàng này.");
            }
            var feedback = new Feedback
            {
                UserId = feedbackDto.UserId.Value,
                OrderId = feedbackDto.OrderId.Value,
                Rating = feedbackDto.Rating.Value,
                Content = feedbackDto.Content,
                BranchId = order.BranchId,
            };

            await _feedbackRepository.AddFeedbackAsync(feedback);
            return (true, "Phản hồi đã được tạo thành công.");
        }

        public async Task<FeedbackResponse> GetAllFeedbacksAsync(int page, int pageSize, Guid? branchId)
        {
            _logger.LogInformation("Fetching feedbacks with pagination and filters");

            var feedbacks = await _feedbackRepository.GetAllFeedbacksAsync(branchId);
            if (feedbacks == null || !feedbacks.Any())
            {
                return null;
            }

            var totalItems = feedbacks.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var paginatedFeedbacks = feedbacks
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(f => new FeedbackDto
                {
                    CustomerName = f.Order.User.FullName,
                    OrderCode = f.Order.OrderCode,
                    BranchName = f.Order.Branch.Name,
                    Rating = f.Rating,
                    Content = f.Content,
                    CreatedAt = f.CreatedAt
                }).ToList();

            return new FeedbackResponse
            {
                Items = paginatedFeedbacks,
                TotalItems = totalItems,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }

    }
}

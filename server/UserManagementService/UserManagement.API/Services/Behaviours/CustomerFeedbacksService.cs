using AutoMapper;
using UserManagement.API.Dtos.FeedbackCustomer;
using UserManagement.API.Dtos.Notification;
using UserManagement.API.Repositories.Behaviours;
using UserManagement.API.Repositories.Interfaces;
using UserManagement.API.Services.Interfaces;
using static StackExchange.Redis.Role;

namespace UserManagement.API.Services.Behaviours
{
    public class CustomerFeedbacksService : ICustomerFeedbacksService
    {
        private readonly ILogger<CustomerFeedbacksService> _logger;
        private readonly ICustomerFeedbacksRepository _customerFeedbacksRepository;
        private readonly IMapper _mapper;


        public CustomerFeedbacksService(ILogger<CustomerFeedbacksService> logger, IMapper mapper, ICustomerFeedbacksRepository customerFeedbacksRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _customerFeedbacksRepository = customerFeedbacksRepository;
        }

        public async Task<IEnumerable<UserFeedbackGroupDto>> GetAllFeedbackCustomer()
        {
            try
            {
                var feedbacks = await _customerFeedbacksRepository.GetAllFeedbackCustomer();

                if (feedbacks == null || !feedbacks.Any())
                {
                    _logger.LogInformation("No feedbacks found.");
                    return Enumerable.Empty<UserFeedbackGroupDto>();
                }

                var feedbackDtos = feedbacks
                    .GroupBy(f => f.UserId) 
                    .Select(g => new UserFeedbackGroupDto
                    {
                        UserId = g.Key,
                        FullName = g.Select(f => f.User.FullName).FirstOrDefault(),
                        Email = g.Select(f => f.User.Email).FirstOrDefault(),
                        Avatar = g.Select(f => f.User.Avatar).FirstOrDefault(),
                        RankName = g.Select(f => f.User.Ranks.Name).FirstOrDefault(),
                        Phone = g.Select(f => f.User.Phone).FirstOrDefault(),
                        Address = g.Select(f => f.User.Address).FirstOrDefault(),
                        Status = g.Select(f => f.User.Status).FirstOrDefault(),
                        Feedbacks = g.Select(f => new FeedbackCustomerDto
                        {
                            Id = f.Id,
                            Content = f.Content,
                            CreatedAt = f.CreatedAt,
                        }).ToList() 
                    }).ToList();

                _logger.LogInformation($"{feedbackDtos.Count} feedback groups found.");
                return feedbackDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in service layer {nameof(GetAllFeedbackCustomer)}: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }



    }
}

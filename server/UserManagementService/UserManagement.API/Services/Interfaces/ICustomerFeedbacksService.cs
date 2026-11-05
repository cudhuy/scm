using UserManagement.API.Dtos.FeedbackCustomer;
using UserManagement.API.Dtos.Notification;

namespace UserManagement.API.Services.Interfaces
{
    public interface ICustomerFeedbacksService
    {
        Task<IEnumerable<UserFeedbackGroupDto>> GetAllFeedbackCustomer();

    }
}

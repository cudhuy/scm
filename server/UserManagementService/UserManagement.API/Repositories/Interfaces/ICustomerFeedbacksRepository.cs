using Domain.Entities;

namespace UserManagement.API.Repositories.Interfaces
{
    public interface ICustomerFeedbacksRepository
    {
        Task<List<Feedback>> GetAllFeedbackCustomer();

    }
}

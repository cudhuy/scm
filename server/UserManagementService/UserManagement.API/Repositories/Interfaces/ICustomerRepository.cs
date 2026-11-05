using Domain.Entities;
using UserManagement.API.Dtos.Customer;

namespace UserManagement.API.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<User> GetCustomerById(Guid id);
        Task<IEnumerable<User>> GetListCustomers();
        Task<bool> UpdateCustomer(CustomerUpdateDto customer);
        Task<bool> Save();
        Task<int> CountTotalOrders(Guid userId);
        Task<int> CountPendingOrders(Guid userId);
        Task<int> CountCompletedOrders(Guid userId);

    }
}

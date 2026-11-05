using Domain.Entities;
using UserManagement.API.Dtos.Customer;
using UserManagement.API.Dtos.Profile;

namespace UserManagement.API.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<GetCustomersResponseDto> GetAllCustomers(int page, int? pageSize, string? search, Guid? rankId);
        Task<User> GetCustomerById(Guid id);
        Task<bool> UpdateCustomer(CustomerUpdateDto customer);
        Task<UserProfileDto?> GetUserProfileAsync(Guid userId);
    }
}

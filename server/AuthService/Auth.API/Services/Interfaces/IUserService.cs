using Auth.API.Dtos;
using Auth.API.Entities;

namespace Auth.API.Services.Interfaces
{
    public interface IUserService
    {
        List<UserDto> FetchUsers();
        Task<UserRole> GetRoleById(string? roleId);
        Task<StaffInfo> GetStaffByUserId(string id);
        Task<User> GetUserByEmail(string email);
        Task<bool> UpdateUser(User user);
    }
}
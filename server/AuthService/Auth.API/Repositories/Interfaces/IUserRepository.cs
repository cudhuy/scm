using Auth.API.Dtos;
using Auth.API.Entities;

namespace AuthService.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> AddUser(string userId, RegisterDto registerDto, byte[] passwordHash, byte[] passwordSalt);

        //Task<bool> Save();

        Task<User> GetUser(LoginDto userLogin);

        Task<User> GetUserByEmail(string email);
        Task<bool> UpdateUser(User user);
        Task<UserRole> GetRoleById(string? roleId);
        Task<StaffInfo> GetStaffByUserId(string id);
    }
}
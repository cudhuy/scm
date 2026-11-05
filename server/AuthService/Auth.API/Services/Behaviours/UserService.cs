using Auth.API.Dtos;
using Auth.API.Entities;
using Auth.API.Services.Interfaces;
using AuthService.Repositories.Behaviours;
using AuthService.Repositories.Interfaces;

namespace Auth.API.Services.Behaviours
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserRepository> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserRepository> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public List<UserDto> FetchUsers()
        {
            _logger.LogInformation("Entered method FetchUsers");
            _logger.LogInformation("Leaving method FetchUsers");
            throw new NotImplementedException();
        }

        public Task<UserRole> GetRoleById(string? roleId)
        {
            return _userRepository.GetRoleById(roleId);
        }

        public Task<StaffInfo> GetStaffByUserId(string id)
        {
            return _userRepository.GetStaffByUserId(id);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            _logger.LogInformation("Entered method FetchUsers");
            var result = await _userRepository.GetUserByEmail(email);
            _logger.LogInformation("Leaving method FetchUsers");
            return result;
        }

        public async Task<bool> UpdateUser(User user)
        {
            return await _userRepository.UpdateUser(user);
        }
    }
}
using Auth.API.Dtos;
using Auth.API.Entities;
using Auth.API.Helpers;
using Auth.API.Services.Interfaces;
using AuthService.Repositories.Interfaces;

namespace Auth.API.Services.Behaviours
{
    public class AuthenService : IAuthenService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthenService> _logger;

        public AuthenService(IUserRepository userRepository, ILogger<AuthenService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<User> Authenticate(LoginDto userLogin)
        {
            _logger.LogInformation("Entered method Authenticate");
            var user = await _userRepository.GetUser(userLogin);
            if (user == null || !PassWordHelper.VerifyPasswordHash(userLogin.PassWord, user.PasswordHash, user.PasswordSalt)) return null;
            _logger.LogInformation("Leaving method Authenticate");
            return user;
        }

        public async Task<bool> RegisterUser(string userId, RegisterDto registerDto)
        {
            _logger.LogInformation("Entered method RegisterUser");
            PassWordHelper.CreatePasswordHash(registerDto.PassWord, out var passwordHash, out var passwordSalt);
            var result = await _userRepository.AddUser(userId, registerDto, passwordHash, passwordSalt);
            _logger.LogInformation("Leaving method RegisterUser");
            return result;
        }
    }
}
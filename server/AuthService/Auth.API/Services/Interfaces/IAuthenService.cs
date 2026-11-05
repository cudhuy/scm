using Auth.API.Dtos;
using Auth.API.Entities;

namespace Auth.API.Services.Interfaces
{
    public interface IAuthenService
    {
        Task<User> Authenticate(LoginDto userLogin);

        Task<bool> RegisterUser(string userId, RegisterDto registerDto);
    }
}
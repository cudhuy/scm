using Auth.API.Entities;

namespace Auth.API.Services.Interfaces
{
    public interface ITokenService
    {
        Task<bool> DeleteOtpByUserId(string id);
        Task<string> GenerateToken(User user);
        Task<string> GetOtpByUserId(string id);
        Task<bool> SaveTokenToDB(string userId, string token, string tokenType);
    }
}
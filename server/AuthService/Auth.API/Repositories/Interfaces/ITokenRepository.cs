namespace Auth.API.Repositories.Interfaces
{
    public interface ITokenRepository
    {
        Task<bool> SaveTokenToDB(string userId, string token, string tokenType);

        //Task<bool> Save();
        Task<string> GetOtpByUserId(string id);
        Task<bool> DeleteOtpByUserId(string id);
    }
}
using Auth.API.Entities;
using Auth.API.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace Auth.API.Repositories.Behaviours
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IMongoCollection<Token> _tokens;
        private readonly ILogger<TokenRepository> _logger;

        public TokenRepository(IMongoDatabase database, ILogger<TokenRepository> logger)
        {
            _tokens = database.GetCollection<Token>("token");
            _logger = logger;
        }

        public async Task<bool> DeleteOtpByUserId(string id)
        {
            var result = await _tokens.DeleteOneAsync(x => x.UserId == id && x.TokenType == "OTP");
            return result.DeletedCount > 0;
        }

        public async Task<string> GetOtpByUserId(string id)
        {
            var token = await _tokens.Find(x => x.UserId == id && x.TokenType == "OTP").FirstOrDefaultAsync();
            return token?.TokenValue;
        }

        public async Task<bool> SaveTokenToDB(string userId, string token, string tokenType)
        {
            _logger.LogInformation("Entered method SaveTokenToDB TokenRepository");
            if (string.IsNullOrEmpty(token)) throw new ArgumentNullException(nameof(token));

            var newToken = new Token
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                TokenValue = token,
                TokenType = tokenType,
                ExpirationDate = DateTime.UtcNow.AddMinutes(30),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false,
                IsDeleted = false
            };

            await _tokens.InsertOneAsync(newToken);
            _logger.LogInformation("Leaving method SaveTokenToDB TokenRepository");
            return true;
        }
    }
}

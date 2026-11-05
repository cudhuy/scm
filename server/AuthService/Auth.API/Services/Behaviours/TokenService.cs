using Auth.API.Entities;
using Auth.API.Repositories.Interfaces;
using Auth.API.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth.API.Services.Behaviours
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly ITokenRepository _tokenRepository;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IConfiguration config, ITokenRepository tokenRepository, ILogger<TokenService> logger)
        {
            _config = config;
            _tokenRepository = tokenRepository;
            _logger = logger;
        }

        public async Task<bool> DeleteOtpByUserId(string id)
        {
            return await _tokenRepository.DeleteOtpByUserId(id);
        }

        public async Task<string> GenerateToken(User user)
        {
            _logger.LogInformation("Entered method GenerateToken");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.RoleId.ToString())
                }),
                Issuer = _config["Jwt:Issuer"],
                Expires = DateTime.Now.AddMinutes(_config.GetValue<int>("Jwt:TokenValidityMins")),
                Audience = _config["Jwt:Audience"],
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);

            await SaveTokenToDB(user.Id, accessToken, "Login");
            _logger.LogInformation("Leaving method GenerateToken");
            return accessToken;
        }

        public async Task<string> GetOtpByUserId(string id)
        {
            _logger.LogInformation("Entered method GetOtpByUserId");
            return await _tokenRepository.GetOtpByUserId(id);
        }

        public async Task<bool> SaveTokenToDB(string userId, string token, string tokenType)
        {
            _logger.LogInformation("Entered method SaveTokenToDB");
            var result = await _tokenRepository.SaveTokenToDB(userId, token, tokenType);
            _logger.LogInformation("Leaving method SaveTokenToDB");
            return result;
        }
    }
}
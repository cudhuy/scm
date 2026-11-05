using Auth.API.Constants;
using Auth.API.Dtos;
using Auth.API.Entities;
using Auth.API.Messaging.KafkaMessage;
using Auth.API.Messaging.Producers;
using AuthService.Repositories.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace AuthService.Repositories.Behaviours
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<UserRole> _userRoles;
        private readonly IMongoCollection<StaffInfo> _staffInfos;
        private readonly ILogger<UserRepository> _logger;
        private readonly IMapper _mapper;
        private readonly RegisterProducerService _registerProducerService;

        public UserRepository(IMongoDatabase database, ILogger<UserRepository> logger, IMapper mapper, RegisterProducerService registerProducerService)
        {
            _users = database.GetCollection<User>("user");
            _userRoles = database.GetCollection<UserRole>("userrole");
            _staffInfos = database.GetCollection<StaffInfo>("staffInfos");
            _logger = logger;
            _mapper = mapper;
            _registerProducerService = registerProducerService;
        }

        public async Task<bool> AddUser(string userId, RegisterDto registerDto, byte[] passwordHash, byte[] passwordSalt)
        {
            _logger.LogInformation("Entered method AddUser UserRepository");
            var userEntity = new User
            {
                Id = userId,
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                CreatedAt = DateTime.Now,
                RoleId = AppConstants.USER_ROLE_ID,
                Status = "Active",
                IsDeleted = false,
                IsVerified = registerDto.IsLoginWithGoogle,
                UpdatedAt = DateTime.Now
            };
            await _users.InsertOneAsync(userEntity);
            if (registerDto.IsLoginWithGoogle) await SerializationUser(userEntity);
            _logger.LogInformation("Leaving method AddUser UserRepository");
            return true;
        }

        private async Task SerializationUser(User userEntity)
        {
            _logger.LogInformation("Entered method SerializationUser UserRepository");
            var userMessage = _mapper.Map<User, UserRegisterMessage>(userEntity);
            var messageJson = JsonConvert.SerializeObject(userMessage);
            await _registerProducerService.ProducerMessage(messageJson);
            _logger.LogInformation("Leaving method SerializationUser UserRepository");
        }

        public async Task<User> GetUser(LoginDto userLogin)
        {
            if (userLogin == null || string.IsNullOrWhiteSpace(userLogin.Email)) return null;
            return await _users.Find(u => u.Email == userLogin.Email).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByEmail(string email)
        {
            _logger.LogInformation("Entered method GetUserByEmail UserRepository");
            var result = await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
            _logger.LogInformation("Leaving method GetUserByEmail UserRepository");
            return result;
        }

        public async Task<bool> UpdateUser(User user)
        {
            var updateResult = await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
            return updateResult.ModifiedCount > 0;
        }

        public async Task<UserRole> GetRoleById(string? roleId)
        {
            return await _userRoles.Find(r => r.Id == roleId)
                .FirstOrDefaultAsync();
        }

        public async Task<StaffInfo> GetStaffByUserId(string id)
        {
            return await _staffInfos.Find(s => s.UserId == id).FirstOrDefaultAsync();
        }
    }
}

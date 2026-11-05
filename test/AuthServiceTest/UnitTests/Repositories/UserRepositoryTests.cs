//using Auth.API.Dtos;
//using Auth.API.Entities;
//using Auth.API.Messaging.Producers;
//using AuthService.Repositories.Behaviours;
//using AutoMapper;
//using Microsoft.Extensions.Logging;
//using MongoDB.Driver;
//using Moq;
//using System.Threading;
//using System.Threading.Tasks;
//using Xunit;

//namespace AuthServiceTest.UnitTests.Repositories
//{
//    public class UserRepositoryTests
//    {
//        private readonly Mock<IMongoCollection<User>> _mockUsers;
//        private readonly Mock<IMongoCollection<UserRole>> _mockUserRoles;
//        private readonly Mock<IMongoCollection<StaffInfo>> _mockStaffInfos;
//        private readonly Mock<IMongoDatabase> _mockDatabase;
//        private readonly Mock<ILogger<UserRepository>> _mockLogger;
//        private readonly Mock<IMapper> _mockMapper;
//        private readonly Mock<RegisterProducerService> _mockRegisterProducerService;
//        private readonly UserRepository _userRepository;

//        public UserRepositoryTests()
//        {
//            _mockUsers = new Mock<IMongoCollection<User>>();
//            _mockUserRoles = new Mock<IMongoCollection<UserRole>>();
//            _mockStaffInfos = new Mock<IMongoCollection<StaffInfo>>();
//            _mockDatabase = new Mock<IMongoDatabase>();
//            _mockLogger = new Mock<ILogger<UserRepository>>();
//            _mockMapper = new Mock<IMapper>();
//            _mockRegisterProducerService = new Mock<RegisterProducerService>(); // Sử dụng interface thay vì class

//            _mockDatabase.Setup(db => db.GetCollection<User>("user", null)).Returns(_mockUsers.Object);
//            _mockDatabase.Setup(db => db.GetCollection<UserRole>("userrole", null)).Returns(_mockUserRoles.Object);
//            _mockDatabase.Setup(db => db.GetCollection<StaffInfo>("staffInfos", null)).Returns(_mockStaffInfos.Object);

//            _userRepository = new UserRepository(
//                _mockDatabase.Object,
//                _mockLogger.Object,
//                _mockMapper.Object,
//                _mockRegisterProducerService.Object
//            );
//        }

//        [Fact]
//        public async Task AddUser_ShouldInsertUser()
//        {
//            // Arrange
//            var userId = "123";
//            var registerDto = new RegisterDto
//            {
//                FullName = "Test User",
//                Email = "test@example.com",
//                PassWord = "password123",
//                IsLoginWithGoogle = false
//            };
//            byte[] passwordHash = new byte[64];
//            byte[] passwordSalt = new byte[128];

//            _mockRegisterProducerService
//                .Setup(s => s.ProducerMessage(It.IsAny<string>()))
//                .Verifiable();

//            // Act
//            var result = await _userRepository.AddUser(userId, registerDto, passwordHash, passwordSalt);

//            // Assert
//            Assert.True(result);
//            _mockUsers.Verify(u => u.InsertOneAsync(It.IsAny<User>(), null, default), Times.Once);
//            _mockRegisterProducerService.Verify(s => s.ProducerMessage(It.IsAny<string>()), Times.Once);
//        }

//        [Fact]
//        public async Task GetUserByEmail_ShouldReturnUser_WhenExists()
//        {
//            // Arrange
//            var email = "test@example.com";
//            var expectedUser = new User { Id = "123", Email = email };
//            var mockCursor = new Mock<IAsyncCursor<User>>();
//            mockCursor.SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);
//            mockCursor.Setup(c => c.Current).Returns(new List<User> { expectedUser });

//            _mockUsers.Setup(u => u.FindAsync(It.IsAny<FilterDefinition<User>>(), null, default))
//                      .ReturnsAsync(mockCursor.Object);

//            // Act
//            var result = await _userRepository.GetUserByEmail(email);

//            // Assert
//            Assert.NotNull(result);
//            Assert.Equal(expectedUser.Id, result.Id);
//        }

//        [Fact]
//        public async Task GetUserByEmail_ShouldReturnNull_WhenUserNotFound()
//        {
//            // Arrange
//            var email = "notfound@example.com";
//            var mockCursor = new Mock<IAsyncCursor<User>>();
//            mockCursor.Setup(c => c.MoveNext(It.IsAny<CancellationToken>())).Returns(false);

//            _mockUsers.Setup(u => u.FindAsync(It.IsAny<FilterDefinition<User>>(), null, default))
//                      .ReturnsAsync(mockCursor.Object);

//            // Act
//            var result = await _userRepository.GetUserByEmail(email);

//            // Assert
//            Assert.Null(result);
//        }

//        [Fact]
//        public async Task UpdateUser_ShouldReturnTrue_WhenUserUpdated()
//        {
//            // Arrange
//            var user = new User { Id = "123", Email = "test@example.com" };
//            var updateResult = new Mock<ReplaceOneResult>();
//            updateResult.Setup(r => r.ModifiedCount).Returns(1);

//            _mockUsers.Setup(u => u.ReplaceOneAsync(It.IsAny<FilterDefinition<User>>(), user, It.IsAny<ReplaceOptions>(), default))
//                                  .ReturnsAsync(updateResult.Object);

//            // Act
//            var result = await _userRepository.UpdateUser(user);

//            // Assert
//            Assert.True(result);
//        }

//        [Fact]
//        public async Task UpdateUser_ShouldReturnFalse_WhenNoModification()
//        {
//            // Arrange
//            var user = new User { Id = "123", Email = "test@example.com" };
//            var updateResult = new Mock<ReplaceOneResult>();
//            updateResult.Setup(r => r.ModifiedCount).Returns(0);

//            _mockUsers.Setup(u => u.ReplaceOneAsync(It.IsAny<FilterDefinition<User>>(), user, It.IsAny<ReplaceOptions>(), default))
//                                  .ReturnsAsync(updateResult.Object);

//            // Act
//            var result = await _userRepository.UpdateUser(user);

//            // Assert
//            Assert.False(result);
//        }
//    }
//}

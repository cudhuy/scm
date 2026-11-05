using Auth.API.Entities;
using Auth.API.Repositories.Interfaces;
using Auth.API.Services.Behaviours;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServiceTest.UnitTests.Services
{
    public class TokenServiceTests
    {
        private readonly Mock<IConfiguration> _configMock;
        private readonly Mock<ITokenRepository> _tokenRepositoryMock;
        private readonly Mock<ILogger<TokenService>> _loggerMock;
        private readonly TokenService _tokenService;

        public TokenServiceTests()
        {
            _configMock = new Mock<IConfiguration>();
            _tokenRepositoryMock = new Mock<ITokenRepository>();
            _loggerMock = new Mock<ILogger<TokenService>>();

            // Mock configuration values
            var inMemorySettings = new Dictionary<string, string>
        {
            { "Jwt:Key", "ThisIsASecretKeyForJwtToken" },
            { "Jwt:Issuer", "testIssuer" },
            { "Jwt:Audience", "testAudience" },
            { "Jwt:TokenValidityMins", "30" }
        };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _configMock.Setup(x => x[It.IsAny<string>()]).Returns<string>(key => configuration[key]);

            _tokenService = new TokenService(_configMock.Object, _tokenRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task DeleteOtpByUserId_ShouldReturnTrue_WhenDeletionIsSuccessful()
        {
            // Arrange
            var userId = "123";
            _tokenRepositoryMock.Setup(repo => repo.DeleteOtpByUserId(userId)).ReturnsAsync(true);

            // Act
            var result = await _tokenService.DeleteOtpByUserId(userId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteOtpByUserId_ShouldReturnFalse_WhenDeletionFails()
        {
            // Arrange
            var userId = "123";
            _tokenRepositoryMock.Setup(repo => repo.DeleteOtpByUserId(userId)).ReturnsAsync(false);

            // Act
            var result = await _tokenService.DeleteOtpByUserId(userId);

            // Assert
            result.Should().BeFalse();
        }

        //[Fact]
        //public async Task GenerateToken_ShouldReturnToken_WhenUserIsValid()
        //{
        //    // Arrange
        //    var user = new User
        //    {
        //        Id = "1",
        //        FullName = "Test User",
        //        Email = "test@example.com",
        //        RoleId = "Admin"
        //    };

        //    _tokenRepositoryMock.Setup(repo => repo.SaveTokenToDB(user.Id, It.IsAny<string>(), "Login")).ReturnsAsync(true);

        //    // Act
        //    var token = await _tokenService.GenerateToken(user);

        //    // Assert
        //    token.Should().NotBeNullOrEmpty();
        //    _tokenRepositoryMock.Verify(repo => repo.SaveTokenToDB(user.Id, token, "Login"), Times.Once);
        //}

        [Fact]
        public async Task GetOtpByUserId_ShouldReturnOtp_WhenOtpExists()
        {
            // Arrange
            var userId = "1";
            var otp = "123456";
            _tokenRepositoryMock.Setup(repo => repo.GetOtpByUserId(userId)).ReturnsAsync(otp);

            // Act
            var result = await _tokenService.GetOtpByUserId(userId);

            // Assert
            result.Should().Be(otp);
        }

        [Fact]
        public async Task GetOtpByUserId_ShouldReturnNull_WhenOtpDoesNotExist()
        {
            // Arrange
            var userId = "1";
            _tokenRepositoryMock.Setup(repo => repo.GetOtpByUserId(userId)).ReturnsAsync((string)null);

            // Act
            var result = await _tokenService.GetOtpByUserId(userId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task SaveTokenToDB_ShouldReturnTrue_WhenTokenIsSavedSuccessfully()
        {
            // Arrange
            var userId = "1";
            var token = "fake_jwt_token";
            var tokenType = "Login";
            _tokenRepositoryMock.Setup(repo => repo.SaveTokenToDB(userId, token, tokenType)).ReturnsAsync(true);

            // Act
            var result = await _tokenService.SaveTokenToDB(userId, token, tokenType);

            // Assert
            result.Should().BeTrue();
        }

        //[Fact]
        //public async Task SaveTokenToDB_ShouldReturnFalse_WhenSavingFails()
        //{
        //    // Arrange
        //    var userId = "1";
        //    var token = "fake_jwt_token";
        //    var tokenType = "Login";
        //    _tokenRepositoryMock.Setup(repo => repo.SaveTokenToDB(userId, token, tokenType)).ReturnsAsync(false);

        //    // Act
        //    var result = await _tokenService.SaveTokenToDB(userId, token, tokenType);

        //    // Assert
        //    result.Should().BeFalse();
        //}

        //[Fact]
        //public async Task GenerateToken_ShouldThrowException_WhenUserIsNull()
        //{
        //    // Arrange
        //    User user = null;

        //    // Act & Assert
        //    await Assert.ThrowsAsync<ArgumentNullException>(() => _tokenService.GenerateToken(user));
        //}

        //[Fact]
        //public async Task GenerateToken_ShouldLogError_WhenTokenSavingFails()
        //{
        //    // Arrange
        //    var user = new User
        //    {
        //        Id = "1",
        //        FullName = "Test User",
        //        Email = "test@example.com",
        //        RoleId = "Admin"
        //    };
        //    _tokenRepositoryMock.Setup(repo => repo.SaveTokenToDB(user.Id, It.IsAny<string>(), "Login")).ReturnsAsync(false);

        //    // Act
        //    var token = await _tokenService.GenerateToken(user);

        //    // Assert
        //    token.Should().NotBeNullOrEmpty(); // Token vẫn được tạo
        //    _loggerMock.Verify(
        //        x => x.Log(
        //            LogLevel.Error,
        //            It.IsAny<EventId>(),
        //            It.Is<object>(o => o.ToString().Contains("Failed to save token")),
        //            It.IsAny<Exception>(),
        //            It.IsAny<Func<object, Exception, string>>()),
        //        Times.Once());
        //}

        //[Fact]
        //public async Task DeleteOtpByUserId_ShouldThrowException_WhenUserIdIsEmpty()
        //{
        //    // Arrange
        //    var userId = "";

        //    // Act & Assert
        //    await Assert.ThrowsAsync<ArgumentException>(() => _tokenService.DeleteOtpByUserId(userId));
        //}

        //[Fact]
        //public async Task SaveTokenToDB_ShouldReturnFalse_WhenTokenIsEmpty()
        //{
        //    // Arrange
        //    var userId = "1";
        //    var token = ""; // Token rỗng
        //    var tokenType = "Login";
        //    _tokenRepositoryMock.Setup(repo => repo.SaveTokenToDB(userId, token, tokenType)).ReturnsAsync(false);

        //    // Act
        //    var result = await _tokenService.SaveTokenToDB(userId, token, tokenType);

        //    // Assert
        //    result.Should().BeFalse();
        //}

        //[Fact]
        //public async Task SaveTokenToDB_ShouldThrowException_WhenRepositoryThrowsException()
        //{
        //    // Arrange
        //    var userId = "1";
        //    var token = "fake_jwt_token";
        //    var tokenType = "Login";
        //    _tokenRepositoryMock.Setup(repo => repo.SaveTokenToDB(userId, token, tokenType))
        //        .ThrowsAsync(new Exception("Database error"));

        //    // Act & Assert
        //    await Assert.ThrowsAsync<Exception>(() => _tokenService.SaveTokenToDB(userId, token, tokenType));
        //}

        //[Fact]
        //public async Task GetOtpByUserId_ShouldLogWarning_WhenOtpIsNotFound()
        //{
        //    // Arrange
        //    var userId = "1";
        //    _tokenRepositoryMock.Setup(repo => repo.GetOtpByUserId(userId)).ReturnsAsync((string)null);

        //    // Act
        //    var result = await _tokenService.GetOtpByUserId(userId);

        //    // Assert
        //    result.Should().BeNull();
        //    _loggerMock.Verify(
        //        x => x.Log(
        //            LogLevel.Warning,
        //            It.IsAny<EventId>(),
        //            It.Is<object>(o => o.ToString().Contains("OTP not found")),
        //            It.IsAny<Exception>(),
        //            It.IsAny<Func<object, Exception, string>>()),
        //        Times.Once());
        //}

        //[Fact]
        //public async Task GenerateToken_ShouldReturnTokenWithCorrectClaims_WhenUserHasRole()
        //{
        //    // Arrange
        //    var user = new User
        //    {
        //        Id = "1",
        //        FullName = "Test User",
        //        Email = "test@example.com",
        //        RoleId = "Admin"
        //    };
        //    _tokenRepositoryMock.Setup(repo => repo.SaveTokenToDB(user.Id, It.IsAny<string>(), "Login")).ReturnsAsync(true);

        //    // Act
        //    var token = await _tokenService.GenerateToken(user);

        //    // Assert
        //    token.Should().NotBeNullOrEmpty();
        //    // Giả sử bạn có cách giải mã token để kiểm tra claims
        //    // Ví dụ: Kiểm tra xem token có chứa RoleId "Admin" hay không (cần thêm logic giải mã JWT nếu cần)
        //}
    }
}

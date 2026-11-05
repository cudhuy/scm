using Auth.API.Dtos;
using Auth.API.Entities;
using Auth.API.Helpers;
using Auth.API.Services.Behaviours;
using AuthService.Repositories.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServiceTest.UnitTests.Services
{
    public class AuthenServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILogger<AuthenService>> _loggerMock;
        private readonly AuthenService _authenService;

        public AuthenServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<AuthenService>>();
            _authenService = new AuthenService(_userRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Authenticate_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var loginDto = new LoginDto { Email = "test@example.com", PassWord = "password123" };
            _userRepositoryMock.Setup(repo => repo.GetUser(loginDto)).ReturnsAsync((User)null);

            // Act
            var result = await _authenService.Authenticate(loginDto);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Authenticate_ShouldReturnUser_WhenCredentialsAreValid()
        {
            // Arrange
            var loginDto = new LoginDto { Email = "test@example.com", PassWord = "password123" };
            PassWordHelper.CreatePasswordHash(loginDto.PassWord, out var passwordHash, out var passwordSalt);
            var user = new User { Id = "1", Email = loginDto.Email, PasswordHash = passwordHash, PasswordSalt = passwordSalt };

            _userRepositoryMock.Setup(repo => repo.GetUser(loginDto)).ReturnsAsync(user);

            // Act
            var result = await _authenService.Authenticate(loginDto);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be(loginDto.Email);
        }

        [Fact]
        public async Task Authenticate_ShouldReturnNull_WhenPasswordIsIncorrect()
        {
            // Arrange
            var loginDto = new LoginDto { Email = "test@example.com", PassWord = "wrongpassword" };
            PassWordHelper.CreatePasswordHash("correctpassword", out var passwordHash, out var passwordSalt);
            var user = new User { Id = "1", Email = loginDto.Email, PasswordHash = passwordHash, PasswordSalt = passwordSalt };

            _userRepositoryMock.Setup(repo => repo.GetUser(loginDto)).ReturnsAsync(user);

            // Act
            var result = await _authenService.Authenticate(loginDto);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task RegisterUser_ShouldReturnTrue_WhenUserIsRegisteredSuccessfully()
        {
            // Arrange
            var userId = "1";
            var registerDto = new RegisterDto { Email = "test@example.com", PassWord = "password123" };
            _userRepositoryMock.Setup(repo => repo.AddUser(It.IsAny<string>(), It.IsAny<RegisterDto>(), It.IsAny<byte[]>(), It.IsAny<byte[]>())).ReturnsAsync(true);

            // Act
            var result = await _authenService.RegisterUser(userId, registerDto);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task RegisterUser_ShouldReturnFalse_WhenRegistrationFails()
        {
            // Arrange
            var userId = "1";
            var registerDto = new RegisterDto { Email = "test@example.com", PassWord = "password123" };
            _userRepositoryMock.Setup(repo => repo.AddUser(It.IsAny<string>(), It.IsAny<RegisterDto>(), It.IsAny<byte[]>(), It.IsAny<byte[]>())).ReturnsAsync(false);

            // Act
            var result = await _authenService.RegisterUser(userId, registerDto);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task Authenticate_ShouldReturnNull_WhenEmailIsEmpty()
        {
            // Arrange
            var loginDto = new LoginDto { Email = "", PassWord = "password123" };
            _userRepositoryMock.Setup(repo => repo.GetUser(loginDto)).ReturnsAsync((User)null);

            // Act
            var result = await _authenService.Authenticate(loginDto);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Authenticate_ShouldReturnNull_WhenPasswordIsEmpty()
        {
            // Arrange
            var loginDto = new LoginDto { Email = "test@example.com", PassWord = "" };
            PassWordHelper.CreatePasswordHash("password123", out var passwordHash, out var passwordSalt);
            var user = new User { Id = "1", Email = loginDto.Email, PasswordHash = passwordHash, PasswordSalt = passwordSalt };

            _userRepositoryMock.Setup(repo => repo.GetUser(loginDto)).ReturnsAsync(user);

            // Act
            var result = await _authenService.Authenticate(loginDto);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Authenticate_ShouldThrowException_WhenRepositoryThrowsException()
        {
            // Arrange
            var loginDto = new LoginDto { Email = "test@example.com", PassWord = "password123" };
            _userRepositoryMock.Setup(repo => repo.GetUser(loginDto)).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _authenService.Authenticate(loginDto));
        }

        [Fact]
        public async Task RegisterUser_ShouldReturnFalse_WhenEmailAlreadyExists()
        {
            // Arrange
            var userId = "1";
            var registerDto = new RegisterDto { Email = "test@example.com", PassWord = "password123" };
            _userRepositoryMock.Setup(repo => repo.AddUser(It.IsAny<string>(), It.IsAny<RegisterDto>(), It.IsAny<byte[]>(), It.IsAny<byte[]>())).ReturnsAsync(false);

            // Act
            var result = await _authenService.RegisterUser(userId, registerDto);

            // Assert
            result.Should().BeFalse();
        }

        //[Fact]
        //public async Task RegisterUser_ShouldThrowException_WhenPasswordHashGenerationFails()
        //{
        //    // Arrange
        //    var userId = "1";
        //    var registerDto = new RegisterDto { Email = "test@example.com", PassWord = null }; // PassWord null để mô phỏng lỗi
        //    _userRepositoryMock.Setup(repo => repo.AddUser(It.IsAny<string>(), It.IsAny<RegisterDto>(), It.IsAny<byte[]>(), It.IsAny<byte[]>())).ThrowsAsync(new Exception("Hash generation failed"));

        //    // Act & Assert
        //    await Assert.ThrowsAsync<Exception>(() => _authenService.RegisterUser(userId, registerDto));
        //}

        //[Fact]
        //public async Task Authenticate_ShouldLogInfo_WhenMethodIsCalled()
        //{
        //    // Arrange
        //    var loginDto = new LoginDto { Email = "test@example.com", PassWord = "wrongpassword" };
        //    PassWordHelper.CreatePasswordHash("correctpassword", out var passwordHash, out var passwordSalt);
        //    var user = new User { Id = "1", Email = loginDto.Email, PasswordHash = passwordHash, PasswordSalt = passwordSalt };

        //    _userRepositoryMock.Setup(repo => repo.GetUser(loginDto)).ReturnsAsync(user);

        //    // Act
        //    var result = await _authenService.Authenticate(loginDto);

        //    // Assert
        //    result.Should().BeNull();
        //    _loggerMock.Verify(
        //        x => x.Log(
        //            LogLevel.Information,
        //            It.IsAny<EventId>(),
        //            It.Is<object>(o => o.ToString().Contains("Entered method Authenticate")), // Sử dụng It.Is<object>
        //            It.IsAny<Exception>(),
        //            It.IsAny<Func<object, Exception, string>>()), // Sử dụng Func<object, Exception, string>
        //        Times.Once());
        //}
    }
}

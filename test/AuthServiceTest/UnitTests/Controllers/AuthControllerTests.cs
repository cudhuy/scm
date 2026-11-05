//using Auth.API.Controllers;
//using Auth.API.Dtos;
//using Auth.API.Entities;
//using Auth.API.Services.Interfaces;
//using FluentAssertions;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using Moq;
//using StackExchange.Redis;
//using System.Threading.Tasks;
//using Xunit;

//namespace AuthServiceTest.Controllers
//{
//    public class AuthControllerTests
//    {
//        private readonly AuthController _controller;
//        private readonly Mock<IUserService> _userServiceMock = new();
//        private readonly Mock<IAuthenService> _authenServiceMock = new();
//        private readonly Mock<ITokenService> _tokenServiceMock = new();
//        private readonly Mock<ILogger<AuthController>> _loggerMock = new();

//        public AuthControllerTests()
//        {
//            _controller = new AuthController(
//                _userServiceMock.Object,
//                _tokenServiceMock.Object,
//                null,  // EmailHelper (có thể mock nếu cần)
//                _authenServiceMock.Object,
//                _loggerMock.Object,
//                null,  // RedisHelper (có thể mock nếu cần)
//                null,  // RegisterProducerService
//                null   // IMapper
//            );
//        }

//        [Fact]
//        public async Task Login_ShouldReturnUnauthorized_WhenInvalidCredentials()
//        {
//            // Arrange
//            var loginDto = new LoginDto { Email = "test@example.com", PassWord = "wrongpass" };
//            _authenServiceMock.Setup(s => s.Authenticate(loginDto)).ReturnsAsync((User)null);

//            // Act
//            var result = await _controller.Login(loginDto);

//            // Assert
//            var actionResult = Assert.IsType<UnauthorizedObjectResult>(result);
//            Assert.Equal(401, actionResult.StatusCode);
//        }

//        [Fact]
//        public async Task Login_ShouldReturnOk_WhenValidCredentials()
//        {
//            // Arrange
//            var user = new User { Email = "test@example.com", IsVerified = true, Status = "Active" };
//            var loginDto = new LoginDto { Email = "test@example.com", PassWord = "correctpass" };
//            _authenServiceMock.Setup(s => s.Authenticate(loginDto)).ReturnsAsync(user);
//            _tokenServiceMock.Setup(s => s.GenerateToken(user)).ReturnsAsync("fake_token");
//            _userServiceMock.Setup(s => s.GetRoleById(It.IsAny<string>())).ReturnsAsync(new UserRole { RoleName = "User" });
//            _userServiceMock.Setup(s => s.GetStaffByUserId(It.IsAny<string>())).ReturnsAsync((StaffInfo)null);

//            // Act
//            var result = await _controller.Login(loginDto);

//            // Assert
//            var actionResult = Assert.IsType<OkObjectResult>(result);
//            Assert.Equal(200, actionResult.StatusCode);
//        }

//        [Fact]
//        public async Task ResetPassword_ShouldReturnNotFound_WhenUserDoesNotExist()
//        {
//            // Arrange
//            var resetPasswordDto = new ResetPasswordDto { Email = "nonexistent@example.com", NewPassword = "NewPass123" };
//            _userServiceMock.Setup(x => x.GetUserByEmail(resetPasswordDto.Email)).ReturnsAsync((User)null);

//            // Act
//            var result = await _controller.ResetPassword(resetPasswordDto);

//            // Assert
//            result.Should().BeOfType<NotFoundObjectResult>();
//        }

//        [Fact]
//        public async Task ResetPassword_ShouldReturnOk_WhenPasswordIsResetSuccessfully()
//        {
//            // Arrange
//            var user = new User { Id = "1", Email = "test@example.com" };
//            var resetPasswordDto = new ResetPasswordDto { Email = "test@example.com", NewPassword = "NewPass123" };
//            _userServiceMock.Setup(x => x.GetUserByEmail(resetPasswordDto.Email)).ReturnsAsync(user);
//            _userServiceMock.Setup(x => x.UpdateUser(user)).ReturnsAsync(true);

//            // Act
//            var result = await _controller.ResetPassword(resetPasswordDto);

//            // Assert
//            result.Should().BeOfType<OkObjectResult>();
//        }
//    }
//}

using Auth.API.Controllers;
using Auth.API.Dtos;
using Auth.API.Entities;
using Auth.API.Services.Interfaces;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
namespace AuthServiceTest.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _mapperMock = new Mock<IMapper>();

            _controller = new UserController(_userServiceMock.Object, _mapperMock.Object);

            // Mock HttpContext để tránh NullReferenceException
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, "1") // Mock userId
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetUserByEmail_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            string email = "nonexistent@example.com";
            _userServiceMock.Setup(x => x.GetUserByEmail(email)).ReturnsAsync((User)null);

            // Act
            var result = await _controller.GetUserByEmail(email);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetUserByEmail_ShouldReturnUserDto_WhenUserExists()
        {
            // Arrange
            string email = "test@example.com";
            var user = new User { Id = "1", Email = email };
            var userDto = new UserDto { Id = "1", Email = email };

            _userServiceMock.Setup(x => x.GetUserByEmail(email)).ReturnsAsync(user);
            _mapperMock.Setup(x => x.Map<UserDto>(user)).Returns(userDto);

            // Act
            var result = await _controller.GetUserByEmail(email);

            // Assert
            result.Value.Should().BeEquivalentTo(userDto);
        }
    }
}
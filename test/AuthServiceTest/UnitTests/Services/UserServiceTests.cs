using Auth.API.Entities;
using Auth.API.Services.Behaviours;
using AuthService.Repositories.Behaviours;
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
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILogger<UserRepository>> _loggerMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<UserRepository>>();
            _userService = new UserService(_userRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void FetchUsers_ShouldThrowNotImplementedException()
        {
            // Act
            Action act = () => _userService.FetchUsers();

            // Assert
            act.Should().Throw<NotImplementedException>();
        }

        [Fact]
        public async Task GetRoleById_ShouldReturnRole_WhenRoleExists()
        {
            // Arrange
            var roleId = "admin";
            var expectedRole = new UserRole { Id = roleId, RoleName = "Admin" };

            _userRepositoryMock.Setup(repo => repo.GetRoleById(roleId)).ReturnsAsync(expectedRole);

            // Act
            var result = await _userService.GetRoleById(roleId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedRole);
        }

        [Fact]
        public async Task GetRoleById_ShouldReturnNull_WhenRoleDoesNotExist()
        {
            // Arrange
            var roleId = "unknown";

            _userRepositoryMock.Setup(repo => repo.GetRoleById(roleId)).ReturnsAsync((UserRole)null);

            // Act
            var result = await _userService.GetRoleById(roleId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetStaffByUserId_ShouldReturnStaffInfo_WhenUserExists()
        {
            // Arrange
            var userId = "123";
            var expectedStaffInfo = new StaffInfo { UserId = userId, Title = "Manager" };

            _userRepositoryMock.Setup(repo => repo.GetStaffByUserId(userId)).ReturnsAsync(expectedStaffInfo);

            // Act
            var result = await _userService.GetStaffByUserId(userId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedStaffInfo);
        }

        [Fact]
        public async Task GetStaffByUserId_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "unknown";

            _userRepositoryMock.Setup(repo => repo.GetStaffByUserId(userId)).ReturnsAsync((StaffInfo)null);

            // Act
            var result = await _userService.GetStaffByUserId(userId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetUserByEmail_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var email = "test@example.com";
            var expectedUser = new User { Id = "1", Email = email, FullName = "Test User" };

            _userRepositoryMock.Setup(repo => repo.GetUserByEmail(email)).ReturnsAsync(expectedUser);

            // Act
            var result = await _userService.GetUserByEmail(email);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedUser);
        }

        [Fact]
        public async Task GetUserByEmail_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var email = "notfound@example.com";

            _userRepositoryMock.Setup(repo => repo.GetUserByEmail(email)).ReturnsAsync((User)null);

            // Act
            var result = await _userService.GetUserByEmail(email);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnTrue_WhenUpdateIsSuccessful()
        {
            // Arrange
            var user = new User { Id = "1", Email = "updated@example.com" };

            _userRepositoryMock.Setup(repo => repo.UpdateUser(user)).ReturnsAsync(true);

            // Act
            var result = await _userService.UpdateUser(user);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnFalse_WhenUpdateFails()
        {
            // Arrange
            var user = new User { Id = "1", Email = "updated@example.com" };

            _userRepositoryMock.Setup(repo => repo.UpdateUser(user)).ReturnsAsync(false);

            // Act
            var result = await _userService.UpdateUser(user);

            // Assert
            result.Should().BeFalse();
        }

        //[Fact]
        //public async Task GetRoleById_ShouldThrowArgumentNullException_WhenRoleIdIsNull()
        //{
        //    // Arrange
        //    string roleId = null;

        //    // Act
        //    Func<Task> act = async () => await _userService.GetRoleById(roleId);

        //    // Assert
        //    await act.Should().ThrowAsync<ArgumentNullException>();
        //}

        //[Fact]
        //public async Task GetStaffByUserId_ShouldThrowArgumentNullException_WhenUserIdIsNull()
        //{
        //    // Arrange
        //    string userId = null;

        //    // Act
        //    Func<Task> act = async () => await _userService.GetStaffByUserId(userId);

        //    // Assert
        //    await act.Should().ThrowAsync<ArgumentNullException>();
        //}

        //[Fact]
        //public async Task GetUserByEmail_ShouldThrowArgumentNullException_WhenEmailIsNull()
        //{
        //    // Arrange
        //    string email = null;

        //    // Act
        //    Func<Task> act = async () => await _userService.GetUserByEmail(email);

        //    // Assert
        //    await act.Should().ThrowAsync<ArgumentNullException>();
        //}

        //[Fact]
        //public async Task UpdateUser_ShouldThrowArgumentNullException_WhenUserIsNull()
        //{
        //    // Arrange
        //    User user = null;

        //    // Act
        //    Func<Task> act = async () => await _userService.UpdateUser(user);

        //    // Assert
        //    await act.Should().ThrowAsync<ArgumentNullException>();
        //}

        [Fact]
        public async Task GetRoleById_ShouldReturnNull_WhenRoleIdIsEmpty()
        {
            // Arrange
            var roleId = string.Empty;

            _userRepositoryMock.Setup(repo => repo.GetRoleById(roleId)).ReturnsAsync((UserRole)null);

            // Act
            var result = await _userService.GetRoleById(roleId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetUserByEmail_ShouldReturnNull_WhenEmailIsEmpty()
        {
            // Arrange
            var email = string.Empty;

            _userRepositoryMock.Setup(repo => repo.GetUserByEmail(email)).ReturnsAsync((User)null);

            // Act
            var result = await _userService.GetUserByEmail(email);

            // Assert
            result.Should().BeNull();
        }
    }
}

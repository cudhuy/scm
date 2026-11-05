using Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.API.Dtos;
using UserManagement.API.Dtos.Notification;
using UserManagement.API.Repositories.Interfaces;
using UserManagement.API.Services.Behaviours;
using UserManagement.API.Services.Interfaces;

namespace UserManagementTest.UnitTests.Services
{
    public class RankUpdateServiceTests
    {
        private readonly Mock<IMembershipRanksRepository> _rankRepoMock;
        private readonly Mock<ILogger<RankUpdateService>> _loggerMock;
        private readonly Mock<INotificationsService> _notificationServiceMock;
        private readonly RankUpdateService _service;

        public RankUpdateServiceTests()
        {
            _rankRepoMock = new Mock<IMembershipRanksRepository>();
            _loggerMock = new Mock<ILogger<RankUpdateService>>();
            _notificationServiceMock = new Mock<INotificationsService>();
            _service = new RankUpdateService(_rankRepoMock.Object, _loggerMock.Object, _notificationServiceMock.Object);
        }

        //[Fact]
        //public async Task UpdateUserRanksAsync_ShouldUpdateRankAndSendNotification_WhenUserEligibleForHigherRank()
        //{
        //    var userId = Guid.NewGuid();
        //    var users = new List<User> { new User { Id = userId } };
        //    var newRank = new MembershipRanks { Id = Guid.NewGuid(), Name = "Gold" };

        //    _rankRepoMock.Setup(r => r.GetUsersWithOrdersAsync()).ReturnsAsync(users);
        //    _rankRepoMock.Setup(r => r.GetTotalAmountByUserIdAsync(userId)).ReturnsAsync(500);
        //    _rankRepoMock.Setup(r => r.GetHighestEligibleRankAsync(500)).ReturnsAsync(newRank);
        //    _rankRepoMock.Setup(r => r.UpdateUserRankAsync(userId, Guid.NewGuid())).Returns(Task.CompletedTask);
        //    _rankRepoMock.Setup(r => r.GetUserEmailByIdAsync(userId)).ReturnsAsync("test@example.com");
        //    _notificationServiceMock.Setup(n => n.CreateNotificationUser(It.IsAny<CreateNotificationUserDto>())).ReturnsAsync(true);

        //    await _service.UpdateUserRanksAsync();

        //    _rankRepoMock.Verify(r => r.UpdateUserRankAsync(userId, Guid.NewGuid()), Times.Once);
        //    _notificationServiceMock.Verify(n => n.CreateNotificationUser(It.IsAny<CreateNotificationUserDto>()), Times.Once);
        //}

        //[Fact]
        //public async Task UpdateUserRanksAsync_ShouldSkipUser_WhenAlreadyHasHighestRank()
        //{
        //    var userId = Guid.NewGuid();
        //    var users = new List<User> { new User { Id = userId } };
        //    var rank = new MembershipRanks { Id = Guid.NewGuid(), Name = "Gold" };

        //    _rankRepoMock.Setup(r => r.GetUsersWithOrdersAsync()).ReturnsAsync(users);
        //    _rankRepoMock.Setup(r => r.GetTotalAmountByUserIdAsync(userId)).ReturnsAsync(500);
        //    _rankRepoMock.Setup(r => r.GetHighestEligibleRankAsync(500)).ReturnsAsync(rank);

        //    await _service.UpdateUserRanksAsync();

        //    _rankRepoMock.Verify(r => r.UpdateUserRankAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        //    _notificationServiceMock.Verify(n => n.CreateNotificationUser(It.IsAny<CreateNotificationUserDto>()), Times.Never);
        //}

        [Fact]
        public async Task UpdateUserRanksAsync_ShouldReturn_WhenNoUsersFound()
        {
            _rankRepoMock.Setup(r => r.GetUsersWithOrdersAsync()).ReturnsAsync(new List<User>());

            await _service.UpdateUserRanksAsync();

            _rankRepoMock.Verify(r => r.GetUsersWithOrdersAsync(), Times.Once);
            _rankRepoMock.VerifyNoOtherCalls();
            _notificationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task UpdateUserRanksAsync_ShouldHandleExceptionAndContinue_WhenUserProcessingFails()
        {
            var userId = Guid.NewGuid();
            var users = new List<User> { new User { Id = userId } };

            _rankRepoMock.Setup(r => r.GetUsersWithOrdersAsync()).ReturnsAsync(users);
            _rankRepoMock.Setup(r => r.GetTotalAmountByUserIdAsync(userId)).ThrowsAsync(new Exception("Test error"));

            await _service.UpdateUserRanksAsync();

            //_loggerMock.VerifyLog(LogLevel.Error, Times.AtLeastOnce());
        }

        [Fact]
        public async Task UpdateUserRanksAsync_ShouldNotSendNotification_WhenEmailIsEmpty()
        {
            var userId = Guid.NewGuid();
            var users = new List<User> { new User { Id = userId } };
            var newRank = new MembershipRanks { Id = Guid.NewGuid(), Name = "Gold" };

            _rankRepoMock.Setup(r => r.GetUsersWithOrdersAsync()).ReturnsAsync(users);
            _rankRepoMock.Setup(r => r.GetTotalAmountByUserIdAsync(userId)).ReturnsAsync(500);
            _rankRepoMock.Setup(r => r.GetHighestEligibleRankAsync(500)).ReturnsAsync(newRank);
            _rankRepoMock.Setup(r => r.UpdateUserRankAsync(userId, Guid.NewGuid())).Returns(Task.CompletedTask);
            _rankRepoMock.Setup(r => r.GetUserEmailByIdAsync(userId)).ReturnsAsync(string.Empty);

            await _service.UpdateUserRanksAsync();

            _notificationServiceMock.Verify(n => n.CreateNotificationUser(It.IsAny<CreateNotificationUserDto>()), Times.Never);
        }
    }
}

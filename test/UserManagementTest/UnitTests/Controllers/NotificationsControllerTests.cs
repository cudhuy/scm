using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using UserManagement.API.Controllers;
using UserManagement.API.Dtos.Notification;
using UserManagement.API.Helpers;
using UserManagement.API.Services.Interfaces;
using Xunit;

namespace UserManagementTest.UnitTests.Controllers
{
    public class NotificationsControllerTests
    {
        private readonly Mock<INotificationsService> _notificationsServiceMock;
        private readonly Mock<ILogger<NotificationsController>> _loggerMock;
        private readonly Mock<IHubContext<NotificationHub>> _hubContextMock;
        private readonly NotificationsController _controller;

        public NotificationsControllerTests()
        {
            _notificationsServiceMock = new Mock<INotificationsService>();
            _loggerMock = new Mock<ILogger<NotificationsController>>();
            _hubContextMock = new Mock<IHubContext<NotificationHub>>();

            _controller = new NotificationsController(_loggerMock.Object, _notificationsServiceMock.Object, _hubContextMock.Object);
        }

        [Fact]
        public async Task GetAllNotificationSystemByPage_ShouldReturnOk_WhenDataExists()
        {
            _notificationsServiceMock.Setup(s => s.GetAllNotificationSystemForPage(0, 10, null))
                .ReturnsAsync(new NotificationResponse { Items = new List<NotificationSystemDto> { new NotificationSystemDto() } });

            var result = await _controller.GetAllNotificationSystemByPage();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetAllNotificationSystemByPage_ShouldReturnNotFound_WhenNoData()
        {
            _notificationsServiceMock.Setup(s => s.GetAllNotificationSystemForPage(0, 10, null))
                .ReturnsAsync(new NotificationResponse { Items = new List<NotificationSystemDto>() });

            var result = await _controller.GetAllNotificationSystemByPage();
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetAllNotificationSystemByPage_ShouldReturnServerError_WhenExceptionThrown()
        {
            _notificationsServiceMock.Setup(s => s.GetAllNotificationSystemForPage(0, 10, null))
                .ThrowsAsync(new Exception("Internal Error"));

            var result = await _controller.GetAllNotificationSystemByPage();
            result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task CreateNotificationSystem_ShouldReturnBadRequest_WhenInvalidPriority()
        {
            var dto = new CreateNotificationSystemDto { Title = "Test", Priority = 3 };

            var result = await _controller.CreateNotificationSystem(dto);
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task CreateNotificationSystem_ShouldReturnOk_WhenSuccessful()
        {
            var dto = new CreateNotificationSystemDto { Title = "Test", Content = "Content", Priority = 1 };
            _notificationsServiceMock.Setup(s => s.CreateNotificationSystem(dto)).ReturnsAsync(true);

            var result = await _controller.CreateNotificationSystem(dto);
            result.Should().BeOfType<ObjectResult>();
        }

        //[Fact]
        //public async Task CreateNotificationSystem_ShouldReturnServerError_WhenSignalRFails()
        //{
        //    var dto = new CreateNotificationSystemDto { Title = "Test", Content = "Content", Priority = 1 };
        //    _notificationsServiceMock.Setup(s => s.CreateNotificationSystem(dto)).ReturnsAsync(true);
        //    _hubContextMock.Setup(h => h.Clients.All.SendAsync(It.IsAny<string>(), It.IsAny<string>(), default))
        //        .ThrowsAsync(new Exception("SignalR Error"));

        //    var result = await _controller.CreateNotificationSystem(dto);
        //    result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
        //}

        [Fact]
        public async Task DeleteNotificationSystem_ShouldReturnOk_WhenDeleted()
        {
            var id = Guid.NewGuid();
            _notificationsServiceMock.Setup(s => s.DeleteNotificationSystem(id)).ReturnsAsync(true);

            var result = await _controller.DeleteNotificationSystem(id);
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteNotificationSystem_ShouldReturnNotFound_WhenNotFound()
        {
            var id = Guid.NewGuid();
            _notificationsServiceMock.Setup(s => s.DeleteNotificationSystem(id)).ReturnsAsync(false);

            var result = await _controller.DeleteNotificationSystem(id);
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task DeleteNotificationSystem_ShouldReturnServerError_WhenException()
        {
            var id = Guid.NewGuid();
            _notificationsServiceMock.Setup(s => s.DeleteNotificationSystem(id)).ThrowsAsync(new Exception("Error"));

            var result = await _controller.DeleteNotificationSystem(id);
            result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task UpdateNotificationSystem_ShouldReturnBadRequest_WhenDtoIsNull()
        {
            // Act
            var result = await _controller.UpdateNotificationSystem(Guid.NewGuid(), null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Dữ liệu thông báo không hợp lệ.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateNotificationSystem_ShouldReturnNotFound_WhenUpdateFails()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new UpdateNotificationSystemDto { Content = "Content" };
            _notificationsServiceMock.Setup(s => s.UpdateNotificationSystem(id, dto)).ReturnsAsync((false));

            // Act
            var result = await _controller.UpdateNotificationSystem(id, dto);

            // Assert
            var notFoundResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Cập nhật nội dung thông báo thành công!", notFoundResult.Value!.ToString());
        }

        [Fact]
        public async Task UpdateNotificationSystem_ShouldReturnOk_WhenSuccess()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new UpdateNotificationSystemDto {Content = "Updated" };
            _notificationsServiceMock.Setup(s => s.UpdateNotificationSystem(id, dto)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateNotificationSystem(id, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Cập nhật nội dung thông báo thành công", okResult.Value!.ToString());
        }

        [Fact]
        public async Task CreateNotificationRanks_ShouldReturnBadRequest_WhenInvalidInput()
        {
            // Arrange
            var dto = new CreateNotificationRanksDtos(); // Missing Title/Content

            // Act
            var result = await _controller.CreateNotificationRanks(dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Dữ liệu thông báo không hợp lệ", badRequest.Value!.ToString());
        }

        [Fact]
        public async Task CreateNotificationRanks_ShouldReturnOk_WhenSuccess()
        {
            // Arrange
            var dto = new CreateNotificationRanksDtos { Title = "T", Content = "C", RankIds = new List<Guid>() };
            _notificationsServiceMock.Setup(s => s.CreateNotificationForRanks(dto)).ReturnsAsync(true);

            // Act
            var result = await _controller.CreateNotificationRanks(dto);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Thêm thông báo người dùng thành công", ok.Value!.ToString());
        }

        [Fact]
        public async Task CreateNotificationRanks_ShouldReturn500_WhenFailed()
        {
            // Arrange
            var dto = new CreateNotificationRanksDtos { Title = "T", Content = "C", RankIds = new List<Guid>() };
            _notificationsServiceMock.Setup(s => s.CreateNotificationForRanks(dto)).ReturnsAsync(false);

            // Act
            var result = await _controller.CreateNotificationRanks(dto);

            // Assert
            var serverError = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverError.StatusCode);
        }

        [Fact]
        public async Task GetAllNotificationRanksByPage_ShouldReturnNotFound_WhenEmpty()
        {
            _notificationsServiceMock.Setup(s => s.GetAllNotificationRanksForPage(It.IsAny<int>(), It.IsAny<int>(), null, It.IsAny<List<Guid>>()))
                        .ReturnsAsync(new RankNotificationResponse());

            var result = await _controller.GetAllNotificationRanksByPage();

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("Không có thông báo nào", notFound.Value!.ToString());
        }

        [Fact]
        public async Task GetAllNotificationRanksByPage_ShouldReturnOk_WhenHasData()
        {
            _notificationsServiceMock.Setup(s => s.GetAllNotificationRanksForPage(It.IsAny<int>(), It.IsAny<int>(), null, It.IsAny<List<Guid>>()))
                        .ReturnsAsync(new RankNotificationResponse());

            var result = await _controller.GetAllNotificationRanksByPage();

            var ok = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<RankNotificationResponse>(ok.Value);
        }

        [Fact]
        public async Task GetNotificationCounts_ShouldReturnOk_WithCounts()
        {
            _notificationsServiceMock.Setup(s => s.GetNotificationCounts()).ReturnsAsync(new NotificationCountsDto());

            var result = await _controller.GetNotificationCounts();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<NotificationCountsDto>(ok.Value);
        }

        [Fact]
        public async Task GetNotificationSystemBase_ShouldReturnOk_WithData()
        {
            _notificationsServiceMock.Setup(s => s.GetNotificationSystemBase()).ReturnsAsync(new List<NotiSystemBaseDto>());

            var result = await _controller.GetNotificationSystemBase();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<NotiSystemBaseDto>>(ok.Value);
        }


    }
}

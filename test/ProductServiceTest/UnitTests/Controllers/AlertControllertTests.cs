using Microsoft.AspNetCore.Mvc;
using Moq;
using Product.API.Controllers;
using Product.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductServiceTest.UnitTests.Controllers
{
    public class AlertControllerTests
    {
        private readonly Mock<IAlertService> _mockAlertService;
        private readonly AlertController _controller;

        public AlertControllerTests()
        {
            _mockAlertService = new Mock<IAlertService>();
            _controller = new AlertController(_mockAlertService.Object);
        }

        [Fact]
        public async Task SendAlerts_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            _mockAlertService.Setup(s => s.CheckAndSendAlerts()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.SendAlerts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task SendAlerts_ThrowsException_Returns500()
        {
            // Arrange
            _mockAlertService.Setup(s => s.CheckAndSendAlerts()).ThrowsAsync(new System.Exception("Email failed"));

            // Act
            var controller = new AlertController(_mockAlertService.Object);
            var result = await Record.ExceptionAsync(() => controller.SendAlerts());

            // Assert
            Assert.NotNull(result);
            Assert.IsType<System.Exception>(result);
            Assert.Equal("Email failed", result.Message);
        }
    }
}

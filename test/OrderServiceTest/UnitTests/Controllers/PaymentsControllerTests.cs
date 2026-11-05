using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Order.API.Dtos.Sepay;
using Order.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderServiceTest.UnitTests.Controllers
{
    public class PaymentsControllerTests
    {
        private readonly Mock<IPaymentsService> _paymentsServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly PaymentsController _controller;

        public PaymentsControllerTests()
        {
            _paymentsServiceMock = new Mock<IPaymentsService>();
            _configurationMock = new Mock<IConfiguration>();
            _controller = new PaymentsController(_paymentsServiceMock.Object, _configurationMock.Object);
        }

        [Fact]
        public async Task GenerateQR_ReturnsFileResult_WhenQRGenerated()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var fakeQrData = new byte[] { 1, 2, 3 };
            _paymentsServiceMock.Setup(p => p.GenerateQR(orderId)).ReturnsAsync(fakeQrData);

            // Act
            var result = await _controller.GenerateQR(orderId);

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("image/png", fileResult.ContentType);
            Assert.Equal(fakeQrData, fileResult.FileContents);
        }

        [Fact]
        public async Task GenerateQR_ReturnsBadRequest_WhenQRIsNull()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _paymentsServiceMock.Setup(p => p.GenerateQR(orderId)).ReturnsAsync((byte[])null);

            // Act
            var result = await _controller.GenerateQR(orderId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Không thể tạo mã QR", badRequestResult.Value);
        }

        [Fact]
        public async Task ReceiveSepayWebhook_ReturnsUnauthorized_WhenApiKeyMissing()
        {
            // Arrange
            var context = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext = context;

            // Act
            var result = await _controller.ReceiveSepayWebhook(new SepayWebhookRequestDto());

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Thiếu API Key", unauthorized.Value);
        }

        [Fact]
        public async Task ReceiveSepayWebhook_ReturnsUnauthorized_WhenApiKeyInvalid()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Apikey invalid-key";
            _controller.ControllerContext.HttpContext = context;

            _configurationMock.Setup(c => c["SepayConfig:ApiKey"]).Returns("real-key");

            // Act
            var result = await _controller.ReceiveSepayWebhook(new SepayWebhookRequestDto());

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("API Key không hợp lệ", unauthorized.Value);
        }

        [Fact]
        public async Task ReceiveSepayWebhook_ReturnsOk_WhenApiKeyValid()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Apikey real-key";
            _controller.ControllerContext.HttpContext = context;

            var webhookRequest = new SepayWebhookRequestDto(); // có thể setup thêm nếu cần
            _configurationMock.Setup(c => c["SepayConfig:ApiKey"]).Returns("real-key");

            // Act
            var result = await _controller.ReceiveSepayWebhook(webhookRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True(true);

            _paymentsServiceMock.Verify(p => p.CheckPayment(webhookRequest), Times.Once);
        }
    }
}

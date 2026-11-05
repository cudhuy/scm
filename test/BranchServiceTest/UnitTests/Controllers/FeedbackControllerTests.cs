using Branch.API.Controllers;
using Branch.API.Dtos.Feedback;
using Branch.API.Services.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchServiceTest.UnitTests.Controllers
{
    public class FeedbackControllerTests
    {
        private readonly Mock<IFeedbackService> _mockFeedbackService;
        private readonly FeedbackController _controller;

        public FeedbackControllerTests()
        {
            _mockFeedbackService = new Mock<IFeedbackService>();
            _controller = new FeedbackController(_mockFeedbackService.Object);
        }

        [Fact]
        public async Task CreateFeedback_ReturnsBadRequest_WhenDtoIsNullOrInvalid()
        {
            var dto = new CreateFeedbackDto(); // thiếu dữ liệu bắt buộc
            _controller.ModelState.AddModelError("UserId", "Required");

            var result = await _controller.CreateFeedback(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);
        }

        [Fact]
        public async Task CreateFeedback_ReturnsBadRequest_WhenMissingFields()
        {
            var dto = new CreateFeedbackDto
            {
                UserId = null,
                OrderId = null,
                Rating = null,
                Content = null
            };

            var result = await _controller.CreateFeedback(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);
        }

        [Fact]
        public async Task CreateFeedback_ReturnsBadRequest_WhenServiceFails()
        {
            var dto = new CreateFeedbackDto
            {
                UserId = Guid.NewGuid(),
                OrderId = Guid.NewGuid(),
                Rating = 4,
                Content = "Good"
            };

            _mockFeedbackService.Setup(s => s.CreateFeedbackAsync(dto)).ReturnsAsync( ( false, "Lỗi hệ thống" ) );

            var result = await _controller.CreateFeedback(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);
        }

        [Fact]
        public async Task CreateFeedback_ReturnsOk_WhenSuccessful()
        {
            var dto = new CreateFeedbackDto
            {
                UserId = Guid.NewGuid(),
                OrderId = Guid.NewGuid(),
                Rating = 5,
                Content = "Excellent"
            };

            _mockFeedbackService.Setup(s => s.CreateFeedbackAsync(dto)).ReturnsAsync((true, null));

            var result = await _controller.CreateFeedback(dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, ok.StatusCode);
        }

        [Fact]
        public async Task GetAllFeedbacks_ReturnsNotFound_WhenEmpty()
        {
            _mockFeedbackService.Setup(s => s.GetAllFeedbacksAsync(1, 10, Guid.NewGuid())).ReturnsAsync(new FeedbackResponse());

            var result = await _controller.GetAllFeedbacks();

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFound.StatusCode);
        }

        [Fact]
        public async Task GetAllFeedbacks_ReturnsOk_WhenHasData()
        {
            var data = new List<FeedbackDto> { new FeedbackDto(), new FeedbackDto() };
            _mockFeedbackService.Setup(s => s.GetAllFeedbacksAsync(1, 10, null)).ReturnsAsync(new FeedbackResponse());

            var result = await _controller.GetAllFeedbacks();

            var ok = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, ok.StatusCode);
        }

        [Fact]
        public async Task GetAllFeedbacks_ReturnsBadRequest_WhenExceptionThrown()
        {
            _mockFeedbackService.Setup(s => s.GetAllFeedbacksAsync(1, 10, null)).ThrowsAsync(new Exception("Lỗi nè"));

            var result = await _controller.GetAllFeedbacks();

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);
        }
    }
}

using AutoMapper;
using Branch.API.Dtos.Feedback;
using Branch.API.Services.Behaviours;
using Branch.API.Services.Interfaces;
using Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchServiceTest.UnitTests.Services
{
    public class FeedbackServiceTests
    {
        private readonly Mock<IFeedbackRepository> _feedbackRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<FeedbackService>> _loggerMock;
        private readonly FeedbackService _feedbackService;

        public FeedbackServiceTests()
        {
            _feedbackRepoMock = new Mock<IFeedbackRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<FeedbackService>>();
            _feedbackService = new FeedbackService(_feedbackRepoMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        //[Fact]
        //public async Task CreateFeedbackAsync_ShouldReturnFalse_WhenFeedbackDtoIsInvalid()
        //{
        //    // Arrange
        //    var dto = new CreateFeedbackDto();

        //    // Act
        //    var result = await _feedbackService.CreateFeedbackAsync(dto);

        //    // Assert
        //    result.Success.Should().BeFalse();
        //    result.Message.Should().Be("Dữ liệu yêu cầu không hợp lệ.");
        //}

        [Fact]
        public async Task CreateFeedbackAsync_ShouldReturnFalse_WhenOrderDoesNotExist()
        {
            // Arrange
            var dto = new CreateFeedbackDto
            {
                UserId = Guid.NewGuid(),
                OrderId = Guid.NewGuid(),
                Rating = 5,
                Content = "Good"
            };

            _feedbackRepoMock.Setup(x => x.GetOrderByIdAsync(dto.OrderId.Value, dto.UserId.Value))
                             .ReturnsAsync((Order)null);

            // Act
            var result = await _feedbackService.CreateFeedbackAsync(dto);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Đơn hàng khôgn tồn tại");
        }

        [Fact]
        public async Task CreateFeedbackAsync_ShouldReturnFalse_WhenOrderNotCompleted()
        {
            // Arrange
            var dto = new CreateFeedbackDto
            {
                UserId = Guid.NewGuid(),
                OrderId = Guid.NewGuid(),
                Rating = 4,
                Content = "Not done yet"
            };

            var order = new Order { Status = 1 };

            _feedbackRepoMock.Setup(x => x.GetOrderByIdAsync(dto.OrderId.Value, dto.UserId.Value))
                             .ReturnsAsync(order);

            // Act
            var result = await _feedbackService.CreateFeedbackAsync(dto);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Đơn hàng chưa hoàn thành");
        }

        [Fact]
        public async Task CreateFeedbackAsync_ShouldReturnFalse_WhenFeedbackAlreadyExists()
        {
            // Arrange
            var dto = new CreateFeedbackDto
            {
                UserId = Guid.NewGuid(),
                OrderId = Guid.NewGuid(),
                Rating = 5,
                Content = "Already reviewed"
            };

            var order = new Order { Status = 2, BranchId = Guid.NewGuid() };

            _feedbackRepoMock.Setup(x => x.GetOrderByIdAsync(dto.OrderId.Value, dto.UserId.Value))
                             .ReturnsAsync(order);

            _feedbackRepoMock.Setup(x => x.FeedbackExistsForOrderAsync(dto.OrderId.Value))
                             .ReturnsAsync(true);

            // Act
            var result = await _feedbackService.CreateFeedbackAsync(dto);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Đã có phản hồi");
        }

        [Fact]
        public async Task CreateFeedbackAsync_ShouldReturnTrue_WhenValid()
        {
            // Arrange
            var dto = new CreateFeedbackDto
            {
                UserId = Guid.NewGuid(),
                OrderId = Guid.NewGuid(),
                Rating = 5,
                Content = "All good"
            };

            var order = new Order { Status = 2, BranchId = Guid.NewGuid() };

            _feedbackRepoMock.Setup(x => x.GetOrderByIdAsync(dto.OrderId.Value, dto.UserId.Value))
                             .ReturnsAsync(order);

            _feedbackRepoMock.Setup(x => x.FeedbackExistsForOrderAsync(dto.OrderId.Value))
                             .ReturnsAsync(false);

            _feedbackRepoMock.Setup(x => x.AddFeedbackAsync(It.IsAny<Feedback>()))
                             .Returns(Task.CompletedTask);

            // Act
            var result = await _feedbackService.CreateFeedbackAsync(dto);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Contain("thành công");
        }

        [Fact]
        public async Task GetAllFeedbacksAsync_ShouldReturnPaginatedResult()
        {
            // Arrange
            var branchId = Guid.NewGuid();
            var order = new Order
            {
                User = new User { FullName = "John Doe" },
                OrderCode = "ORD001",
                Branch = new Domain.Entities.Branch { Name = "Branch A" }
            };

            var feedbacks = new List<Feedback>
        {
            new Feedback { Order = order, Rating = 4, Content = "Nice", CreatedAt = DateTime.UtcNow },
            new Feedback { Order = order, Rating = 5, Content = "Great", CreatedAt = DateTime.UtcNow }
        };

            _feedbackRepoMock.Setup(x => x.GetAllFeedbacksAsync(branchId))
                             .ReturnsAsync(feedbacks);

            // Act
            var result = await _feedbackService.GetAllFeedbacksAsync(1, 1, branchId);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(1);
            result.TotalItems.Should().Be(2);
            result.TotalPages.Should().Be(2);
        }

        [Fact]
        public async Task GetAllFeedbacksAsync_ShouldReturnNull_WhenNoFeedback()
        {
            _feedbackRepoMock.Setup(x => x.GetAllFeedbacksAsync(It.IsAny<Guid?>()))
                             .ReturnsAsync(new List<Feedback>());

            var result = await _feedbackService.GetAllFeedbacksAsync(1, 10, null);

            result.Should().BeNull();
        }
    }
}

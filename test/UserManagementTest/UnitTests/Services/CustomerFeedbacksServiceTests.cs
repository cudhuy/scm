using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.API.Dtos.FeedbackCustomer;
using UserManagement.API.Repositories.Interfaces;
using UserManagement.API.Services.Behaviours;
using Xunit;

namespace UserManagementTest.UnitTests.Services
{
    public class CustomerFeedbacksServiceTests
    {
        private readonly Mock<ICustomerFeedbacksRepository> _mockRepo;
        private readonly Mock<ILogger<CustomerFeedbacksService>> _mockLogger;
        private readonly IMapper _mapper;
        private readonly CustomerFeedbacksService _service;

        public CustomerFeedbacksServiceTests()
        {
            _mockRepo = new Mock<ICustomerFeedbacksRepository>();
            _mockLogger = new Mock<ILogger<CustomerFeedbacksService>>();

            var config = new MapperConfiguration(cfg => { });
            _mapper = config.CreateMapper();

            _service = new CustomerFeedbacksService(_mockLogger.Object, _mapper, _mockRepo.Object);
        }

        //[Fact]
        //public async Task GetAllFeedbackCustomer_ShouldReturnGroupedFeedbacks_WhenFeedbacksExist()
        //{
        //    // Arrange
        //    var userId = Guid.NewGuid();
        //    var feedbacks = new List<Feedback>
        //    {
        //        new Feedback
        //        {
        //            UserId = Guid.NewGuid(),
        //            User = new User
        //            {
        //                Id = userId,
        //                FullName = "John Doe",
        //                Email = "john@example.com",
        //                Avatar = "avatar.png",
        //                Phone = "123456789",
        //                Address = "123 Street",
        //                Status = "Active",
        //                Ranks = new MembershipRanks { Name = "Gold" }
        //            }
        //        }
        //    };

        //    _mockRepo.Setup(repo => repo.GetAllFeedbackCustomer()).ReturnsAsync(feedbacks);

        //    // Act
        //    var result = await _service.GetAllFeedbackCustomer();

        //    // Assert
        //    Assert.Single(result);
        //    var group = result.First();
        //    Assert.Equal(userId, group.UserId);
        //    Assert.Single(group.Feedbacks);
        //    Assert.Equal("John Doe", group.FullName);
        //}

        [Fact]
        public async Task GetAllFeedbackCustomer_ShouldReturnEmpty_WhenNoFeedbacks()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetAllFeedbackCustomer()).ReturnsAsync(new List<Feedback>());

            // Act
            var result = await _service.GetAllFeedbackCustomer();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllFeedbackCustomer_ShouldThrowException_WhenRepositoryFails()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetAllFeedbackCustomer()).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.GetAllFeedbackCustomer());
            Assert.Equal("Database error", ex.Message);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error in service layer")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}

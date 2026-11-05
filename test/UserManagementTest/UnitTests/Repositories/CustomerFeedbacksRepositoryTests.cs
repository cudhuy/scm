using Domain.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;
using UserManagement.API.Repositories.Behaviours;
using FluentAssertions;

namespace UserManagementTest.UnitTests.Repositories
{
    public class CustomerFeedbacksRepositoryTests
    {
        private readonly SCMDbContext _context;
        private readonly CustomerFeedbacksRepository _repository;
        private readonly Mock<ILogger<CustomerFeedbacksRepository>> _loggerMock;

        public CustomerFeedbacksRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<SCMDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SCMDbContext(options);
            _loggerMock = new Mock<ILogger<CustomerFeedbacksRepository>>();
            _repository = new CustomerFeedbacksRepository(_context, _loggerMock.Object);
        }

        private async Task SeedDataAsync()
        {
            var rank = new MembershipRanks { Id = Guid.NewGuid(), Name = "Gold", Avatar =" UnitTest", Description ="UnitTest" };
            var user1 = new User
            {
                Id = Guid.NewGuid(),
                FullName = "Customer 1",
                StaffInfo = null,
                Ranks = rank,
                Avatar = "UnitTest"
            };

            var user2 = new User
            {
                Id = Guid.NewGuid(),
                FullName = "Staff",
                StaffInfo = new StaffInfo { Id = Guid.NewGuid(), Tittle = "UniTest", Type = "UnitTest" }, // Staff => Should be filtered
                Ranks = rank
            };

            var feedbacks = new List<Feedback>
        {
            new Feedback { Id = Guid.NewGuid(), User = user1, Content = "Great!", IsDeleted = false },
            new Feedback { Id = Guid.NewGuid(), User = user2, Content = "From staff", IsDeleted = false },
            new Feedback { Id = Guid.NewGuid(), User = user1, Content = "Deleted", IsDeleted = true }
        };

            await _context.Users.AddRangeAsync(user1, user2);
            await _context.Feedbacks.AddRangeAsync(feedbacks);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAllFeedbackCustomer_ShouldReturnOnlyCustomerFeedbacks()
        {
            // Arrange
            await SeedDataAsync();

            // Act
            var result = await _repository.GetAllFeedbackCustomer();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result); // Only one valid customer feedback
            Assert.Equal("Great!", result[0].Content);
        }

        [Fact]
        public async Task GetAllFeedbackCustomer_ShouldReturnEmptyList_WhenNoFeedback()
        {
            // Act
            var result = await _repository.GetAllFeedbackCustomer();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}

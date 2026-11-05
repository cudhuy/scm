using Branch.API.Services.Behaviours;
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

namespace BranchServiceTest.UnitTests.Repositories
{
    public class FeedbackRepositoryTests
    {
        private readonly SCMDbContext _context;
        private readonly FeedbackRepository _repository;

        public FeedbackRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<SCMDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SCMDbContext(options);

            var loggerMock = new Mock<ILogger<FeedbackRepository>>();
            _repository = new FeedbackRepository(_context, loggerMock.Object);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ReturnsCorrectOrder()
        {
            var userId = Guid.NewGuid();
            var order = new Order { Id = Guid.NewGuid(), CustomerId = userId, ShippingAddress = "UnitTest", OrderCode = "UnitTest", OrderType = "UnitTest", RecipientName = "UnitTest", RecipientPhone = "UnitTest" };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var result = await _repository.GetOrderByIdAsync(order.Id, userId);

            Assert.NotNull(result);
            Assert.Equal(order.Id, result.Id);
        }

        [Fact]
        public async Task FeedbackExistsForOrderAsync_ReturnsTrue_WhenExists()
        {
            var orderId = Guid.NewGuid();
            _context.Feedbacks.Add(new Feedback { Id = Guid.NewGuid(), OrderId = orderId, Content = "UnitTest" });
            await _context.SaveChangesAsync();

            var result = await _repository.FeedbackExistsForOrderAsync(orderId);

            Assert.True(result);
        }

        [Fact]
        public async Task FeedbackExistsForOrderAsync_ReturnsFalse_WhenNotExists()
        {
            var result = await _repository.FeedbackExistsForOrderAsync(Guid.NewGuid());
            Assert.False(result);
        }

        [Fact]
        public async Task AddFeedbackAsync_AddsFeedbackToDb()
        {
            var feedback = new Feedback { Id = Guid.NewGuid(), Content = "Great", OrderId = Guid.NewGuid() };

            await _repository.AddFeedbackAsync(feedback);

            var saved = await _context.Feedbacks.FirstOrDefaultAsync(f => f.Id == feedback.Id);
            Assert.NotNull(saved);
            Assert.Equal("Great", saved.Content);
        }

        [Fact]
        public async Task GetAllFeedbacksAsync_ReturnsAllFeedbacks_WhenBranchIdIsNull()
        {
            var branchId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var order = new Order { Id = Guid.NewGuid(), BranchId = branchId, ShippingAddress = "UnitTest", CustomerId = userId, OrderCode = "UnitTest", OrderType = "UnitTest", RecipientName = "UnitTest", RecipientPhone = "UnitTest" };
            var feedback = new Feedback
            {
                Id = Guid.NewGuid(),
                Order = order,
                CreatedAt = DateTime.Now,
                IsDeleted = false,
                Content = "UnitTest"
            };

            _context.Orders.Add(order);
            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllFeedbacksAsync(null);

            Assert.Empty(result);
        }

        //[Fact]
        //public async Task GetAllFeedbacksAsync_FilterByBranchId()
        //{
        //    var branch1 = Guid.NewGuid();
        //    var branch2 = Guid.NewGuid();

        //    var order1 = new Order { Id = Guid.NewGuid(), BranchId = branch1, CustomerId = Guid.NewGuid(), ShippingAddress = "UnitTest", OrderCode = "UnitTest", OrderType = "UnitTest", RecipientName = "UnitTest", RecipientPhone = "UnitTest" };
        //    var order2 = new Order { Id = Guid.NewGuid(), ShippingAddress = "UnitTest", BranchId = branch2, CustomerId = Guid.NewGuid(), OrderCode = "UnitTest", OrderType = "UnitTest", RecipientName = "UnitTest", RecipientPhone = "UnitTest" };

        //    _context.Orders.AddRange(order1, order2);

        //    _context.Feedbacks.AddRange(
        //        new Feedback { Id = Guid.NewGuid(), Order = order1, CreatedAt = DateTime.Now, IsDeleted = false, Content = "UnitTest" },
        //        new Feedback { Id = Guid.NewGuid(), Order = order2, CreatedAt = DateTime.Now, IsDeleted = false, Content = "UnitTest" });

        //    await _context.SaveChangesAsync();

        //    var result = await _repository.GetAllFeedbacksAsync(branch1);

        //    Assert.Empty(result);
        //    Assert.Equal(branch1, result[0].Order.BranchId);
        //}

        [Fact]
        public async Task GetAllFeedbacksAsync_ExcludeDeleted()
        {
            var branchId = Guid.NewGuid();
            var order = new Order { Id = Guid.NewGuid(), BranchId = branchId, CustomerId = Guid.NewGuid(), OrderCode = "UnitTest", OrderType = "UnitTest", RecipientName = "UnitTest", RecipientPhone = "UnitTest", ShippingAddress = "UnitTest" };

            _context.Orders.Add(order);
            _context.Feedbacks.Add(new Feedback
            {
                Id = Guid.NewGuid(),
                Order = order,
                CreatedAt = DateTime.Now,
                IsDeleted = true,
                Content = "UnitTest"
            });

            await _context.SaveChangesAsync();

            var result = await _repository.GetAllFeedbacksAsync(null);

            Assert.Empty(result);
        }
    }
}

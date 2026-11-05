using Domain.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Order.API.Repositories.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderServiceTest.UnitTests.Repositories
{
    public class OrderRepositoryTests
    {
        private readonly Mock<ILogger<OrderRepository>> _mockLogger;
        private readonly DbContextOptions<SCMDbContext> _dbContextOptions;

        public OrderRepositoryTests()
        {
            _mockLogger = new Mock<ILogger<OrderRepository>>();
            _dbContextOptions = new DbContextOptionsBuilder<SCMDbContext>()
                                .UseInMemoryDatabase("TestDatabase")
                                .Options;
        }

        private SCMDbContext CreateDbContext()
        {
            return new SCMDbContext(_dbContextOptions);
        }

        [Fact]
        public async Task GetOrdersByCustomer_ShouldReturnOrders_WhenCustomerExists()
        {
            // Arrange
            var dbContext = CreateDbContext();
            var orderRepository = new OrderRepository(_mockLogger.Object, dbContext);

            var customerId = Guid.NewGuid();

            dbContext.Orders.Add(new Domain.Entities.Order
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                OrderCode = "ORDER123",
                Status = 1,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                OrderType = "UnitTest",
                RecipientName = " UnitTest",
                RecipientPhone = "UnitTest",
                ShippingAddress = "UnitTest",
            });

            dbContext.SaveChanges();

            // Act
            var orders = await orderRepository.GetOrdersByCustomer(customerId, null, null, null);

            // Assert
            Assert.NotNull(orders);
            Assert.Single(orders);
            Assert.Equal(customerId, orders[0].CustomerId);
        }

        [Fact]
        public async Task GetOrdersByCustomer_ShouldReturnEmpty_WhenCustomerHasNoOrders()
        {
            // Arrange
            var dbContext = CreateDbContext();
            var orderRepository = new OrderRepository(_mockLogger.Object, dbContext);

            var customerId = Guid.NewGuid();

            // Act
            var orders = await orderRepository.GetOrdersByCustomer(customerId, null, null, null);

            // Assert
            Assert.NotNull(orders);
            Assert.Empty(orders);
        }

        [Fact]
        public async Task GetOrderDetailById_ShouldReturnOrder_WhenOrderExists()
        {
            // Arrange
            // Use a unique InMemory DB name so this test does not share state with
            // other tests in the class (previously all used the hardcoded "TestDatabase").
            var options = new DbContextOptionsBuilder<SCMDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var dbContext = new SCMDbContext(options);
            var orderRepository = new OrderRepository(_mockLogger.Object, dbContext);

            // Order.BranchId is [Required]; the repository's Include(b => b.Branch) chain
            // expects a real principal row, so seed a minimal Branch before adding the Order.
            var branch = new Domain.Entities.Branch
            {
                Id = Guid.NewGuid(),
                Name = "UnitTestBranch",
                Address = "UnitTest",
                Phone = "0000000000",
                Type = "UnitTest",
                IsActive = true,
            };
            dbContext.Branches.Add(branch);

            var orderId = Guid.NewGuid();

            var order = new Domain.Entities.Order
            {
                Id = orderId,
                OrderCode = "ORDER123",
                Status = 1,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                CustomerId = Guid.NewGuid(),
                BranchId = branch.Id,
                OrderType = "UnitTest",
                RecipientName = "UnitTest",
                RecipientPhone = "UnitTest",
                ShippingAddress = "UnitTest",
            };

            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await orderRepository.GetOrderDetailById(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderId, result.Id);
        }

        [Fact]
        public async Task AddOrderCancellation_ShouldAddCancellationSuccessfully()
        {
            // Arrange
            var dbContext = CreateDbContext();
            var orderRepository = new OrderRepository(_mockLogger.Object, dbContext);

            var orderCancellation = new OrderCancellation
            {
                Id = Guid.NewGuid(),
                OrderId = Guid.NewGuid(),
                CancelReasonId = Guid.NewGuid(),
                IsDeleted = false,
                CustomReason = "UnitTest",
            };

            // Act
            await orderRepository.AddOrderCancellation(orderCancellation);

            // Assert
            var cancellation = await dbContext.OrderCancellations.FindAsync(orderCancellation.Id);
            Assert.NotNull(cancellation);
        }
    }
}

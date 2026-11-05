using AutoMapper;
using Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Order.API.Dtos.OrderManagement;
using Order.API.Dtos.OrderOfCustomer;
using Order.API.Repositories.Interfaces;
using Order.API.Services.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Domain.Enums.OrderEnum;

namespace OrderServiceTest.UnitTests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<OrderService>> _loggerMock;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<OrderService>>();
            _orderService = new OrderService(_orderRepositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        // Existing tests (from previous response) are assumed to be included here for brevity.

        [Fact]
        public async Task CreateOrderOnl_ValidInput_ReturnsOrderId()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var branchId = Guid.NewGuid();
            var orderDto = new CreateOrderOnlDto
            {
                CustomerId = customerId,
                BranchId = branchId,
                TotalAmount = 1000,
                ShippingAddress = "123 Street",
                RecipientPhone = "1234567890",
                RecipientName = "John Doe",
                //PaymentMethod = (int)EPaymentMethod.Cash,
                OrderDetails = new List<CreateOrderOnlDto.OrderDetailDto>
                {
                    new() { ProductId = Guid.NewGuid(), Quantity = 2 }
                }
            };

            _orderRepositoryMock.Setup(repo => repo.GetCustomerByIdAsync(customerId))
                .ReturnsAsync(new User { Id = customerId });
            _orderRepositoryMock.Setup(repo => repo.GetListIdBranch())
                .ReturnsAsync(new List<Guid> { branchId });
            _orderRepositoryMock.Setup(repo => repo.GetProductById(It.IsAny<Guid>()))
                .ReturnsAsync(new Product { Id = orderDto.OrderDetails[0].ProductId, Price = 500 });
            _orderRepositoryMock.Setup(repo => repo.GetTodayOrderCount())
                .ReturnsAsync(1);
            _orderRepositoryMock.Setup(repo => repo.CreateOrderOnlAsync(It.IsAny<Domain.Entities.Order>()));
                //.Returns(Task.CompletedTask);

            // Act
            var result = await _orderService.CreateOrderOnl(orderDto);

            // Assert
            result.Should().NotBe(Guid.Empty);
            _orderRepositoryMock.Verify(repo => repo.CreateOrderOnlAsync(It.IsAny<Domain.Entities.Order>()), Times.Once());
        }

        [Fact]
        public async Task CreateOrderOnl_CustomerNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var orderDto = new CreateOrderOnlDto { CustomerId = Guid.NewGuid() };
            _orderRepositoryMock.Setup(repo => repo.GetCustomerByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _orderService.CreateOrderOnl(orderDto));
        }

        [Fact]
        public async Task CreateOrderOnl_InvalidVoucher_ThrowsInvalidOperationException()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var branchId = Guid.NewGuid();
            var voucherId = Guid.NewGuid();
            var orderDto = new CreateOrderOnlDto
            {
                CustomerId = customerId,
                BranchId = branchId,
                VoucherId = voucherId,
                TotalAmount = 500
            };

            _orderRepositoryMock.Setup(repo => repo.GetCustomerByIdAsync(customerId))
                .ReturnsAsync(new User { Id = customerId });
            _orderRepositoryMock.Setup(repo => repo.GetListIdBranch())
                .ReturnsAsync(new List<Guid> { branchId });
            _orderRepositoryMock.Setup(repo => repo.GetVoucherByIdAsync(voucherId))
                .ReturnsAsync(new Voucher { Id = voucherId, MinOrder = 1000 });

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _orderService.CreateOrderOnl(orderDto));
        }

        [Fact]
        public async Task UpdateCompletedOrderOnlAsync_OrderInDeliveringStatus_ReturnsTrue()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Domain.Entities.Order
            {
                Id = orderId,
                OrderType = EOrderType.Online.ToString(),
                Status = (int)EStatus.Delivering,
                //IsPaid = false
            };

            _orderRepositoryMock.Setup(repo => repo.GetOrderById(orderId))
                .ReturnsAsync(order);
            _orderRepositoryMock.Setup(repo => repo.UpdateStatusOrder(It.IsAny<Domain.Entities.Order>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _orderService.UpdateCompletedOrderOnlAsync(orderId);

            // Assert
            result.Should().BeTrue();
            _orderRepositoryMock.Verify(repo => repo.UpdateStatusOrder(It.Is<Domain.Entities.Order>(o =>
                o.Status == (int)EStatus.Completed/* && o.IsPaid*/)), Times.Once());
        }

        [Fact]
        public async Task UpdateCompletedOrderOnlAsync_OrderNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _orderRepositoryMock.Setup(repo => repo.GetOrderById(orderId))
                .ReturnsAsync((Domain.Entities.Order)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _orderService.UpdateCompletedOrderOnlAsync(orderId));
        }

        [Fact]
        public async Task UpdateCompletedOrderOnlAsync_OrderIsOffline_ThrowsInvalidOperationException()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Domain.Entities.Order
            {
                Id = orderId,
                OrderType = EOrderType.Offline.ToString(),
                Status = (int)EStatus.Delivering
            };

            _orderRepositoryMock.Setup(repo => repo.GetOrderById(orderId))
                .ReturnsAsync(order);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _orderService.UpdateCompletedOrderOnlAsync(orderId));
        }

        [Fact]
        public async Task CreateOrderOff_ValidInput_ReturnsOrderId()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var tableId = Guid.NewGuid();
            var branchId = Guid.NewGuid();
            var orderDto = new CreateOrderOffDto
            {
                CustomerId = customerId,
                TableId = tableId,
                TotalAmount = 1000,
                OrderDetails = new List<CreateOrderOffDto.OrderDetailOffDtos>
                {
                    new() { ProductId = Guid.NewGuid(), Quantity = 2 }
                }
            };

            _orderRepositoryMock.Setup(repo => repo.GetBranchTableById(tableId))
                .ReturnsAsync(new BranchTable { Id = tableId, BranchId = branchId, IsOccupied = false });
            _orderRepositoryMock.Setup(repo => repo.GetCustomerByIdAsync(customerId))
                .ReturnsAsync(new User { Id = customerId });
            _orderRepositoryMock.Setup(repo => repo.GetProductById(It.IsAny<Guid>()))
                .ReturnsAsync(new Product { Price = 500 });
            _orderRepositoryMock.Setup(repo => repo.GetTodayOrderCount())
                .ReturnsAsync(1);
            _orderRepositoryMock.Setup(repo => repo.CreateOrderOnlAsync(It.IsAny<Domain.Entities.Order>()));
                //.Returns(Task.CompletedTask);
            _orderRepositoryMock.Setup(repo => repo.UpdateBranchTableAsync(It.IsAny<BranchTable>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _orderService.CreateOrderOff(orderDto);

            // Assert
            result.Should().NotBe(Guid.Empty);
            _orderRepositoryMock.Verify(repo => repo.UpdateBranchTableAsync(It.Is<BranchTable>(t => t.IsOccupied)), Times.Once());
        }

        [Fact]
        public async Task CreateOrderOff_TableOccupied_ThrowsArgumentException()
        {
            // Arrange
            var tableId = Guid.NewGuid();
            var orderDto = new CreateOrderOffDto { TableId = tableId };
            _orderRepositoryMock.Setup(repo => repo.GetBranchTableById(tableId))
                .ReturnsAsync(new BranchTable { Id = tableId, IsOccupied = true });

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _orderService.CreateOrderOff(orderDto));
        }

        [Fact]
        public async Task CancelOrderOffAsync_ValidPendingOrder_ReturnsTrue()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Domain.Entities.Order
            {
                Id = orderId,
                OrderType = EOrderType.Offline.ToString(),
                Status = (int)EStatus.Pending,
                CustomerId = Guid.NewGuid(),
                voucherId = Guid.Empty
            };

            _orderRepositoryMock.Setup(repo => repo.GetOrderById(orderId))
                .ReturnsAsync(order);
            _orderRepositoryMock.Setup(repo => repo.UpdateStatusOrder(It.IsAny<Domain.Entities.Order>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _orderService.CancelOrderOffAsync(orderId);

            // Assert
            result.Should().BeTrue();
            _orderRepositoryMock.Verify(repo => repo.UpdateStatusOrder(It.Is<Domain.Entities.Order>(o =>
                o.Status == (int)EStatus.Canceled)), Times.Once());
        }

        [Fact]
        public async Task CancelOrderOffAsync_OnlineOrder_ThrowsInvalidOperationException()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Domain.Entities.Order
            {
                Id = orderId,
                OrderType = EOrderType.Online.ToString(),
                Status = (int)EStatus.Pending
            };

            _orderRepositoryMock.Setup(repo => repo.GetOrderById(orderId))
                .ReturnsAsync(order);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _orderService.CancelOrderOffAsync(orderId));
        }

        [Fact]
        public async Task GetOrderOnlsAsync_OrdersExist_ReturnsPaginatedOrders()
        {
            // Arrange
            var branchId = Guid.NewGuid();
            var orders = new List<Domain.Entities.Order>
            {
                new() { Id = Guid.NewGuid(), OrderCode = "ORD001", Status = (int)EStatus.Pending, OrderType = EOrderType.Online.ToString() },
                new() { Id = Guid.NewGuid(), OrderCode = "ORD002", Status = (int)EStatus.Processing, OrderType = EOrderType.Online.ToString() }
            };
            var orderDtos = orders.Select(o => new OrderDto { OrderId = o.Id, OrderCode = o.OrderCode }).ToList();
            var page = 1;
            var pageSize = 10;

            _orderRepositoryMock.Setup(repo => repo.GetListIdBranch())
                .ReturnsAsync(new List<Guid> { branchId });
            _orderRepositoryMock.Setup(repo => repo.GetOrderOnls(null, null, branchId))
                .ReturnsAsync(orders);
            _mapperMock.Setup(m => m.Map<List<OrderDto>>(It.IsAny<List<Domain.Entities.Order>>()))
                .Returns(orderDtos);

            // Act
            var result = await _orderService.GetOrderOnlsAsync(null, null, branchId, page, pageSize);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            result.TotalItems.Should().Be(2);
            result.CurrentPage.Should().Be(page);
            result.PageSize.Should().Be(pageSize);
            result.TotalPages.Should().Be(1);
        }

        [Fact]
        public async Task GetOrderOnlsAsync_BranchNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var branchId = Guid.NewGuid();
            _orderRepositoryMock.Setup(repo => repo.GetListIdBranch())
                .ReturnsAsync(new List<Guid>());

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _orderService.GetOrderOnlsAsync(null, null, branchId, 1, 10));
        }

        [Fact]
        public async Task GetOrderOnlsAsync_NoOrdersFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var branchId = Guid.NewGuid();
            _orderRepositoryMock.Setup(repo => repo.GetListIdBranch())
                .ReturnsAsync(new List<Guid> { branchId });
            _orderRepositoryMock.Setup(repo => repo.GetOrderOnls(null, null, branchId))
                .ReturnsAsync(new List<Domain.Entities.Order>());

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _orderService.GetOrderOnlsAsync(null, null, branchId, 1, 10));
        }
    }
}

//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.SignalR;
//using Moq;
//using Order.API.Controllers;
//using Order.API.Dtos.CancelReason;
//using Order.API.Dtos.OrderOfCustomer;
//using Order.API.Services.Interfaces;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace OrderServiceTest.UnitTests.Controllers
//{
//    public class OrderControllerTests
//    {
//        private readonly Mock<IOrderService> _orderServiceMock;
//        private readonly Mock<IHubContext<CreateOrderHub>> _hubContextMock;
//        private readonly OrderController _controller;

//        public OrderControllerTests()
//        {
//            _orderServiceMock = new Mock<IOrderService>();
//            _hubContextMock = new Mock<IHubContext<CreateOrderHub>>();
//            _controller = new OrderController(_orderServiceMock.Object, _hubContextMock.Object);
//        }

//        [Fact]
//        public async Task GetAllOrder1Customer_ReturnsOkResult()
//        {
//            // Arrange
//            var customerId = Guid.NewGuid();
//            var orders = new List<OrderOfACustomerDto> { new OrderOfACustomerDto { OrderCode = "ORD001" } };
//            _orderServiceMock.Setup(x => x.GetCustomerOrdersAsync(customerId, null, null, null, 1, 10))
//                             .ReturnsAsync(orders);

//            // Act
//            var result = await _controller.GetAllOrder1Customer(customerId);

//            // Assert
//            var okResult = Assert.IsType<OkObjectResult>(result.Result);
//            var returnValue = Assert.IsType<List<OrderOfACustomerDto>>(okResult.Value);
//            Assert.Single(returnValue);
//        }

//        [Fact]
//        public async Task GetOrderById_ReturnsNotFound_WhenOrderNotExists()
//        {
//            // Arrange
//            var orderId = Guid.NewGuid();
//            _orderServiceMock.Setup(x => x.GetOrderByIdAsync(orderId))
//                             .ThrowsAsync(new KeyNotFoundException("Không tìm thấy đơn hàng"));

//            // Act
//            var result = await _controller.GetOrderById(orderId);

//            // Assert
//            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
//            Assert.Equal("Không tìm thấy đơn hàng", ((dynamic)notFoundResult.Value).message);
//        }

//        [Fact]
//        public async Task CancelOrder_ReturnsOk_WhenSuccessful()
//        {
//            // Arrange
//            var orderId = Guid.NewGuid();
//            var request = new CancelOrderRequestDto
//            {
//                CancelReasonId = Guid.NewGuid(),
//                Reason = null
//            };

//            _orderServiceMock.Setup(x => x.CancelOrderAsync(orderId, request.CancelReasonId, null))
//                             .ReturnsAsync(true);

//            // Act
//            var result = await _controller.CancelOrder(orderId, request);

//            // Assert
//            var okResult = Assert.IsType<OkObjectResult>(result);
//            Assert.Equal("Hủy đơn hàng thành công", ((dynamic)okResult.Value).message);
//        }
//    }
//}

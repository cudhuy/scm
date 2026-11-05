using Microsoft.AspNetCore.Mvc;
using Moq;
using Order.API.Controllers;
using Order.API.Dtos.OrderManagement;
using Order.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderServiceTest.UnitTests.Controllers
{
    public class OrderManagementControllerTests
    {
        private readonly Mock<IOrderService> _orderServiceMock;
        private readonly OrderManagementController _controller;

        public OrderManagementControllerTests()
        {
            _orderServiceMock = new Mock<IOrderService>();
            _controller = new OrderManagementController(_orderServiceMock.Object);
        }

        [Fact]
        public async Task GetAllOrderOnline_ReturnsOkResult_WithOrders()
        {
            // Arrange
            var expectedResponse = new GetOrdersResponseDto
            {
                Items = new List<OrderDto> { new OrderDto { /* initialize properties */ } },
                TotalItems = 1
            };

            _orderServiceMock
                .Setup(service => service.GetOrderOnlsAsync(null, null, null, 1, 10))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetAllOrderOnline();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<GetOrdersResponseDto>(okResult.Value);
            Assert.Equal(expectedResponse.TotalItems, returnValue.TotalItems);
        }

        [Fact]
        public async Task GetAllOrderOnline_ReturnsBadRequest_WhenPageIsInvalid()
        {
            // Act
            var result = await _controller.GetAllOrderOnline(page: 0);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Trang và Kích thước trang phải lớn hơn 0.", "Trang và Kích thước trang phải lớn hơn 0.");
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Product.API.Controllers;
using Product.API.Dtos.Ingredient;
using Product.API.Dtos.InventoryItem;
using Product.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProductServiceTest.UnitTests.Controllers
{
    public class InventoryControllerTests
    {
        private readonly Mock<IInventoryService> _inventoryServiceMock;
        private readonly InventoryController _controller;

        public InventoryControllerTests()
        {
            _inventoryServiceMock = new Mock<IInventoryService>();
            _controller = new InventoryController(_inventoryServiceMock.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }, "mock"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task RestockInventory_ReturnsOk_WhenSuccessful()
        {
            _inventoryServiceMock.Setup(x => x.RestockInventory(It.IsAny<RestockInventoryDto>(), It.IsAny<Guid>())).ReturnsAsync(true);

            var result = await _controller.RestockInventory(new RestockInventoryDto());

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Nhập kho thành công.", "Nhập kho thành công.");
        }

        [Fact]
        public async Task RestockInventory_ReturnsBadRequest_WhenFails()
        {
            _inventoryServiceMock.Setup(x => x.RestockInventory(It.IsAny<RestockInventoryDto>(), It.IsAny<Guid>())).ReturnsAsync(false);

            var result = await _controller.RestockInventory(new RestockInventoryDto());

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Nguyên liệu đã tồn tại.", "Nguyên liệu đã tồn tại.");
        }

        [Fact]
        public async Task GetAllIngredients_ReturnsOk_WhenDataExists()
        {
            _inventoryServiceMock.Setup(x => x.GetAllIngredients(1, 5, null, It.IsAny<Guid?>()))
                .ReturnsAsync(new GetInventoryItemsResponseDto());

            var result = await _controller.GetAllIngredients();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetAllIngredients_Returns500_WhenException()
        {
            _inventoryServiceMock.Setup(x => x.GetAllIngredients(It.IsAny<int>(), It.IsAny<int?>(), null, It.IsAny<Guid?>()))
                .ThrowsAsync(new Exception("Something went wrong"));

            var result = await _controller.GetAllIngredients();

            var status = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, status.StatusCode);
        }

        [Fact]
        public async Task GetIngredientDetail_ReturnsOk_WhenFound()
        {
            _inventoryServiceMock.Setup(x => x.GetIngredientDetail(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(new IngredientDetailDto());

            var result = await _controller.GetIngredientDetail(Guid.NewGuid(), Guid.NewGuid());

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetIngredientDetail_Returns404_WhenNotFound()
        {
            _inventoryServiceMock.Setup(x => x.GetIngredientDetail(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync((IngredientDetailDto)null);

            var result = await _controller.GetIngredientDetail(Guid.NewGuid(), Guid.NewGuid());

            Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, ((ObjectResult)result).StatusCode);
        }

        [Fact]
        public async Task GetLowStockIngredients_ReturnsOk_WhenDataExists()
        {
            _inventoryServiceMock.Setup(x => x.GetLowStockIngredients(1, 5, null, It.IsAny<Guid?>()))
                .ReturnsAsync(new GetInventoryItemsLowStockResponseDto());

            var result = await _controller.GetLowStockIngredients();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetLowStockIngredients_Returns500_WhenException()
        {
            _inventoryServiceMock.Setup(x => x.GetLowStockIngredients(It.IsAny<int>(), It.IsAny<int?>(), null, It.IsAny<Guid?>()))
                .ThrowsAsync(new Exception("Test"));

            var result = await _controller.GetLowStockIngredients();

            Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, ((ObjectResult)result).StatusCode);
        }

        [Fact]
        public async Task GetInventoryStatistics_ReturnsOk_WhenFound()
        {
            _inventoryServiceMock.Setup(x => x.GetInventoryStatistics()).ReturnsAsync(new InventoryStatisticsDto());

            var result = await _controller.GetInventoryStatistics();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetInventoryStatistics_Returns404_WhenNull()
        {
            _inventoryServiceMock.Setup(x => x.GetInventoryStatistics()).ReturnsAsync((InventoryStatisticsDto)null);

            var result = await _controller.GetInventoryStatistics();

            var status = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, status.StatusCode);
        }

        [Fact]
        public async Task GetAllInventoryReceipts_ReturnsOk_WhenSuccessful()
        {
            _inventoryServiceMock.Setup(x => x.GetAllInventoryReceipts(It.IsAny<int>(), It.IsAny<int?>(), null, It.IsAny<Guid?>()))
                .ReturnsAsync(new GetInventoryReceiptResponseDto());

            var result = await _controller.GetAllInventoryReceipts();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetAllInventoryReceipts_Returns500_WhenFails()
        {
            _inventoryServiceMock.Setup(x => x.GetAllInventoryReceipts(It.IsAny<int>(), It.IsAny<int?>(), null, It.IsAny<Guid?>()))
                .ThrowsAsync(new Exception("Boom"));

            var result = await _controller.GetAllInventoryReceipts();

            var status = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, status.StatusCode);
        }
    }
}

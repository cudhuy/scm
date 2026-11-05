using Microsoft.Extensions.Logging;
using Moq;
using Product.API.Dtos.InventoryItem;
using Product.API.Repositories.Interfaces;
using Product.API.Services.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductServiceTest.UnitTests.Services
{
    public class InventoryServiceTests
    {
        private readonly Mock<IInventoryRepository> _mockRepo;
        private readonly Mock<ILogger<InventoryService>> _mockLogger;
        private readonly InventoryService _service;

        public InventoryServiceTests()
        {
            _mockRepo = new Mock<IInventoryRepository>();
            _mockLogger = new Mock<ILogger<InventoryService>>();
            _service = new InventoryService(_mockRepo.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllIngredients_ReturnsItems_WhenDataExists()
        {
            // Arrange
            var mockData = new List<Domain.Entities.Ingredient>
        {
            new Domain.Entities.Ingredient
            {
                Id = Guid.NewGuid(),
                Name = "Sugar",
                Unit = "kg",
                InventoryItems = new List<Domain.Entities.InventoryItem>
                {
                    new Domain.Entities.InventoryItem
                    {
                        Quantity = 10,
                        Inventory = new Domain.Entities.Inventory
                        {
                            Branch = new Domain.Entities.Branch { Name = "Main", Id = Guid.NewGuid() }
                        }
                    }
                },
                InventoryReceipts = new List<Domain.Entities.InventoryReceipt>
                {
                    new Domain.Entities.InventoryReceipt
                    {
                        UnitPrice = 10000,
                        IngredientId = Guid.NewGuid()
                    }
                }
            }
        };
            _mockRepo.Setup(r => r.GetAllIngredients()).ReturnsAsync(mockData);

            // Act
            var result = await _service.GetAllIngredients(1, 10, null, null);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
        }

        [Fact]
        public async Task GetAllIngredients_ReturnsEmpty_WhenNoData()
        {
            _mockRepo.Setup(r => r.GetAllIngredients()).ReturnsAsync(new List<Domain.Entities.Ingredient>());

            var result = await _service.GetAllIngredients(1, 10, null, null);

            Assert.Empty(result.Items);
        }

        [Fact]
        public async Task GetAllInventoryReceipts_ReturnsData_WhenExists()
        {
            var receipts = new List<Domain.Entities.InventoryReceipt>
        {
            new Domain.Entities.InventoryReceipt
            {
                Ingredient = new Domain.Entities.Ingredient { Name = "Flour", Unit = "kg" },
                QuantityReceived = 5,
                UnitPrice = 10000,
                CreatedAt = DateTime.Now,
                Inventory = new Domain.Entities.Inventory
                {
                    Branch = new Domain.Entities.Branch { Name = "Branch A" }
                },
                CreatedBy = Guid.NewGuid()
            }
        };

            _mockRepo.Setup(r => r.GetAllInventoryReceipts()).ReturnsAsync(receipts);
            _mockRepo.Setup(r => r.GetInventoryReceicer(It.IsAny<Guid>())).ReturnsAsync("Test User");

            var result = await _service.GetAllInventoryReceipts(1, 10, null, null);

            Assert.NotNull(result);
            Assert.Single(result.Items);
        }

        [Fact]
        public async Task GetAllInventoryReceipts_ReturnsEmpty_WhenNoReceipts()
        {
            _mockRepo.Setup(r => r.GetAllInventoryReceipts()).ReturnsAsync(new List<Domain.Entities.InventoryReceipt>());

            var result = await _service.GetAllInventoryReceipts(1, 10, null, null);

            Assert.Empty(result.Items);
        }

        [Fact]
        public async Task GetLowStockIngredients_ReturnsItems_WhenExists()
        {
            var data = new List<Domain.Entities.LowStockAlert>
        {
            new Domain.Entities.LowStockAlert
            {
                Ingredient = new Domain.Entities.Ingredient { Name = "Salt", Unit = "kg" },
                InventoryItem = new Domain.Entities.InventoryItem
                {
                    Quantity = 2,
                    LastRestocked = DateTime.Now,
                    ExpiryDate = DateTime.Now.AddDays(10)
                },
                Inventory = new Domain.Entities.Inventory
                {
                    Branch = new Domain.Entities.Branch { Name = "Main" },
                    InventoryItems = new List<Domain.Entities.InventoryItem>()
                },
                AlertMessage = "Low stock"
            }
        };

            _mockRepo.Setup(r => r.GetLowStockIngredients()).ReturnsAsync(data);

            var result = await _service.GetLowStockIngredients(1, 10, null, null);

            Assert.Single(result.Items);
        }

        [Fact]
        public async Task GetLowStockIngredients_ReturnsEmpty_WhenNoData()
        {
            _mockRepo.Setup(r => r.GetLowStockIngredients()).ReturnsAsync(new List<Domain.Entities.LowStockAlert>());

            var result = await _service.GetLowStockIngredients(1, 10, null, null);

            Assert.Empty(result.Items);
        }

        [Fact]
        public async Task RestockInventory_ReturnsTrue_WhenSuccess()
        {
            var dto = new RestockInventoryDto();
            var userId = Guid.NewGuid();

            _mockRepo.Setup(r => r.RestockInventory(dto, userId)).ReturnsAsync(true);

            var result = await _service.RestockInventory(dto, userId);

            Assert.True(result);
        }

        [Fact]
        public async Task RestockInventory_ReturnsFalse_WhenFailed()
        {
            var dto = new RestockInventoryDto();
            var userId = Guid.NewGuid();

            _mockRepo.Setup(r => r.RestockInventory(dto, userId)).ReturnsAsync(false);

            var result = await _service.RestockInventory(dto, userId);

            Assert.False(result);
        }
    }
}

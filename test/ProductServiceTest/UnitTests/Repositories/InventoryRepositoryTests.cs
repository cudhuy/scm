//using Domain.Context;
//using Domain.Entities;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Query;
//using Microsoft.Extensions.Logging;
//using Moq;
//using Product.API.Dtos.Ingredient;
//using Product.API.Dtos.InventoryItem;
//using Product.API.Repositories.Behaviours;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ProductServiceTest.UnitTests.Repositories
//{
//    public class InventoryRepositoryTests
//    {
//        private readonly Mock<SCMDbContext> _mockContext;
//        private readonly Mock<ILogger<InventoryRepository>> _mockLogger;
//        private readonly InventoryRepository _repository;

//        public InventoryRepositoryTests()
//        {
//            _mockContext = new Mock<SCMDbContext>();
//            _mockLogger = new Mock<ILogger<InventoryRepository>>();
//            _repository = new InventoryRepository(_mockLogger.Object, _mockContext.Object);
//        }

//        [Fact]
//        public async Task GetAllExpiredIngredients_ShouldReturnCorrectData_WhenIngredientsExist()
//        {
//            // Arrange
//            var mockData = new List<LowStockAlert>
//            {
//                new LowStockAlert
//                {
//                    Ingredient = new Ingredient { Name = "Ingredient1" },
//                    Inventory = new Inventory { Branch = new Branch { Name = "Branch1" } },
//                    CreatedAt = DateTime.UtcNow
//                }
//            };

//            _mockContext.Setup(c => c.LowStockAlerts.Include(It.IsAny<Func<IQueryable<LowStockAlert>, IIncludableQueryable<LowStockAlert, object>>>())
//                                                      .OrderBy(It.IsAny<Func<LowStockAlert, DateTime>>())
//                                                      .ToListAsync(It.IsAny<CancellationToken>()))
//                        .ReturnsAsync(mockData);

//            // Act
//            var result = await _repository.GetAllExpiredIngredients();

//            // Assert
//            Assert.NotNull(result);
//            Assert.Single(result);
//            Assert.Equal("Ingredient1", result.First().IngredientName);
//            Assert.Equal("Branch1", result.First().InventoryName);
//        }

//        [Fact]
//        public async Task GetAllExpiredIngredients_ShouldReturnEmpty_WhenNoIngredientsExist()
//        {
//            // Arrange
//            var mockData = new List<LowStockAlert>();

//            _mockContext.Setup(c => c.LowStockAlerts.Include(It.IsAny<Func<IQueryable<LowStockAlert>, IIncludableQueryable<LowStockAlert, object>>>())
//                                                      .OrderBy(It.IsAny<Func<LowStockAlert, DateTime>>())
//                                                      .ToListAsync(It.IsAny<CancellationToken>()))
//                        .ReturnsAsync(mockData);

//            // Act
//            var result = await _repository.GetAllExpiredIngredients();

//            // Assert
//            Assert.Empty(result);
//        }

//        [Fact]
//        public async Task GetAllIngredients_ShouldReturnCorrectData_WhenIngredientsExist()
//        {
//            // Arrange
//            var mockData = new List<Ingredient>
//            {
//                new Ingredient { Name = "Ingredient1" },
//                new Ingredient { Name = "Ingredient2" }
//            };

//            _mockContext.Setup(c => c.Ingredients.Include(It.IsAny<Func<IQueryable<Ingredient>, IIncludableQueryable<Ingredient, object>>>())
//                                                  .Where(It.IsAny<Func<Ingredient, bool>>())
//                                                  .ToListAsync(It.IsAny<CancellationToken>()))
//                        .ReturnsAsync(mockData);

//            // Act
//            var result = await _repository.GetAllIngredients();

//            // Assert
//            Assert.NotNull(result);
//            Assert.Equal(2, result.Count);
//            Assert.Contains(result, i => i.Name == "Ingredient1");
//        }

//        [Fact]
//        public async Task GetIngredientDetail_ShouldReturnCorrectData_WhenIngredientExists()
//        {
//            // Arrange
//            var ingredientId = Guid.NewGuid();
//            var branchId = Guid.NewGuid();

//            var mockData = new
//            {
//                IngredientId = ingredientId,
//                Name = "Ingredient1",
//                branchName = "Branch1",
//                Quantity = 100,
//                LowStockThreshold = 10,
//                BatchDetails = new List<BatchDetailDto>
//                {
//                    new BatchDetailDto
//                    {
//                        Quantity = "50 kg",
//                        ImportDate = "2025-04-01",
//                        ExpiryDate = "2025-04-01",
//                        UnitPrice = "5000 đ",
//                        Status = "Còn hạn"
//                    }
//                },
//                Supplier = "Supplier1",
//                AvgPrice = 5000m
//            };

//            _mockContext.Setup(c => c.Ingredients.Where(i => i.Id == ingredientId)
//                                                   .Include(It.IsAny<Func<IQueryable<Ingredient>, IIncludableQueryable<Ingredient, object>>>())
//                                                   .Select(It.IsAny<Func<Ingredient, object>>())
//                                                   .FirstOrDefaultAsync(It.IsAny<CancellationToken>()))
//                        .ReturnsAsync(mockData);

//            // Act
//            var result = await _repository.GetIngredientDetail(branchId, ingredientId);

//            // Assert
//            Assert.NotNull(result);
//            Assert.Equal("Ingredient1", result.Name);
//            Assert.Equal("Branch1", result.branchName);
//            Assert.Equal("50 kg", result.BatchDetails.First().Quantity);
//            Assert.Equal("5000 đ", result.AvgPrice);
//        }

//        [Fact]
//        public async Task RestockInventory_ShouldReturnFalse_WhenIngredientNotFound()
//        {
//            // Arrange
//            var restockDto = new RestockInventoryDto
//            {
//                IngredientId = Guid.NewGuid(),
//                IngredientName = "NewIngredient",
//                BranchId = Guid.NewGuid(),
//                QuantityReceived = 100,
//                UnitPrice = 5000,
//                ExpiryDate = DateTime.UtcNow.AddMonths(6),
//                SupplierName = "Supplier1",
//                LowStockThreshold = 10
//            };

//            _mockContext.Setup(c => c.Ingredients.FirstOrDefaultAsync(It.IsAny<Func<Ingredient, bool>>(), It.IsAny<CancellationToken>()))
//                        .ReturnsAsync((Ingredient)null);

//            // Act
//            var result = await _repository.RestockInventory(restockDto, Guid.NewGuid());

//            // Assert
//            Assert.False(result);
//        }

//        [Fact]
//        public async Task RestockInventory_ShouldReturnTrue_WhenRestockSuccessful()
//        {
//            // Arrange
//            var restockDto = new RestockInventoryDto
//            {
//                IngredientId = Guid.NewGuid(),
//                IngredientName = "NewIngredient",
//                BranchId = Guid.NewGuid(),
//                QuantityReceived = 100,
//                UnitPrice = 5000,
//                ExpiryDate = DateTime.UtcNow.AddMonths(6),
//                SupplierName = "Supplier1",
//                LowStockThreshold = 10
//            };

//            _mockContext.Setup(c => c.Ingredients.FirstOrDefaultAsync(It.IsAny<Func<Ingredient, bool>>(), It.IsAny<CancellationToken>()))
//                        .ReturnsAsync(new Ingredient { Id = restockDto.IngredientId, Name = restockDto.IngredientName });

//            _mockContext.Setup(c => c.Inventories.FirstOrDefaultAsync(It.IsAny<Func<Inventory, bool>>(), It.IsAny<CancellationToken>()))
//                        .ReturnsAsync(new Inventory { Id = Guid.NewGuid() });

//            _mockContext.Setup(c => c.InventoryReceipts.AddAsync(It.IsAny<InventoryReceipt>(), It.IsAny<CancellationToken>()))
//                        .Returns(Task.CompletedTask);

//            _mockContext.Setup(c => c.InventoryItem.AddAsync(It.IsAny<InventoryItem>(), It.IsAny<CancellationToken>()))
//                        .Returns(Task.CompletedTask);

//            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
//                        .ReturnsAsync(1);

//            // Act
//            var result = await _repository.RestockInventory(restockDto, Guid.NewGuid());

//            // Assert
//            Assert.True(result);
//        }
//    }
//}

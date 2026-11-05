//using Domain.Context;
//using Domain.Entities;
//using Microsoft.Extensions.Logging;
//using Moq;
//using Product.API.Repositories.Behaviours;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ProductServiceTest.UnitTests.Repositories
//{
//    public class IngredientsRepositoryTests
//    {
//        private readonly Mock<ILogger<IngredientsRepository>> _mockLogger;
//        private readonly Mock<SCMDbContext> _mockDbContext;
//        private readonly IngredientsRepository _ingredientsRepository;

//        public IngredientsRepositoryTests()
//        {
//            _mockLogger = new Mock<ILogger<IngredientsRepository>>();
//            _mockDbContext = new Mock<SCMDbContext>();
//            _ingredientsRepository = new IngredientsRepository(_mockLogger.Object, _mockDbContext.Object);
//        }

//        [Fact]
//        public async Task CreateIngredient_ShouldReturnTrue_WhenIngredientIsCreatedSuccessfully()
//        {
//            // Arrange
//            var ingredient = new Ingredient { Id = Guid.NewGuid(), Name = "Ingredient 1", IsDeleted = false };
//            _mockDbContext.Setup(x => x.Ingredients.AddAsync(It.IsAny<Ingredient>(), default))
//                          .ReturnsAsync(new Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Ingredient>(null));

//            // Act
//            var result = await _ingredientsRepository.CreateIngredient(ingredient);

//            // Assert
//            Assert.True(result);
//        }

//        [Fact]
//        public async Task CreateIngredient_ShouldReturnFalse_WhenErrorOccurs()
//        {
//            // Arrange
//            var ingredient = new Ingredient { Id = Guid.NewGuid(), Name = "Ingredient 2", IsDeleted = false };
//            _mockDbContext.Setup(x => x.Ingredients.AddAsync(It.IsAny<Ingredient>(), default))
//                          .ThrowsAsync(new Exception("Database error"));

//            // Act
//            var result = await _ingredientsRepository.CreateIngredient(ingredient);

//            // Assert
//            Assert.False(result);
//        }

//        [Fact]
//        public async Task GetListIngredient_ShouldReturnIngredients_WhenIngredientsExist()
//        {
//            // Arrange
//            var ingredients = new List<Ingredient>
//        {
//            new Ingredient { Id = Guid.NewGuid(), Name = "Ingredient 1", IsDeleted = false },
//            new Ingredient { Id = Guid.NewGuid(), Name = "Ingredient 2", IsDeleted = false }
//        };
//            _mockDbContext.Setup(x => x.Ingredients.Where(It.IsAny<Func<Ingredient, bool>>()))
//                          .Returns(ingredients.AsQueryable());

//            // Act
//            var result = await _ingredientsRepository.GetListIngredient();

//            // Assert
//            Assert.Equal(2, result.Count);
//        }

//        [Fact]
//        public async Task GetListIngredient_ShouldReturnEmptyList_WhenNoIngredientsExist()
//        {
//            // Arrange
//            var ingredients = new List<Ingredient>();
//            _mockDbContext.Setup(x => x.Ingredients.Where(It.IsAny<Func<Ingredient, bool>>()))
//                          .Returns(ingredients.AsQueryable());

//            // Act
//            var result = await _ingredientsRepository.GetListIngredient();

//            // Assert
//            Assert.Empty(result);
//        }

//        //[Fact]
//        //public async Task GetListIngredient_ShouldThrowException_WhenErrorOccurs()
//        //{
//        //    // Arrange
//        //    _mockDbContext.Setup(x => x.Ingredients.Where(It.IsAny<Func<Ingredient, bool>>()))
//        //                  .ThrowsAsync(new Exception("Database error"));

//        //    // Act & Assert
//        //    await Assert.ThrowsAsync<Exception>(() => _ingredientsRepository.GetListIngredient());
//        //}
//    }
//}

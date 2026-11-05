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
//    public class CategoryRepositoryTests
//    {
//        private readonly Mock<ILogger<CategoryRepository>> _mockLogger;
//        private readonly Mock<SCMDbContext> _mockDbContext;
//        private readonly CategoryRepository _categoryRepository;

//        public CategoryRepositoryTests()
//        {
//            _mockLogger = new Mock<ILogger<CategoryRepository>>();
//            _mockDbContext = new Mock<SCMDbContext>();
//            _categoryRepository = new CategoryRepository(_mockLogger.Object, _mockDbContext.Object);
//        }

//        [Fact]
//        public async Task GetListCategory_ShouldReturnCategories_WhenCategoriesExist()
//        {
//            // Arrange
//            var categories = new List<Category>
//        {
//            new Category { Id = Guid.NewGuid(), Name = "Category 1", IsDeleted = false },
//            new Category { Id = Guid.NewGuid(), Name = "Category 2", IsDeleted = false }
//        };
//            _mockDbContext.Setup(x => x.Categories.Where(It.IsAny<Func<Category, bool>>()))
//                          .Returns(categories.AsQueryable());

//            // Act
//            var result = await _categoryRepository.GetListCategory();

//            // Assert
//            Assert.Equal(2, result.Count);
//        }

//        [Fact]
//        public async Task CreateCategory_ShouldReturnTrue_WhenCategoryIsCreatedSuccessfully()
//        {
//            // Arrange
//            var category = new Category { Id = Guid.NewGuid(), Name = "New Category", IsDeleted = false };
//            _mockDbContext.Setup(x => x.Categories.AddAsync(It.IsAny<Category>(), default))
//                          .ReturnsAsync(new Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Category>(null));

//            // Act
//            var result = await _categoryRepository.CreateCategory(category);

//            // Assert
//            Assert.True(result);
//        }

//        //[Fact]
//        //public async Task UpdateCategory_ShouldReturnTrue_WhenCategoryIsUpdatedSuccessfully()
//        //{
//        //    // Arrange
//        //    var categoryId = Guid.NewGuid();
//        //    var category = new Category { Id = categoryId, Name = "Updated Category", IsDeleted = false };
//        //    _mockDbContext.Setup(x => x.Categories.FirstOrDefaultAsync(It.IsAny<Func<Category, bool>>()))
//        //                  .ReturnsAsync(category);

//        //    // Act
//        //    var result = await _categoryRepository.UpdateCategory(categoryId, category);

//        //    // Assert
//        //    Assert.True(result);
//        //}

//        //[Fact]
//        //public async Task GetCategoryById_ShouldReturnCategory_WhenCategoryExists()
//        //{
//        //    // Arrange
//        //    var categoryId = Guid.NewGuid();
//        //    var category = new Category { Id = categoryId, Name = "Test Category" };
//        //    _mockDbContext.Setup(x => x.Categories.FirstOrDefaultAsync(It.IsAny<Func<Category, bool>>()))
//        //                  .ReturnsAsync(category);

//        //    // Act
//        //    var result = await _categoryRepository.GetCategoryById(categoryId);

//        //    // Assert
//        //    Assert.NotNull(result);
//        //    Assert.Equal(categoryId, result.Id);
//        //}

//        //[Fact]
//        //public async Task DeleteCategory_ShouldReturnTrue_WhenCategoryIsDeletedSuccessfully()
//        //{
//        //    // Arrange
//        //    var categoryId = Guid.NewGuid();
//        //    var category = new Category { Id = categoryId, IsDeleted = false };
//        //    _mockDbContext.Setup(x => x.Categories.FirstOrDefaultAsync(It.IsAny<Func<Category, bool>>()))
//        //                  .ReturnsAsync(category);

//        //    // Act
//        //    var result = await _categoryRepository.DeleteCategory(categoryId);

//        //    // Assert
//        //    Assert.True(result);
//        //    Assert.True(category.IsDeleted);
//        //}
//    }
//}

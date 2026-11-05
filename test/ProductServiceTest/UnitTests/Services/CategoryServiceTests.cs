using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Product.API.Dtos.Category;
using Product.API.Repositories.Interfaces;
using Product.API.Services.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductServiceTest.UnitTests.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<CategoryService>> _loggerMock;
        private readonly CategoryService _service;

        public CategoryServiceTests()
        {
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<CategoryService>>();
            _service = new CategoryService(_categoryRepositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllCategories_ShouldReturnList_WhenExist()
        {
            _categoryRepositoryMock.Setup(r => r.GetListCategory()).ReturnsAsync(new List<Category>
        {
            new Category { Id = Guid.NewGuid(), Name = "Test" }
        });

            var result = await _service.GetAllCategories();

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetAllCategories_ShouldReturnEmpty_WhenNoneExist()
        {
            _categoryRepositoryMock.Setup(r => r.GetListCategory()).ReturnsAsync(new List<Category>());

            var result = await _service.GetAllCategories();

            Assert.Empty(result);
        }

        [Fact]
        public async Task CreateCategory_ShouldReturnTrue_WhenSuccess()
        {
            var dto = new CreateCategoryDto();
            _mapperMock.Setup(m => m.Map<Category>(dto)).Returns(new Category());
            _categoryRepositoryMock.Setup(r => r.CreateCategory(It.IsAny<Category>())).ReturnsAsync(true);

            var result = await _service.CreateCategory(dto);

            Assert.True(result);
        }

        [Fact]
        public async Task CreateCategory_ShouldReturnFalse_WhenFailed()
        {
            var dto = new CreateCategoryDto();
            _mapperMock.Setup(m => m.Map<Category>(dto)).Returns(new Category());
            _categoryRepositoryMock.Setup(r => r.CreateCategory(It.IsAny<Category>())).ReturnsAsync(false);

            var result = await _service.CreateCategory(dto);

            Assert.False(result);
        }

        [Fact]
        public async Task UpdateCategory_ShouldReturnTrue_WhenSuccess()
        {
            var id = Guid.NewGuid();
            var dto = new UpdateCategoryDto();

            _categoryRepositoryMock.Setup(r => r.GetCategoryById(id)).ReturnsAsync(new Category());
            _mapperMock.Setup(m => m.Map<Category>(dto)).Returns(new Category());
            _categoryRepositoryMock.Setup(r => r.UpdateCategory(id, It.IsAny<Category>())).ReturnsAsync(true);

            var result = await _service.UpdateCategory(id, dto);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateCategory_ShouldReturnFalse_WhenNotFound()
        {
            var id = Guid.NewGuid();
            var dto = new UpdateCategoryDto();

            _categoryRepositoryMock.Setup(r => r.GetCategoryById(id)).ReturnsAsync((Category)null);

            var result = await _service.UpdateCategory(id, dto);

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteCategory_ShouldReturnTrue_WhenSuccess()
        {
            var id = Guid.NewGuid();
            _categoryRepositoryMock.Setup(r => r.DeleteCategory(id)).ReturnsAsync(true);

            var result = await _service.DeleteCategory(id);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteCategory_ShouldReturnFalse_WhenFailed()
        {
            var id = Guid.NewGuid();
            _categoryRepositoryMock.Setup(r => r.DeleteCategory(id)).ReturnsAsync(false);

            var result = await _service.DeleteCategory(id);

            Assert.False(result);
        }
    }
}

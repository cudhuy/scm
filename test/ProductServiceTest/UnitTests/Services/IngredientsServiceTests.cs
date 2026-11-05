using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Product.API.Dtos.Ingredient;
using Product.API.Repositories.Interfaces;
using Product.API.Services.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductServiceTest.UnitTests.Services
{
    public class IngredientsServiceTests
    {
        private readonly Mock<IIngredientsRepository> _ingredientsRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<IngredientsService>> _loggerMock;
        private readonly IngredientsService _service;

        public IngredientsServiceTests()
        {
            _ingredientsRepositoryMock = new Mock<IIngredientsRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<IngredientsService>>();
            _service = new IngredientsService(_ingredientsRepositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateIngredient_ShouldReturnTrue_WhenCreationSuccessful()
        {
            var dto = new CreateIngredientDto();
            _mapperMock.Setup(m => m.Map<Ingredient>(dto)).Returns(new Ingredient());
            _ingredientsRepositoryMock.Setup(r => r.CreateIngredient(It.IsAny<Ingredient>())).ReturnsAsync(true);

            var result = await _service.CreateIngredient(dto);

            Assert.True(result);
        }

        [Fact]
        public async Task CreateIngredient_ShouldReturnFalse_WhenCreationFails()
        {
            var dto = new CreateIngredientDto();
            _mapperMock.Setup(m => m.Map<Ingredient>(dto)).Returns(new Ingredient());
            _ingredientsRepositoryMock.Setup(r => r.CreateIngredient(It.IsAny<Ingredient>())).ReturnsAsync(false);

            var result = await _service.CreateIngredient(dto);

            Assert.False(result);
        }

        [Fact]
        public async Task GetAllIngredient_ShouldReturnList_WhenDataExists()
        {
            var list = new List<Ingredient> { new Ingredient { Id = Guid.NewGuid(), Name = "Sugar", Unit = "Kg" } };
            _ingredientsRepositoryMock.Setup(r => r.GetListIngredient()).ReturnsAsync(list);

            var result = await _service.GetAllIngredient();

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetAllIngredient_ShouldReturnEmpty_WhenNoData()
        {
            _ingredientsRepositoryMock.Setup(r => r.GetListIngredient()).ReturnsAsync(new List<Ingredient>());

            var result = await _service.GetAllIngredient();

            Assert.Empty(result);
        }
    }

}

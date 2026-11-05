using Microsoft.AspNetCore.Mvc;
using Moq;
using Product.API.Controllers;
using Product.API.Dtos.Ingredient;
using Product.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductServiceTest.UnitTests.Controllers
{
    public class IngredientsControllerTests
    {
        private readonly Mock<IIngredientsService> _mockService;
        private readonly IngredientsController _controller;

        public IngredientsControllerTests()
        {
            _mockService = new Mock<IIngredientsService>();
            _controller = new IngredientsController(_mockService.Object);
        }

        // ----- CREATE INGREDIENT -----

        [Fact]
        public async Task CreateIngredient_ReturnsOk_WhenCreatedSuccessfully()
        {
            // Arrange
            var dto = new CreateIngredientDto { Name = "Sugar", Unit = "Sweet thing" };
            _mockService.Setup(s => s.CreateIngredient(dto)).ReturnsAsync(true);

            // Act
            var result = await _controller.CreateIngredient(dto);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, ok.StatusCode);
        }

        [Fact]
        public async Task CreateIngredient_ReturnsBadRequest_WhenInputIsNull()
        {
            // Act
            var result = await _controller.CreateIngredient(null);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, bad.StatusCode);
        }

        // ----- GET ALL INGREDIENT -----

        [Fact]
        public async Task GetAllIngredient_ReturnsOk_WhenIngredientsExist()
        {
            // Arrange
            var mockList = new List<IngredientDto>
            {
                new IngredientDto { IngredientId = Guid.NewGuid(), Name = "Salt" },
                new IngredientDto { IngredientId = Guid.NewGuid(), Name = "Sugar" }
            };
            _mockService.Setup(s => s.GetAllIngredient()).ReturnsAsync(mockList);

            // Act
            var result = await _controller.GetAllIngredient();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, ok.StatusCode);
        }

        [Fact]
        public async Task GetAllIngredient_ReturnsNotFound_WhenListIsEmpty()
        {
            // Arrange
            _mockService.Setup(s => s.GetAllIngredient()).ReturnsAsync(new List<IngredientDto>());

            // Act
            var result = await _controller.GetAllIngredient();

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFound.StatusCode);
        }
    }
}

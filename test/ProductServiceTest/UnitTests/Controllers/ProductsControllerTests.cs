using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;
using Product.API.Controllers;
using Product.API.Dtos.Product;
using Product.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ProductServiceTest.UnitTests.Controllers
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _mockService;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockService = new Mock<IProductService>();
            _controller = new ProductsController(_mockService.Object);
        }

        // Test cases for GetProducts
        [Fact]
        public async Task GetProducts_ShouldReturnProducts_WhenExist()
        {
            var products = new ProductResponse
            {
                Items = new List<ProductDto>
                {
                    new ProductDto
                    {
                        Id = Guid.NewGuid(),
                        Name = "TestProduct",
                        Price = 100,
                        CategoryId = Guid.NewGuid(),
                        CategoryName = "CategoryTest",
                        Description = "Test description",
                        Image = "test.jpg"
                    }
                },
                TotalItems = 1,
                CurrentPage = 1,
                PageSize = 10,
                TotalPages = 1
            };
            _mockService.Setup(s => s.GetProductsAsync(1, 10, null, null)).ReturnsAsync(products);

            var result = await _controller.GetProducts(1, 10, null, null) as OkObjectResult;

            result.Should().NotBeNull();
            result!.Value.Should().BeEquivalentTo(products);
        }

        [Fact]
        public async Task GetProducts_ShouldReturnNotFound_WhenNoProducts()
        {
            var products = new ProductResponse { Items = new List<ProductDto>() };
            _mockService.Setup(s => s.GetProductsAsync(1, 10, null, null)).ReturnsAsync(products);

            var result = await _controller.GetProducts(1, 10, null, null);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetProducts_ShouldReturnBadRequest_WhenExceptionThrown()
        {
            _mockService.Setup(s => s.GetProductsAsync(1, 10, null, null)).ThrowsAsync(new Exception("Database error"));

            var result = await _controller.GetProducts(1, 10, null, null) as BadRequestObjectResult;

            result.Should().NotBeNull();
            result!.Value.Should().BeEquivalentTo(new { message = "Database error" });
        }

        // Test cases for GetDetailProduct
        [Fact]
        public async Task GetDetailProduct_ShouldReturnProduct_WhenExists()
        {
            var productId = Guid.NewGuid();
            var product = new ProductDetailDto { Id = productId, Name = "TestProduct" };
            _mockService.Setup(s => s.GetDetailProductsAsync(productId)).ReturnsAsync(product);

            var result = await _controller.GetDetailProduct(productId) as OkObjectResult;

            result.Should().NotBeNull();
            result!.Value.Should().BeEquivalentTo(product);
        }

        [Fact]
        public async Task GetDetailProduct_ShouldReturnNotFound_WhenNotExists()
        {
            var productId = Guid.NewGuid();
            _mockService.Setup(s => s.GetDetailProductsAsync(productId)).ReturnsAsync((ProductDetailDto?)null);

            var result = await _controller.GetDetailProduct(productId);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetDetailProduct_ShouldReturnInternalServerError_WhenExceptionThrown()
        {
            var productId = Guid.NewGuid();
            _mockService.Setup(s => s.GetDetailProductsAsync(productId)).ThrowsAsync(new Exception());

            var result = await _controller.GetDetailProduct(productId) as ObjectResult;

            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(500);
        }

        // Test cases for GetProductIngredients
        [Fact]
        public async Task GetProductIngredients_ShouldReturnIngredients_WhenExist()
        {
            var productId = Guid.NewGuid();
            var ingredients = new List<IngredientOfProductDto> { new IngredientOfProductDto { IngredientId = Guid.NewGuid(), QuantityRequired = 10 , IngredientName = "AA", Unit = "G"} };
            _mockService.Setup(s => s.GetProductIngredientsAsync(productId)).ReturnsAsync(ingredients);

            var result = await _controller.GetProductIngredients(productId) as OkObjectResult;

            result.Should().NotBeNull();
            result!.Value.Should().BeEquivalentTo(ingredients);
        }

        [Fact]
        public async Task GetProductIngredients_ShouldReturnNotFound_WhenNoIngredients()
        {
            var productId = Guid.NewGuid();
            _mockService.Setup(s => s.GetProductIngredientsAsync(productId)).ReturnsAsync( new List<IngredientOfProductDto>());

            var result = await _controller.GetProductIngredients(productId);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        // Test cases for DeleteProduct
        [Fact]
        public async Task DeleteProduct_ShouldReturnOk_WhenDeleted()
        {
            var productId = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteProduct(productId)).ReturnsAsync(true);

            var result = await _controller.DeleteProduct(productId) as OkObjectResult;

            result.Should().NotBeNull();
            result!.Value.Should().BeEquivalentTo(new { message = "Xóa sản phẩm thành công!" });
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnNotFound_WhenNotExists()
        {
            var productId = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteProduct(productId)).ReturnsAsync(false);

            var result = await _controller.DeleteProduct(productId);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnInternalServerError_WhenExceptionThrown()
        {
            var productId = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteProduct(productId)).ThrowsAsync(new Exception("Delete failed"));

            var result = await _controller.DeleteProduct(productId) as ObjectResult;

            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(500);
        }

        // Test cases for CreateProduct
        //[Fact]
        //public async Task CreateProduct_ShouldReturnCreated_WhenValid()
        //{
        //    var productDto = new CreateProductDto { Name = "New Product", Ingredients = "[]" };
        //    var ingredients = new List<ProductIngredientDTO>();
        //    _mockService.Setup(s => s.CreateProduct(productDto, It.IsAny<List<ProductIngredientDTO>>())).ReturnsAsync(true);

        //    var result = await _controller.CreateProduct(productDto) as CreatedAtActionResult;

        //    result.Should().NotBeNull();
        //    result!.Value.Should().BeEquivalentTo(new { message = "Product created successfully." });
        //}

        [Fact]
        public async Task CreateProduct_ShouldReturnBadRequest_WhenInvalidJson()
        {
            var productDto = new CreateProductDto { Name = "New Product", Ingredients = "invalid json" };

            var result = await _controller.CreateProduct(productDto) as BadRequestObjectResult;

            result.Should().NotBeNull();
            result!.Value.Should().BeEquivalentTo(new { message = "Định dạng JSON không hợp lệ trong trường nguyên liệu." });
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnBadRequest_WhenEmptyIngredients()
        {
            var productDto = new CreateProductDto { Name = "New Product", Ingredients = "" };

            var result = await _controller.CreateProduct(productDto) as BadRequestObjectResult;

            result.Should().NotBeNull();
            result!.Value.Should().BeEquivalentTo(new { message = "Danh sách nguyên liệu là bắt buộc và không được để trống." });
        }

        // Test cases for UpdateProduct
        //[Fact]
        //public async Task UpdateProduct_ShouldReturnOk_WhenUpdated()
        //{
        //    var productId = Guid.NewGuid();
        //    var productDto = new UpdateProductDto { Name = "Updated Product", Ingredients = "[]" };
        //    _mockService.Setup(s => s.UpdateProduct(productId, productDto, It.IsAny<List<ProductIngredientDTO>>())).ReturnsAsync(true);

        //    var result = await _controller.UpdateProduct(productId, productDto) as OkObjectResult;

        //    result.Should().NotBeNull();
        //    result!.Value.Should().BeEquivalentTo(new { message = "Product updated successfully." });
        //}

        [Fact]
        public async Task UpdateProduct_ShouldReturnNotFound_WhenProductNotExists()
        {
            var productId = Guid.NewGuid();
            var productDto = new UpdateProductDto { Name = "Updated Product", Ingredients = "[]" };
            _mockService.Setup(s => s.UpdateProduct(productId, productDto, It.IsAny<List<ProductIngredientDTO>>())).ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.UpdateProduct(productId, productDto);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UpdateProduct_ShouldReturnBadRequest_WhenInvalidData()
        {
            var productId = Guid.NewGuid();
            var productDto = new UpdateProductDto { Name = "Updated Product", Ingredients = "invalid json" };

            var result = await _controller.UpdateProduct(productId, productDto) as BadRequestObjectResult;

            result.Should().NotBeNull();
            result!.Value.Should().BeEquivalentTo(new { message = "Định dạng JSON không hợp lệ trong trường Thành phần." });
        }

        //// Test cases for GetListProductBase
        //[Fact]
        //public async Task GetListProductBase_ShouldReturnProducts_WhenExist()
        //{
        //    var products = new GetProductsBaseResponseDto { Items = new List<ProductBaseDto> { new ProductBaseDto { Id = Guid.NewGuid(), Name = "Test" , CategoryName ="fgh" , Image="123" , Price = 12 } } };
        //    _mockService.Setup(s => s.GetListProductBase(1, 2, "", 0)).ReturnsAsync(products);

        //    var result = await _controller.GetListProductBase(1, null, null, 0) as OkObjectResult;

        //    result.Should().NotBeNull();
        //    result!.Value.Should().BeEquivalentTo(products);
        //}

        [Fact]
        public async Task GetListProductBase_ShouldReturnBadRequest_WhenInvalidSortOrder()
        {
            var result = await _controller.GetListProductBase(1, null, null, 3) as BadRequestObjectResult;

            result.Should().NotBeNull();
            result!.Value.Should().BeEquivalentTo(new { message = "Thứ tự sắp xếp không hợp lệ. Các giá trị chấp nhận: 0 (mặc định), 1 (giá tăng dần), 2 (giá giảm dần)." });
        }
    }
}
//using AutoMapper;
//using Domain.Entities;
//using FluentAssertions;
//using Microsoft.Extensions.Logging;
//using Moq;
//using Product.API.Dtos.Product;
//using Product.API.Helpers;
//using Product.API.Repositories.Interfaces;
//using Product.API.Services.Behaviours;

//namespace ProductServiceTest.UnitTests.Services
//{
//    public class ProductServiceTests
//    {
//        private readonly Mock<IProductRepository> _mockRepo;
//        private readonly Mock<IMapper> _mockMapper;
//        private readonly Mock<ILogger<ProductService>> _mockLogger;
//        private readonly Mock<CloudinaryHelper> _mockCloudinaryHelper;
//        private readonly ProductService _service;

//        public ProductServiceTests()
//        {
            //_mockRepo = new Mock<IProductRepository>();
//            _mockMapper = new Mock<IMapper>();
//            _mockLogger = new Mock<ILogger<ProductService>>();
//            _mockCloudinaryHelper = new Mock<CloudinaryHelper>();

//            _service = new ProductService(
//                _mockRepo.Object,
//                _mockMapper.Object,
//                _mockLogger.Object,
//                _mockCloudinaryHelper.Object
//            );
//        }

//        [Fact]
//        public async Task GetProductsAsync_ShouldReturnPaginatedProducts_WhenProductsExist()
//        {
//            var products = new List<Domain.Entities.Product>
//            {
//                new() { Id = Guid.NewGuid(), Name = "P1", Price = 10, CategoryId = Guid.NewGuid(), Description = "Desc", Image = "img" },
//                new() { Id = Guid.NewGuid(), Name = "P2", Price = 20, CategoryId = Guid.NewGuid(), Description = "Desc", Image = "img" }
//            };

//            _mockRepo.Setup(r => r.GetProductsAsync(null, null)).ReturnsAsync(products);
//            _mockMapper.Setup(m => m.Map<List<ProductDto>>(It.IsAny<List<Domain.Entities.Product>>())).Returns(new List<ProductDto>());

//            var result = await _service.GetProductsAsync(1, 2, null, null);

//            result.Should().NotBeNull();
//            result!.TotalItems.Should().Be(2);
//            result.Items.Should().HaveCount(2);
//        }

//        [Fact]
//        public async Task GetProductsAsync_ShouldReturnNull_WhenNoProducts()
//        {
//            _mockRepo.Setup(r => r.GetProductsAsync(null, null)).ReturnsAsync(new List<Domain.Entities.Product>());

//            var result = await _service.GetProductsAsync(1, 10, null, null);

//            result.Should().BeNull();
//        }

//        [Fact]
//        public async Task GetProductsAsync_ShouldPaginateCorrectly()
//        {
//            var products = Enumerable.Range(1, 10)
//                .Select(i => new Domain.Entities.Product { Id = Guid.NewGuid(), Name = $"Product{i}" }).ToList();

//            _mockRepo.Setup(r => r.GetProductsAsync(null, null)).ReturnsAsync(products);
//            _mockMapper.Setup(m => m.Map<List<ProductDto>>(It.IsAny<List<Domain.Entities.Product>>()))
//                .Returns(new List<ProductDto>());

//            var result = await _service.GetProductsAsync(2, 3, null, null);

//            result.Should().NotBeNull();
//            result!.CurrentPage.Should().Be(2);
//            result.Items.Count.Should().Be(3);
//        }

//        [Fact]
//        public async Task GetDetailProductsAsync_ShouldReturnDetails_WhenProductExists()
//        {
//            var productId = Guid.NewGuid();
//            var product = new Domain.Entities.Product
//            {
//                Id = productId,
//                Name = "Product",
//                Description = "Desc",
//                Price = 50,
//                Category = new Category { Name = "Category1" },
//                ProductIngredients = new List<ProductIngredient>
//                {
//                    new() { Ingredient = new Ingredient { Id = Guid.NewGuid(), Name = "Salt" }, QuantityRequired = 5, Unit = "g" }
//                }
//            };

//            _mockRepo.Setup(r => r.GetProductByIdAsync(productId)).ReturnsAsync(product);

//            var result = await _service.GetDetailProductsAsync(productId);

//            result.Should().NotBeNull();
//            result!.Name.Should().Be("Product");
//            result.Ingredients.Should().HaveCount(1);
//        }

//        [Fact]
//        public async Task GetDetailProductsAsync_ShouldReturnNull_WhenNotFound()
//        {
//            _mockRepo.Setup(r => r.GetProductByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Domain.Entities.Product?)null);

//            var result = await _service.GetDetailProductsAsync(Guid.NewGuid());

//            result.Should().BeNull();
//        }

//        [Fact]
//        public async Task GetDetailProductsAsync_ShouldHandleEmptyIngredients()
//        {
//            var product = new Domain.Entities.Product
//            {
//                Id = Guid.NewGuid(),
//                Name = "NoIngredientProduct",
//                ProductIngredients = new List<ProductIngredient>()
//            };

//            _mockRepo.Setup(r => r.GetProductByIdAsync(product.Id)).ReturnsAsync(product);

//            var result = await _service.GetDetailProductsAsync(product.Id);

//            result.Should().NotBeNull();
//            result!.Ingredients.Should().BeEmpty();
//        }

//        [Fact]
//        public async Task GetProductIngredients_ShouldReturnIngredients_WhenExist()
//        {
//            var productId = Guid.NewGuid();
//            var product = new Domain.Entities.Product
//            {
//                ProductIngredients = new List<ProductIngredient>
//                {
//                    new() { Ingredient = new Ingredient { Id = Guid.NewGuid(), Name = "Sugar" }, QuantityRequired = 10, Unit = "g" }
//                }
//            };

//            _mockRepo.Setup(r => r.GetProductWithIngredientsAsync(productId)).ReturnsAsync(product);

//            var result = await _service.GetProductIngredientsAsync(productId);

//            result.Should().HaveCount(1);
//            result[0].IngredientName.Should().Be("Sugar");
//        }

//        [Fact]
//        public async Task GetProductIngredients_ShouldReturnEmpty_WhenNoIngredients()
//        {
//            var product = new Domain.Entities.Product
//            {
//                ProductIngredients = new List<ProductIngredient>()
//            };

//            _mockRepo.Setup(r => r.GetProductWithIngredientsAsync(It.IsAny<Guid>())).ReturnsAsync(product);

//            var result = await _service.GetProductIngredientsAsync(Guid.NewGuid());

//            result.Should().BeEmpty();
//        }

//        [Fact]
//        public async Task GetProductIngredients_ShouldReturnEmpty_WhenProductNotFound()
//        {
//            _mockRepo.Setup(r => r.GetProductWithIngredientsAsync(It.IsAny<Guid>())).ReturnsAsync((Domain.Entities.Product?)null);

//            var result = await _service.GetProductIngredientsAsync(Guid.NewGuid());

//            result.Should().BeEmpty();
//        }

//        [Fact]
//        public async Task DeleteProduct_ShouldReturnTrue_WhenDeletedSuccessfully()
//        {
//            var id = Guid.NewGuid();
//            _mockRepo.Setup(r => r.DeleteProduct(id)).ReturnsAsync(true);

//            var result = await _service.DeleteProduct(id);

//            result.Should().BeTrue();
//        }

//        [Fact]
//        public async Task DeleteProduct_ShouldReturnFalse_WhenNotFound()
//        {
//            var id = Guid.NewGuid();
//            _mockRepo.Setup(r => r.DeleteProduct(id)).ReturnsAsync(false);

//            var result = await _service.DeleteProduct(id);

//            result.Should().BeFalse();
//        }

//        [Fact]
//        public async Task DeleteProduct_ShouldThrowException_WhenRepositoryFails()
//        {
//            var id = Guid.NewGuid();
//            _mockRepo.Setup(r => r.DeleteProduct(id)).ThrowsAsync(new Exception("Failed"));

//            Func<Task> act = async () => await _service.DeleteProduct(id);

//            await act.Should().ThrowAsync<Exception>().WithMessage("*Failed*");
//        }

//        [Fact]
//        public async Task CreateProduct_ShouldReturnTrue_WhenValid()
//        {
//            var dto = new CreateProductDto { Name = "TestProduct", CategoryId = Guid.NewGuid() };
//            var ingredientId = Guid.NewGuid();
//            var ingredients = new List<ProductIngredientDTO>
//            {
//                new() { IngredientId = ingredientId, QuantityRequired = 5 }
//            };

//            _mockRepo.Setup(r => r.GetListIdIngredient()).ReturnsAsync(new List<Guid> { ingredientId });
//            _mockRepo.Setup(r => r.ProductExistsWithSameIngredientsAndCategory(dto.Name, dto.CategoryId, ingredients)).ReturnsAsync(false);
//            _mockRepo.Setup(r => r.GetIngredientUnits(It.IsAny<List<Guid>>())).ReturnsAsync(new Dictionary<Guid, string> { { ingredientId, "g" } });
//            _mockRepo.Setup(r => r.CreateProduct(It.IsAny<Domain.Entities.Product>(), It.IsAny<List<ProductIngredient>>())).ReturnsAsync(true);

//            var result = await _service.CreateProduct(dto, ingredients);

//            result.Should().BeTrue();
//        }

//        [Fact]
//        public async Task CreateProduct_ShouldThrowArgumentException_WhenInvalidIngredients()
//        {
//            var dto = new CreateProductDto { Name = "InvalidProduct", CategoryId = Guid.NewGuid() };
//            var invalidId = Guid.NewGuid();
//            var ingredients = new List<ProductIngredientDTO> { new() { IngredientId = invalidId, QuantityRequired = 2 } };

//            _mockRepo.Setup(r => r.GetListIdIngredient()).ReturnsAsync(new List<Guid>());

//            Func<Task> act = async () => await _service.CreateProduct(dto, ingredients);

//            await act.Should().ThrowAsync<ArgumentException>().WithMessage("*không có sẵn*");
//        }

//        [Fact]
//        public async Task CreateProduct_ShouldThrowArgumentException_WhenProductAlreadyExists()
//        {
//            var dto = new CreateProductDto { Name = "DuplicateProduct", CategoryId = Guid.NewGuid() };
//            var validId = Guid.NewGuid();
//            var ingredients = new List<ProductIngredientDTO> { new() { IngredientId = validId, QuantityRequired = 3 } };

//            _mockRepo.Setup(r => r.GetListIdIngredient()).ReturnsAsync(new List<Guid> { validId });
//            _mockRepo.Setup(r => r.ProductExistsWithSameIngredientsAndCategory(dto.Name, dto.CategoryId, ingredients)).ReturnsAsync(true);

//            Func<Task> act = async () => await _service.CreateProduct(dto, ingredients);

//            await act.Should().ThrowAsync<ArgumentException>().WithMessage("*sản phẩm đã tồn tại*");
//        }
//    }
//}

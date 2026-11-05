using AutoMapper;
using Domain.Entities;
using Product.API.Dtos.InventoryItem;
using Product.API.Dtos.Product;
using Product.API.Helpers;
using Product.API.Repositories.Interfaces;
using Product.API.Services.Interfaces;

namespace Product.API.Services.Behaviours
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;
        private readonly CloudinaryHelper _cloudinaryHelper;


        public ProductService(IProductRepository productRepository, IMapper mapper, ILogger<ProductService> logger, CloudinaryHelper cloudinaryHelper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
            _cloudinaryHelper = cloudinaryHelper;

        }
        public async Task<ProductResponse> GetProductsAsync(int page, int pageSize, string? search, Guid? category)
        {
            _logger.LogInformation("Fetching products with pagination");

            var products = await _productRepository.GetProductsAsync(search, category);
            if (products == null || !products.Any())
            {
                return null;
            }
            var totalItems = products.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var paginatedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name,
                Description = p.Description,
                Image = p.Image,
                IsDeleted = p.IsDeleted
            }).ToList();

            return new ProductResponse
            {
                Items = _mapper.Map<List<ProductDto>>(paginatedProducts),
                TotalItems = totalItems,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }
        public async Task<ProductDetailDto?> GetDetailProductsAsync(Guid id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null) return null;

            return new ProductDetailDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Status = product.Status,
                CreatedAt = product.CreatedAt,
                CategoryName = product.Category?.Name,
                Image = product.Image,
                Ingredients = product.ProductIngredients.Select(pi => new IngredientOfProductDto
                {
                    IngredientId = pi.Ingredient.Id,
                    IngredientName = pi.Ingredient.Name,
                    QuantityRequired = pi.QuantityRequired,
                    Unit = pi.Unit
                }).ToList()
            };
        }
        public async Task<List<IngredientOfProductDto>> GetProductIngredientsAsync(Guid productId)
        {
            var productIngredient = await _productRepository.GetProductWithIngredientsAsync(productId);
            if (productIngredient == null || productIngredient.ProductIngredients == null)
            {
                return new List<IngredientOfProductDto>();
            }

            return productIngredient.ProductIngredients.Select(pi => new IngredientOfProductDto
            {
                IngredientId = pi.Ingredient.Id,
                IngredientName = pi.Ingredient.Name,
                QuantityRequired = pi.QuantityRequired,
                Unit = pi.Unit
            }).ToList();
        }
        public async Task<bool> DeleteProduct(Guid id)
        {
            _logger.LogInformation("Entered method DeleteProduct in service for Product ID {Id}'ProductService'", id);

            try
            {
                var result = await _productRepository.DeleteProduct(id);
                if (!result)
                {
                    _logger.LogWarning("Product with ID {Id} not found in service.'ProductService'", id);
                    return false;
                }

                _logger.LogInformation("Successfully marked product with ID {Id} as deleted in service.'ProductService'", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in service layer {nameof(DeleteProduct)}: {ex.Message}, StackTrace: {ex.StackTrace}'DeleteProduct'");
                throw;
            }
        }
        public async Task<bool?> ToggleProductStatus(Guid id)
        {
            _logger.LogInformation("Entered ToggleProductStatus in ProductService for ID {Id}", id);

            try
            {
                var result = await _productRepository.ToggleProductStatus(id);
                if (result == null)
                {
                    _logger.LogWarning("Product with ID {Id} not found in service.", id);
                    return null;
                }

                _logger.LogInformation("Successfully toggled IsDeleted for product ID {Id} to {Status}", id, result);
                return result;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ToggleProductStatus: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<bool> CreateProduct(CreateProductDto productDto, List<ProductIngredientDTO> ingredients)
        {
            try
            {
                _logger.LogInformation("Entered method CreateProduct in service 'ProductService'");

                var existingIngredientIds = await _productRepository.GetListIdIngredient();
                var invalidIngredients = ingredients
                    .Where(i => !existingIngredientIds.Contains(i.IngredientId))
                    .Select(i => i.IngredientId)
                    .ToList();

                if (invalidIngredients.Any())
                {
                    _logger.LogError("Invalid Ingredient IDs: {IngredientIds}", string.Join(", ", invalidIngredients));
                    throw new ArgumentException($"Các ID thành phần sau đây không có sẵn: {string.Join(", ", invalidIngredients)}");
                }

                bool productExists = await _productRepository.ProductExistsWithSameIngredientsAndCategory(productDto.Name, productDto.CategoryId, ingredients);
                if (productExists)
                {
                    _logger.LogError("Product with name '{ProductName}', category '{CategoryId}', and same ingredients already exists.", productDto.Name, productDto.CategoryId);
                    throw new ArgumentException("Sản phẩm này được tạo ra từ cùng thành phần và cùng loại danh mục này .");
                }

                string? imageUrl = null;
                if (productDto.Image != null)
                {
                    var uploadResult = await _cloudinaryHelper.UploadImage(new List<IFormFile> { productDto.Image });
                    if (!uploadResult.IsSuccess || uploadResult.Files.Count == 0)
                    {
                        throw new InvalidOperationException("Failed to upload image to Cloudinary.");
                    }
                    imageUrl = uploadResult.Files.First().Url;
                }

                var ingredientUnits = await _productRepository.GetIngredientUnits(ingredients.Select(i => i.IngredientId).ToList());

                var product = _mapper.Map<Domain.Entities.Product>(productDto);
                product.Image = imageUrl ?? "https://res.cloudinary.com/dmutopvrr/image/upload/default-image.webp";

                var productIngredients = ingredients.Select(i => new ProductIngredient
                {
                    IngredientId = i.IngredientId,
                    QuantityRequired = i.QuantityRequired,
                    Unit = ingredientUnits.ContainsKey(i.IngredientId) ? ingredientUnits[i.IngredientId] : null
                }).ToList();

                _logger.LogInformation("Successfully added a product in service 'ProductService'");

                return await _productRepository.CreateProduct(product, productIngredients);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in {nameof(CreateProduct)}: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }
        //test 1 

        public async Task<List<InventoryItemWarningDto>> CheckInventoryForIngredients(List<ProductIngredientDTO> productIngredients)
        {
            _logger.LogInformation("Entered method CheckInventoryForIngredients in service 'ProductService'");

            var warnings = new List<InventoryItemWarningDto>();

            foreach (var ingredient in productIngredients)
            {
                var inventoryItem = await _productRepository.GetInventoryItemByIngredientId(ingredient.IngredientId);
                _logger.LogInformation($"Checking inventory for IngredientId: {ingredient.IngredientId}");
                var ingredientName = await _productRepository.GetIngredientNameById(ingredient.IngredientId);

                if (inventoryItem == null)
                {
                    warnings.Add(new InventoryItemWarningDto
                    {
                        IngredientName = ingredientName ?? ingredient.IngredientId.ToString(),
                        IsStockLow = false,
                        IsExpired = false,
                        WarningMessage = "Ingredient not found."
                    });
                }
                else
                {
                    var isStockLow = inventoryItem.Quantity < ingredient.QuantityRequired;
                    var isExpired = inventoryItem.ExpiryDate < DateTime.Now;

                    if (isStockLow || isExpired)
                    {
                        _logger.LogWarning($"Ingredient {ingredient.IngredientId}: Stock low: {isStockLow}, Expired: {isExpired}");
                    }

                    warnings.Add(new InventoryItemWarningDto
                    {
                        IngredientName = ingredientName ?? ingredient.IngredientId.ToString(),
                        IsStockLow = isStockLow,
                        IsExpired = isExpired,
                        WarningMessage = isStockLow ? "Low stock." : (isExpired ? "Ingredient expired." : "OK")
                    });
                }
            }
            _logger.LogInformation("Exit method CheckInventoryForIngredients in service 'ProductService'");

            return warnings;
        }

        public async Task<bool> UpdateProduct(Guid id, UpdateProductDto productDto, List<ProductIngredientDTO> ingredients)
        {
            try
            {
                _logger.LogInformation($"Updating product with ID: {id}");

                var existingProduct = await _productRepository.GetProductById(id);
                if (existingProduct == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy sản phẩm.");
                }

                if (ingredients == null || !ingredients.Any())
                {
                    throw new ArgumentException("Danh sách thành phần nguyên liệu  không được để trống.");
                }
                bool productExists = await _productRepository.ProductExistsWithSameIngredientsAndCategoryToUpdate(id, productDto.Name, productDto.CategoryId, ingredients);
                if (productExists)
                {
                    _logger.LogError("Product with name '{ProductName}', category '{CategoryId}', and same ingredients already exists.", productDto.Name, productDto.CategoryId);
                    throw new ArgumentException("Sản phẩm này được tạo ra từ cùng thành phần và cùng loại.");
                }

                var ingredientIds = ingredients.Select(i => i.IngredientId).ToList();
                var existingIngredientIds = await _productRepository.GetExistingIngredientIds(ingredientIds);
                var invalidIngredientIds = ingredientIds.Except(existingIngredientIds).ToList();
                if (invalidIngredientIds.Any())
                {
                    throw new ArgumentException($"ID thành phần không hợp lệ: {string.Join(", ", invalidIngredientIds)}");
                }

                var product = _mapper.Map<Domain.Entities.Product>(productDto);
                product.Id = id;

                string? imageUrl = existingProduct.Image;
                if (productDto.Image != null)
                {
                    var uploadResult = await _cloudinaryHelper.UploadImage(new List<IFormFile> { productDto.Image });
                    if (!uploadResult.IsSuccess || uploadResult.Files.Count == 0)
                    {
                        throw new InvalidOperationException("Failed to upload image to Cloudinary.");
                    }
                    imageUrl = uploadResult.Files.First().Url;
                }
                product.Image = imageUrl;

                var ingredientUnits = await _productRepository.GetIngredientUnits(ingredientIds);
                var productIngredients = ingredients.Select(i => new ProductIngredient
                {
                    IngredientId = i.IngredientId,
                    QuantityRequired = i.QuantityRequired,
                    Unit = ingredientUnits.ContainsKey(i.IngredientId) ? ingredientUnits[i.IngredientId] : null
                }).ToList();

                _logger.LogInformation("Successfully mapped updated product and ingredients.");

                return await _productRepository.UpdateProduct(id, product, productIngredients);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Update failed: {ex.Message}");
                throw;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Ingredient validation error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating product: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }



        public async Task<GetProductsBaseResponseDto> GetListProductBase(int page, int? pageSize, string? search, Guid? userId, int sortOrder = 0)
        {
            try
            {
                var productList = await _productRepository.GetListProductBaseRepo();

                if (productList == null || !productList.Any())
                {
                    _logger.LogInformation("No products found.");
                    return new GetProductsBaseResponseDto
                    {
                        Items = new List<ProductBaseDto>(),
                        TotalItems = 0,
                        CurrentPage = page,
                        PageSize = pageSize ?? 10,
                        TotalPages = 0
                    };
                }

                if (!string.IsNullOrWhiteSpace(search))
                {
                    productList = productList.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                productList = sortOrder switch
                {
                    1 => productList.OrderBy(p => p.Price).ToList(),          // Giá tăng dần
                    2 => productList.OrderByDescending(p => p.Price).ToList(), // Giá giảm dần
                    _ => productList.ToList()            // Mặc định: Tên tăng dần
                };
                var totalItems = productList.Count();
                var size = pageSize ?? 10;
                var totalPages = (int)Math.Ceiling(totalItems / (double)size);

                var paginatedProducts = productList
                    .Skip((page - 1) * size)
                    .Take(size)
                    .ToList();
                List<Guid> wishlistProductIds = new();
                if (userId.HasValue)
                {
                    wishlistProductIds = await _productRepository.GetWishlistProductIdsByUserAsync(userId.Value);
                }

                var productDtos = paginatedProducts.Select(p => new ProductBaseDto
                {
                    Id = p.Id,
                    CategoryName = p.Category?.Name,
                    Name = p.Name,
                    Price = p.Price,
                    Image = p.Image,
                    IsWishlist = userId.HasValue && wishlistProductIds.Contains(p.Id)
                }).ToList();

                _logger.LogInformation($"Successfully retrieved {totalItems} products.");

                return new GetProductsBaseResponseDto
                {
                    Items = productDtos,
                    TotalItems = totalItems,
                    CurrentPage = page,
                    PageSize = size,
                    TotalPages = totalPages
                };

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in service layer {nameof(GetListProductBase)}: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }


        public async Task<ProductDetailUserDto?> GetProductById(Guid productID)
        {
            try
            {
                var product = await _productRepository.GetProductDetailById(productID);
                if (product == null)
                {
                    _logger.LogInformation("No product found.");
                    return null;
                }

                var productDto = new ProductDetailUserDto
                {
                    Id = product.Id,
                    CategoryName = product.Category?.Name,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Image = product.Image
                };

                _logger.LogInformation($"Product found with ID: {productDto.Id}");
                return productDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(GetProductById)}.");
                throw;
            }
        }


        public async Task<IEnumerable<ProductBaseDto>> GetSimilarProducts(Guid productID)
        {
            try
            {
                var products = await _productRepository.GetProductById(productID);
                if (products == null)
                {
                    _logger.LogInformation("No products found for similar search.");
                    return Enumerable.Empty<ProductBaseDto>();
                }

                var categoryId = products.CategoryId;
                if (categoryId == null)
                {
                    _logger.LogInformation($"Product with ID {productID} has no category.");
                    return Enumerable.Empty<ProductBaseDto>();
                }

                var similarProducts = await _productRepository.GetProductsByCategory(categoryId);
                if (!similarProducts.Any())
                {
                    _logger.LogInformation($"No similar products found for category {categoryId}.");
                    return Enumerable.Empty<ProductBaseDto>();
                }

                var similarProductDtos = similarProducts.Select(p => new ProductBaseDto
                {
                    Id = p.Id,
                    CategoryName = p.Category?.Name,
                    Name = p.Name,
                    Price = p.Price,
                    Image = p.Image

                }).ToList();

                _logger.LogInformation($"Found {similarProductDtos.Count} similar products.");
                return similarProductDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in {nameof(GetSimilarProducts)}: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }


        public async Task<IEnumerable<ProductBaseDto>> GetProductsByCategoryId(Guid categoryId)
        {
            try
            {
                var products = await _productRepository.GetProductsByCategory(categoryId);
                if (products == null || !products.Any())
                {
                    _logger.LogInformation("No products found for the given category.");
                    return null;
                }

                var productDtos = products.Select(p => new ProductBaseDto
                {
                    Id = p.Id,
                    CategoryName = p.Category?.Name,
                    Name = p.Name,
                    Image = p.Image,
                    //Description = p.Description,
                    Price = p.Price,
                }).ToList();

                _logger.LogInformation($"Found {productDtos.Count} products in category {categoryId}");
                return productDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in {nameof(GetProductsByCategoryId)}: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }


        public async Task<IEnumerable<ProductBaseDto>> GetLatestProducts()
        {
            _logger.LogInformation("Entered method GetLatestProducts in service 'ProductService'");

            try
            {
                var products = await _productRepository.GetLatestProducts();
                if (products == null || !products.Any())
                {
                    _logger.LogInformation("No latest products found.");
                    return null;
                }

                return products.Select(p => new ProductBaseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryName = p.Category?.Name,
                    Image = p.Image
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in {nameof(GetLatestProducts)}: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<ProductBaseDto>> GetTopSellingProducts()
        {
            _logger.LogInformation("Entered method GetTopSellingProducts in service 'ProductService'");

            try
            {
                var products = await _productRepository.GetTopSellingProducts();
                if (products == null || !products.Any())
                {
                    _logger.LogInformation("No top-selling products found.");
                    return null;
                }

                return products.Select(p => new ProductBaseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryName = p.Category?.Name,
                    Image = p.Image
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in {nameof(GetTopSellingProducts)}: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> AddToWishlist(Guid userId, Guid productId)
        {
            _logger.LogInformation("Entered method AddToWishlist in service 'ProductService'");

            try
            {
                var existingWishList = await _productRepository.GetByUserAndProductInWishlist(userId, productId);
                if (existingWishList != null)
                {
                    _logger.LogInformation("Product is already in wishlist in this user.");
                    return false;
                }

                var wishListItem = new UserWishList
                {
                    CustomerId = userId,
                    ProductId = productId,
                    CreatedAt = DateTime.Now
                };

                await _productRepository.AddToWishlistAsync(wishListItem);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product to wishlist.");
                throw;
            }
        }
        public async Task<GetProductsBaseResponseDto> GetWishlistByUserIdAsync(Guid userId, int page, int? pageSize, string? search)
        {
            try
            {
                var wishlistItems = await _productRepository.GetWishlistByUserIdAsync(userId);

                if (wishlistItems == null || !wishlistItems.Any())
                {
                    _logger.LogInformation($"No wishlist items found for userId: {userId}");
                    return new GetProductsBaseResponseDto
                    {
                        Items = new List<ProductBaseDto>(),
                        TotalItems = 0,
                        CurrentPage = page,
                        PageSize = pageSize ?? 10,
                        TotalPages = 0
                    };
                }

                if (!string.IsNullOrWhiteSpace(search))
                {
                    wishlistItems = wishlistItems.Where(w => w.Product.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                var totalItems = wishlistItems.Count();
                var size = pageSize ?? 10;
                var totalPages = (int)Math.Ceiling(totalItems / (double)size);

                var paginatedWishlist = wishlistItems
                    .Skip((page - 1) * size)
                    .Take(size)
                    .ToList();

                var productDtos = paginatedWishlist.Select(w => new ProductBaseDto
                {
                    Id = w.Product.Id,
                    CategoryName = w.Product.Category?.Name,
                    Name = w.Product.Name,
                    Price = w.Product.Price,
                    Image = w.Product.Image
                }).ToList();

                return new GetProductsBaseResponseDto
                {
                    Items = productDtos,
                    TotalItems = totalItems,
                    CurrentPage = page,
                    PageSize = size,
                    TotalPages = totalPages
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in {nameof(GetWishlistByUserIdAsync)}: {ex.Message}");
                throw;
            }
        }


        public async Task<bool> DeleteWishlistItemAsync(Guid userId, Guid productId)
        {
            _logger.LogInformation($"Attempting to permanently delete product {productId} from wishlist for user {userId}");

            try
            {
                var wishlistItem = await _productRepository.GetByUserAndProductInWishlist(userId, productId);
                if (wishlistItem == null)
                {
                    _logger.LogWarning($"Wishlist item not found for user {userId} and product {productId}");
                    return false;
                }
                await _productRepository.DeleteWishlistItemAsync(wishlistItem);


                _logger.LogInformation($"Successfully hidden product {productId} for user {userId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in HideWishlistItemAsync: {ex.Message}");
                throw;
            }
        }

    }
}

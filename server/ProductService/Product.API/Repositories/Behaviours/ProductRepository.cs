using Domain.Constanst;
using Domain.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Product.API.Dtos.Product;
using Product.API.Repositories.Interfaces;
using System.Linq;


namespace Product.API.Repositories.Behaviours
{
    public class ProductRepository : IProductRepository
    {
        private readonly ILogger<ProductRepository> _logger;
        private readonly SCMDbContext _context;

        public ProductRepository(ILogger<ProductRepository> logger, SCMDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<List<Domain.Entities.Product>> GetProductsAsync(string? search, Guid? category)
        {
            try
            {
                var query = _context.Products.AsNoTracking()
                    .Include(p => p.Category)
                    .OrderBy(p => p.IsDeleted)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                    query = query.Where(p => p.Name.Contains(search));

                if (category.HasValue)
                    query = query.Where(p => p.CategoryId == category.Value);

                var result = await query.ToListAsync();
                _logger.LogInformation("Successfully retrieved {Count} products", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching products");
                return new List<Domain.Entities.Product>();
            }
        }

        public async Task<Domain.Entities.Product> GetProductByIdAsync(Guid id)
        {
            return await _context.Products
                .Where(p => !p.IsDeleted && p.Id == id)
                .Include(p => p.Category)
                .Include(p => p.ProductIngredients)
                    .ThenInclude(pi => pi.Ingredient)
                .FirstOrDefaultAsync();
        }


        public async Task<Domain.Entities.Product?> GetProductWithIngredientsAsync(Guid productId)
        {
            return await _context.Products
                .Where(p => !p.IsDeleted && p.Id == productId)
                .Include(p => p.ProductIngredients)
                    .ThenInclude(pi => pi.Ingredient)
                .FirstOrDefaultAsync();
        }
        public async Task<bool> DeleteProduct(Guid id)
        {
            _logger.LogInformation("Entered method DeleteProduct in 'ProductRepository'");

            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(v => v.Id == id);
                if (product == null)
                {
                    _logger.LogWarning("Product with ID {Id} not found.'ProductRepository'", id);
                    return false;
                }

                product.IsDeleted = true;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully marked product with ID {Id} as deleted. 'ProductRepository'", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while marking the product as deleted. 'ProductRepository'");
                throw;
            }
        }

        public async Task<bool> CreateProduct(Domain.Entities.Product product, List<ProductIngredient> productIngredients)
        {
            _logger.LogInformation("Entered method CreateProduct in ProductRepository");
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var newProduct = new Domain.Entities.Product
                {
                    CategoryId = product.CategoryId,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Image = product.Image,
                    Status = 1,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = AppConstants.SYSTEM_ROLE_GUID,
                    UpdatedBy = AppConstants.SYSTEM_ROLE_GUID,
                    IsDeleted = false
                };

                await _context.Products.AddAsync(newProduct);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully created Product with ID {ProductId}.", newProduct.Id);

                foreach (var ingredient in productIngredients)
                {
                    var productIngredient = new ProductIngredient
                    {
                        ProductId = newProduct.Id,
                        IngredientId = ingredient.IngredientId,
                        QuantityRequired = ingredient.QuantityRequired,
                        Unit = ingredient.Unit,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        CreatedBy = AppConstants.SYSTEM_ROLE_GUID,
                        UpdatedBy = AppConstants.SYSTEM_ROLE_GUID,
                        IsDeleted = false
                    };

                    await _context.ProductIngredients.AddAsync(productIngredient);
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                _logger.LogInformation("Successfully added Product with ingredients.");
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "An error occurred while creating the product and its ingredients.");
                return false;
            }
        }

        public async Task<bool> ProductExistsWithSameIngredients(string productName, List<ProductIngredientDTO> ingredients)
        {
            _logger.LogInformation("Checking if product '{ProductName}' exists with same ingredients.", productName);

            var existingProducts = await _context.Products
      .Where(p => p.Name == productName && !p.IsDeleted)
      .Select(p => p.Id)
      .ToListAsync();

            if (!existingProducts.Any())
            {
                return false;
            }

            var allExistingIngredients = await _context.ProductIngredients
     .Where(pi => existingProducts.Contains(pi.ProductId) && !pi.IsDeleted)
     .ToListAsync();

            var existingIngredientGroups = allExistingIngredients
          .GroupBy(pi => pi.ProductId)
          .Select(g => g.Select(pi => new { pi.IngredientId, pi.QuantityRequired }).OrderBy(i => i.IngredientId).ToList())
          .ToList();

            var requestedIngredients = ingredients
              .Select(i => new { i.IngredientId, i.QuantityRequired })
              .OrderBy(i => i.IngredientId)
              .ToList();

            bool isDuplicate = existingIngredientGroups.Any(existing => existing.SequenceEqual(requestedIngredients));

            return isDuplicate;
        }
        public async Task<bool> ProductExistsWithSameIngredientsAndCategoryToUpdate(Guid productId, string productName, Guid categoryId, List<ProductIngredientDTO> ingredients)
        {
            _logger.LogInformation("Checking if other product '{ProductName}' with category '{CategoryId}' exists with same ingredients.", productName, categoryId);

            var existingProducts = await _context.Products
                .Where(p => p.Id != productId && p.Name == productName && p.CategoryId == categoryId && !p.IsDeleted)
                .Select(p => p.Id)
                .ToListAsync();

            if (!existingProducts.Any())
            {
                return false;
            }

            var allExistingIngredients = await _context.ProductIngredients
                .Where(pi => existingProducts.Contains(pi.ProductId) && !pi.IsDeleted)
                .ToListAsync();

            var existingIngredientGroups = allExistingIngredients
                .GroupBy(pi => pi.ProductId)
                .Select(g => g
                    .Select(pi => new { pi.IngredientId, pi.QuantityRequired })
                    .OrderBy(i => i.IngredientId)
                    .ToList()
                )
                .ToList();

            var requestedIngredients = ingredients
                .Select(i => new { i.IngredientId, i.QuantityRequired })
                .OrderBy(i => i.IngredientId)
                .ToList();

            bool isDuplicate = existingIngredientGroups.Any(existing => existing.SequenceEqual(requestedIngredients));

            return isDuplicate;
        }

        public async Task<bool> ProductExistsWithSameIngredientsAndCategory(string productName, Guid categoryId, List<ProductIngredientDTO> ingredients)
        {
            _logger.LogInformation("Checking if product '{ProductName}' with category '{CategoryId}' exists with same ingredients.", productName, categoryId);

            var existingProducts = await _context.Products
                .Where(p => p.Name == productName && p.CategoryId == categoryId && !p.IsDeleted)
                .Select(p => p.Id)
                .ToListAsync();

            if (!existingProducts.Any())
            {
                return false; 
            }

            var allExistingIngredients = await _context.ProductIngredients
                .Where(pi => existingProducts.Contains(pi.ProductId) && !pi.IsDeleted)
                .ToListAsync();

            var existingIngredientGroups = allExistingIngredients
                .GroupBy(pi => pi.ProductId)
                .Select(g => g.Select(pi => new { pi.IngredientId, pi.QuantityRequired }).OrderBy(i => i.IngredientId).ToList())
                .ToList();

            var requestedIngredients = ingredients
                .Select(i => new { i.IngredientId, i.QuantityRequired })
                .OrderBy(i => i.IngredientId)
                .ToList();

            bool isDuplicate = existingIngredientGroups.Any(existing => existing.SequenceEqual(requestedIngredients));

            return isDuplicate;
        }


        // for add product

        public async Task<InventoryItem> GetInventoryItemByIngredientId(Guid ingredientId)
        {
            return await _context.InventoryItem.Include(i => i.Inventory)
                .Include(i => i.Ingredient)
                .FirstOrDefaultAsync(i => i.IngredientId == ingredientId);
        }
        public async Task<string> GetIngredientNameById(Guid ingredientId)
        {
            var ingredient = await _context.Ingredients
                .Where(i => i.Id == ingredientId)
                .FirstOrDefaultAsync();

            if (ingredient == null)
            {
                return null;
            }

            return ingredient.Name;
        }
        public async Task<bool?> ToggleProductStatus(Guid id)
        {
            _logger.LogInformation("Entered ToggleProductStatus in ProductRepository");

            try
            {
                var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
                if (product == null)
                {
                    _logger.LogWarning("Product with ID {Id} not found.", id);
                    return null;
                }
                if (product.IsDeleted && product.Category != null && product.Category.IsDeleted)
                {
                    _logger.LogWarning("Cannot activate product ID {Id} because its category is deactivated.", id);
                    throw new InvalidOperationException("Danh mục của sản phẩm này đang bị vô hiệu hóa. Vui lòng kích hoạt danh mục trước.");
                }
                product.IsDeleted = !product.IsDeleted;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Toggled IsDeleted status for product ID {Id} to {Status}", id, product.IsDeleted);
                return product.IsDeleted;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex.Message);
                throw; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while toggling IsDeleted.");
                throw;
            }
        }

        public async Task<List<Guid>> GetListIdIngredient()
        {
            _logger.LogInformation("Entered method GetListIdIngredient in ProductRepository");

            try
            {
                var ids = await _context.Ingredients
                    .Where(v => !v.IsDeleted)
                     .Select(v => v.Id)
                    .ToListAsync();

                _logger.LogInformation("Successfully retrieved {Count} ids of Ingredient .", ids.Count);
                return ids;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving id Ingredient.");
                throw;
            }
        }

        //
        public async Task<List<Guid>> GetExistingIngredientIds(List<Guid> ingredientIds)
        {
            return await _context.Ingredients
                .Where(i => ingredientIds.Contains(i.Id))
                .Select(i => i.Id)
                .ToListAsync();
        }
        public async Task<bool> UpdateProduct(Guid id, Domain.Entities.Product updatedProduct, List<ProductIngredient> updatedIngredients)
        {
            _logger.LogInformation("Entered method UpdateProduct in 'ProductRepository'");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var existingProduct = await _context.Products
                    .Include(p => p.ProductIngredients)
                    .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

                if (existingProduct == null)
                {
                    _logger.LogWarning("Product with ID {Id} not found or is marked as deleted.", id);
                    return false;
                }

                existingProduct.Name = updatedProduct.Name;
                existingProduct.Description = updatedProduct.Description;
                existingProduct.Price = updatedProduct.Price;
                existingProduct.Image = updatedProduct.Image;
                existingProduct.CategoryId = updatedProduct.CategoryId;
                existingProduct.Status = updatedProduct.Status;
                existingProduct.UpdatedAt = DateTime.Now;
                existingProduct.UpdatedBy = AppConstants.SYSTEM_ROLE_GUID;

                _context.Products.Update(existingProduct);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully updated Product with ID {ProductId}.", existingProduct.Id);

                // Remove existing ingredients that are no longer part of the product
                var existingIngredientIds = existingProduct.ProductIngredients.Select(pi => pi.IngredientId).ToList();
                var ingredientsToRemove = existingProduct.ProductIngredients
                    .Where(pi => !updatedIngredients.Any(ui => ui.IngredientId == pi.IngredientId)).ToList();

                _context.ProductIngredients.RemoveRange(ingredientsToRemove);

                foreach (var ingredient in updatedIngredients)
                {
                    var existingIngredient = existingProduct.ProductIngredients
                        .FirstOrDefault(pi => pi.IngredientId == ingredient.IngredientId);

                    if (existingIngredient != null)
                    {
                        existingIngredient.QuantityRequired = ingredient.QuantityRequired;
                        existingIngredient.Unit = ingredient.Unit;
                        existingIngredient.UpdatedAt = DateTime.Now;
                        existingIngredient.UpdatedBy = AppConstants.SYSTEM_ROLE_GUID;
                    }
                    else
                    {
                        var productIngredient = new ProductIngredient
                        {
                            ProductId = existingProduct.Id,
                            IngredientId = ingredient.IngredientId,
                            QuantityRequired = ingredient.QuantityRequired,
                            Unit = ingredient.Unit,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            CreatedBy = AppConstants.SYSTEM_ROLE_GUID,
                            UpdatedBy = AppConstants.SYSTEM_ROLE_GUID,
                            IsDeleted = false
                        };
                        await _context.ProductIngredients.AddAsync(productIngredient);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Successfully updated product and its ingredients.");
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "An error occurred while updating the product and its ingredients.");
                return false;
            }
        }

        public async Task<List<Domain.Entities.Product>> GetListProductBaseRepo()
        {
            _logger.LogInformation("Entered method GetListProductBase in ProductRepository");

            try
            {
                var products = await _context.Products
                    .Where(v => !v.IsDeleted)
                    .Include(c => c.Category)
                    .ToListAsync();

                _logger.LogInformation("Successfully retrieved {Count} products.", products.Count);
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving products.");
                throw;
            }
        }

        public async Task<List<Guid>> GetWishlistProductIdsByUserAsync(Guid userId)
        {
            return await _context.UserWishLists
                .Where(w => w.CustomerId == userId && !w.Product.IsDeleted && !w.IsDeleted)
                .Select(w => w.ProductId)
                .ToListAsync();
        }


        public async Task<Domain.Entities.Product?> GetProductById(Guid productID)
        {
            _logger.LogInformation("Entered method GetProductById in ProductRepository");

            try
            {
                return await _context.Products
                    .Include(c => c.Category)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == productID && !p.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in {nameof(GetProductById)}: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw new Exception("An error occurred while fetching the product.", ex);
            }
        }



        public async Task<Domain.Entities.Product> GetProductDetailById(Guid productID)
        {
            _logger.LogInformation("Entered method GetProductById in ProductRepository");

            try
            {
                var product = await _context.Products.Include(c => c.Category)
                    .Where(p => p.Id == productID && !p.IsDeleted).FirstOrDefaultAsync();


                return product;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching the product.", ex);
            }
        }

        public async Task<IEnumerable<Domain.Entities.Product>> GetProductsByCategory(Guid? categoryId)
        {
            try
            {
                var products = await _context.Products.Include(c => c.Category)
                    .Where(p => p.CategoryId == categoryId && !p.IsDeleted)
                    .ToListAsync();
                return products;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching similar products.", ex);
            }
        }
        public async Task<IEnumerable<Domain.Entities.Product>> GetLatestProducts()
        {
            _logger.LogInformation("Entered method GetLatestProducts in ProductRepository");

            try
            {
                return await _context.Products.Include(c => c.Category)
                    .Where(p => !p.IsDeleted)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(4)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching the latest products.", ex);
            }
        }

        public async Task<IEnumerable<Domain.Entities.Product>> GetTopSellingProducts()
        {
            _logger.LogInformation("Entered method GetTopSellingProducts in ProductRepository");
            try
            {
                var topProducts = await _context.OrderDetails
                    .GroupBy(od => od.ProductId)
                    .Select(g => new
                    {
                        ProductId = g.Key,
                        TotalQuantity = g.Sum(od => od.Quantity)
                    })
                    .OrderByDescending(g => g.TotalQuantity)
                    .Take(4)
                    .Join(_context.Products.Include(c => c.Category).Where(p => !p.IsDeleted),
                          od => od.ProductId,
                          p => p.Id,
                          (od, p) => p)
                    .ToListAsync();

                return topProducts;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching the top-selling products.", ex);
            }
        }

        public async Task<UserWishList?> GetByUserAndProductInWishlist(Guid userId, Guid productId)
        {
            _logger.LogInformation("Entered method GetByUserAndProduct in ProductRepository");

            return await _context.UserWishLists
                .FirstOrDefaultAsync(w => w.CustomerId == userId && w.ProductId == productId && !w.IsDeleted);
        }

        public async Task AddToWishlistAsync(UserWishList wishListItem)
        {
            await _context.UserWishLists.AddAsync(wishListItem);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<UserWishList>> GetWishlistByUserIdAsync(Guid userId)
        {
            return await _context.UserWishLists
                .Where(w => w.CustomerId == userId && !w.IsDeleted)
                .Include(w => w.Product).ThenInclude(c => c.Category)
                .ToListAsync();
        }
        public async Task UpdateWishlistItemAsync(UserWishList wishlistItem)
        {
            _context.UserWishLists.Update(wishlistItem);
            await _context.SaveChangesAsync();
        }

        public async Task<Dictionary<Guid, string>> GetIngredientUnits(List<Guid> ingredientIds)
        {
            if (ingredientIds == null || !ingredientIds.Any())
            {
                return new Dictionary<Guid, string>();
            }

            return await _context.Ingredients
                .Where(i => ingredientIds.Contains(i.Id))
                .ToDictionaryAsync(i => i.Id, i => i.Unit);
        }
        public async Task DeleteWishlistItemAsync(UserWishList wishlistItem)
        {
            _context.UserWishLists.Remove(wishlistItem);
            await _context.SaveChangesAsync();
        }
    }
}

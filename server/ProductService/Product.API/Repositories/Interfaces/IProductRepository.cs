using Domain.Entities;
using Product.API.Dtos.Product;
namespace Product.API.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Domain.Entities.Product>> GetProductsAsync(string? search, Guid? category);
        Task<bool> DeleteProduct(Guid id);
        Task<bool> CreateProduct(Domain.Entities.Product product, List<ProductIngredient> productIngredients);
        Task<InventoryItem> GetInventoryItemByIngredientId(Guid ingredientId);
        Task<string> GetIngredientNameById(Guid ingredientId);
        Task<List<Guid>> GetListIdIngredient();
        Task<bool> UpdateProduct(Guid id, Domain.Entities.Product updatedProduct, List<ProductIngredient> updatedIngredients);
        Task<Domain.Entities.Product> GetProductByIdAsync(Guid id);
        Task<bool> ProductExistsWithSameIngredients(string productName, List<ProductIngredientDTO> ingredients);
        Task<Dictionary<Guid, string>> GetIngredientUnits(List<Guid> ingredientIds);
        Task<Domain.Entities.Product?> GetProductWithIngredientsAsync(Guid productId);
        Task<List<Guid>> GetExistingIngredientIds(List<Guid> ingredientIds);
        Task<bool> ProductExistsWithSameIngredientsAndCategory(string productName, Guid categoryId, List<ProductIngredientDTO> ingredients);
        Task<bool> ProductExistsWithSameIngredientsAndCategoryToUpdate(Guid productId, string productName, Guid categoryId, List<ProductIngredientDTO> ingredients);
        Task<bool?> ToggleProductStatus(Guid id);
        //user 
        Task<List<Domain.Entities.Product>> GetListProductBaseRepo();
        //  Task<Domain.Entities.Product?> GetProductById(Guid productID);
        Task<Domain.Entities.Product> GetProductDetailById(Guid productID);
        Task<Domain.Entities.Product?> GetProductById(Guid productID);
        Task<IEnumerable<Domain.Entities.Product>> GetProductsByCategory(Guid? categoryId);
        Task<IEnumerable<Domain.Entities.Product>> GetLatestProducts();
        Task<IEnumerable<Domain.Entities.Product>> GetTopSellingProducts();
        Task<UserWishList?> GetByUserAndProductInWishlist(Guid userId, Guid productId);
        Task AddToWishlistAsync(UserWishList wishListItem);
        Task<IEnumerable<UserWishList>> GetWishlistByUserIdAsync(Guid userId);
        Task UpdateWishlistItemAsync(UserWishList wishlistItem);
        Task<List<Guid>> GetWishlistProductIdsByUserAsync(Guid userId);
        Task DeleteWishlistItemAsync(UserWishList wishlistItem);

    }
}

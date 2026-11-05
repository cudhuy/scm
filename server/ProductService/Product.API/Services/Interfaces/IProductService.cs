using Domain.Entities;
using Product.API.Dtos.InventoryItem;
using Product.API.Dtos.Product;

namespace Product.API.Services.Interfaces
{
    public interface IProductService
    {
        Task<ProductResponse> GetProductsAsync(int page, int pageSize, string? search, Guid? category);
        Task<bool> DeleteProduct(Guid id);
        Task<bool> CreateProduct(CreateProductDto productDto, List<ProductIngredientDTO> ingredients);
        Task<List<InventoryItemWarningDto>> CheckInventoryForIngredients(List<ProductIngredientDTO> productIngredients);
        Task<bool> UpdateProduct(Guid id, UpdateProductDto productDto, List<ProductIngredientDTO> ingredients);
        Task<ProductDetailDto?> GetDetailProductsAsync(Guid id);
        Task<List<IngredientOfProductDto>> GetProductIngredientsAsync(Guid productId);
        Task<bool?> ToggleProductStatus(Guid id);
        //user
        Task<GetProductsBaseResponseDto> GetListProductBase(int page, int? pageSize, string? search, Guid? userId, int sortOrder = 0);
        Task<ProductDetailUserDto> GetProductById(Guid productID);
        Task<IEnumerable<ProductBaseDto>> GetSimilarProducts(Guid productID);
        Task<IEnumerable<ProductBaseDto>> GetProductsByCategoryId(Guid categoryId);
        Task<IEnumerable<ProductBaseDto>> GetLatestProducts();
        Task<IEnumerable<ProductBaseDto>> GetTopSellingProducts();
        Task<bool> AddToWishlist(Guid userId, Guid productId);
        Task<GetProductsBaseResponseDto> GetWishlistByUserIdAsync(Guid userId, int page, int? pageSize, string? search);
        Task<bool> DeleteWishlistItemAsync(Guid userId, Guid productId);
    }
}

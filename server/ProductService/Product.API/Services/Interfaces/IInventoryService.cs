using Product.API.Dtos.Ingredient;
using Product.API.Dtos.InventoryItem;

namespace Product.API.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<List<LowStockInventoryItemDto>> GetAllExpiredIngredients();
        Task<GetInventoryItemsResponseDto> GetAllIngredients(int page, int? pageSize, string? search, Guid? branchId);
        Task<GetInventoryReceiptResponseDto> GetAllInventoryReceipts(int page, int? pageSize, string? search, Guid? branchId);
        Task<IngredientDetailDto> GetIngredientDetail(Guid branchId, Guid ingredientId);
        Task<InventoryStatisticsDto> GetInventoryStatistics();
        Task<GetInventoryItemsLowStockResponseDto> GetLowStockIngredients(int page, int? pageSize, string? search, Guid? branchId);
        Task<bool> RestockInventory(RestockInventoryDto restockInventoryDto, Guid userId);
    }
}

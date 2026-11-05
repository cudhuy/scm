using Domain.Entities;
using Product.API.Dtos.Ingredient;
using Product.API.Dtos.InventoryItem;

namespace Product.API.Repositories.Interfaces
{
    public interface IInventoryRepository
    {
        Task<List<LowStockInventoryItemDto>> GetAllExpiredIngredients();
        Task<List<Ingredient>> GetAllIngredients();
        Task<List<InventoryReceipt>> GetAllInventoryReceipts();
        Task<IngredientDetailDto> GetIngredientDetail(Guid branchId, Guid ingredientId);
        Task<string> GetInventoryReceicer(Guid createdBy);
        Task<InventoryStatisticsDto> GetInventoryStatistics();
        Task<List<LowStockAlert>> GetLowStockIngredients();
        Task<bool> RestockInventory(RestockInventoryDto restockInventoryDto, Guid userId);
        Task<bool> Save();
    }
}

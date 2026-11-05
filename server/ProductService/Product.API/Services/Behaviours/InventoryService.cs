using Product.API.Dtos.Ingredient;
using Product.API.Dtos.InventoryItem;
using Product.API.Repositories.Interfaces;
using Product.API.Services.Interfaces;

namespace Product.API.Services.Behaviours
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(IInventoryRepository inventoryRepository, ILogger<InventoryService> logger)
        {
            _inventoryRepository = inventoryRepository;
            _logger = logger;
        }

        public async Task<List<LowStockInventoryItemDto>> GetAllExpiredIngredients()
        {
            return await _inventoryRepository.GetAllExpiredIngredients();
        }

        public async Task<GetInventoryItemsResponseDto> GetAllIngredients(int page, int? pageSize, string? search, Guid? branchId)
        {
            try
            {
                var ingredients = await _inventoryRepository.GetAllIngredients();

                if (ingredients == null || !ingredients.Any())
                {
                    _logger.LogWarning("No ingredients found");
                    return new GetInventoryItemsResponseDto
                    {
                        Items = new List<InventoryItemDto>(),
                        TotalItems = 0,
                        CurrentPage = page,
                        PageSize = pageSize ?? 10,
                        TotalPages = 0
                    };
                }

                if (!string.IsNullOrWhiteSpace(search)) ingredients = ingredients.Where(i => i.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

                if (branchId.HasValue)
                {
                    foreach (var ingredient in ingredients)
                    {
                        ingredient.InventoryItems = ingredient.InventoryItems
                                                    .Where(ii => ii.Inventory.BranchId == branchId)
                                                    .ToList();
                    }
                    // Loại bỏ Ingredient không có InventoryItems nào khớp
                    ingredients = ingredients
                                .Where(i => i.InventoryItems.Any())
                                .ToList();
                }

                var totalItems = ingredients.Count();
                var size = pageSize ?? 10;
                var totalPages = (int)Math.Ceiling(totalItems / (double)size);

                var paginatedIngredients = ingredients
                    .Skip((page - 1) * size)
                    .Take(size)
                    .ToList();

                var inventoryItemDtos = paginatedIngredients.Select(ingredient =>
                {
                    var totalQuantity = ingredient.InventoryItems.Sum(ii => ii.Quantity);

                    var relevantReceipts = ingredient.InventoryReceipts
                           .Where(ir => ir.IngredientId == ingredient.Id) // Chắc chắn chỉ lấy đúng nguyên liệu
                           .ToList();

                    // Tính trung bình giá nhập của nguyên liệu đó
                    var averageUnitPrice = relevantReceipts.Any()
                        ? relevantReceipts.Average(ir => ir.UnitPrice)
                        : 0;

                    // Lấy tên chi nhánh từ inventory item đầu tiên (vì tất cả item đã filter theo branchId)
                    var branchName = ingredient.InventoryItems.FirstOrDefault()?.Inventory.Branch.Name ?? "N/A";

                    return new InventoryItemDto
                    {
                        IngredientId = ingredient.Id,
                        IngredientName = ingredient.Name,
                        IngredientQuantity = $"{totalQuantity} {ingredient.Unit}",
                        Price = $"{averageUnitPrice:N0} đ",   // Tổng giá trị
                        BranchName = branchName
                    };
                }).ToList();

                return new GetInventoryItemsResponseDto
                {
                    Items = inventoryItemDtos,
                    TotalItems = totalItems,
                    CurrentPage = page,
                    PageSize = size,
                    TotalPages = totalPages
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all ingredients");
                throw;
            }

        }

        public async Task<GetInventoryReceiptResponseDto> GetAllInventoryReceipts(int page, int? pageSize, string? search, Guid? branchId)
        {
            try
            {
                var inventoryReceipts = await _inventoryRepository.GetAllInventoryReceipts();

                if (inventoryReceipts == null || !inventoryReceipts.Any())
                {
                    _logger.LogWarning("No inventory receipts found");
                    return new GetInventoryReceiptResponseDto
                    {
                        Items = new List<InventoryReceiptDto>(),
                        TotalItems = 0,
                        CurrentPage = page,
                        PageSize = pageSize ?? 10,
                        TotalPages = 0
                    };
                }

                if (!string.IsNullOrWhiteSpace(search)) inventoryReceipts = inventoryReceipts.Where(ir => ir.Ingredient.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

                if (branchId.HasValue)
                {
                    inventoryReceipts = inventoryReceipts
                        .Where(ir => ir.Inventory.BranchId == branchId)
                        .ToList();
                }

                var totalItems = inventoryReceipts.Count();
                var size = pageSize ?? 10;
                var totalPages = (int)Math.Ceiling(totalItems / (double)size);

                var paginatedInventoryReceipts = inventoryReceipts
                    .Skip((page - 1) * size)
                    .Take(size)
                    .ToList();



                var inventoryReceiptDtos = await Task.WhenAll(paginatedInventoryReceipts.Select(async inventoryReceipt =>
                {
                    return new InventoryReceiptDto
                    {
                        CreateAt = inventoryReceipt.CreatedAt.ToString("dd-MM-yyyy"),
                        IngredientName = inventoryReceipt.Ingredient.Name,
                        IngredientQuantity = $"{inventoryReceipt.QuantityReceived} {inventoryReceipt.Ingredient.Unit}",
                        BranchName = inventoryReceipt.Inventory.Branch.Name,
                        UnitPrice = $"{inventoryReceipt.UnitPrice.ToString("N0")} đ/{inventoryReceipt.Ingredient.Unit}",
                        TotalPrice = $"{(inventoryReceipt.QuantityReceived * inventoryReceipt.UnitPrice).ToString("N0")} đ",
                        InventoryReceiver = GetInventoryReceiver(inventoryReceipt.CreatedBy)
                    };
                }).ToList());

                return new GetInventoryReceiptResponseDto
                {
                    Items = inventoryReceiptDtos,
                    TotalItems = totalItems,
                    CurrentPage = page,
                    PageSize = size,
                    TotalPages = totalPages
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all inventory receipts");
                throw;
            }
        }

        public async Task<IngredientDetailDto> GetIngredientDetail(Guid branchId, Guid ingredientId)
        {
            return await _inventoryRepository.GetIngredientDetail(branchId, ingredientId);
        }

        public async Task<InventoryStatisticsDto> GetInventoryStatistics()
        {
            return await _inventoryRepository.GetInventoryStatistics();
        }

        public async Task<GetInventoryItemsLowStockResponseDto> GetLowStockIngredients(int page, int? pageSize, string? search, Guid? branchId)
        {
            try
            {
                var lowStockIngredients = await _inventoryRepository.GetLowStockIngredients();

                if (lowStockIngredients == null || !lowStockIngredients.Any())
                {
                    _logger.LogWarning("No low stock ingredients found");
                    return new GetInventoryItemsLowStockResponseDto
                    {
                        Items = new List<InventoryItemLowStockDto>(),
                        TotalItems = 0,
                        CurrentPage = page,
                        PageSize = pageSize ?? 10,
                        TotalPages = 0

                    };
                }

                if (!string.IsNullOrWhiteSpace(search)) lowStockIngredients = lowStockIngredients.Where(i => i.Ingredient.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

                if (branchId.HasValue)
                {
                    foreach (var lowStockIngredient in lowStockIngredients)
                    {
                        lowStockIngredient.Inventory.InventoryItems = lowStockIngredient.Inventory.InventoryItems
                                                    .Where(ii => ii.Inventory.BranchId == branchId)
                                                    .ToList();
                    }

                    // Loại bỏ Ingredient không có InventoryItems nào khớp
                    lowStockIngredients = lowStockIngredients
                                .Where(i => i.Inventory.InventoryItems.Any())
                                .ToList();

                }

                var totalItems = lowStockIngredients.Count();
                var size = pageSize ?? 10;
                var totalPages = (int)Math.Ceiling(totalItems / (double)size);

                var paginatedLowStockIngredients = lowStockIngredients
                    .Skip((page - 1) * size)
                    .Take(size)
                    .ToList();

                var inventoryItemWarningDtos = paginatedLowStockIngredients.Select(lowStockIngredient =>
                {
                    //var lowStockInventoryItem = lowStockIngredient.Inventory.InventoryItems
                    //    .FirstOrDefault(ii => branchId == null || ii.Inventory.BranchId == branchId);

                    return new InventoryItemLowStockDto
                    {
                        //IngredientId = lowStockIngredient.Ingredient.Id,
                        IngredientName = lowStockIngredient.Ingredient.Name,
                        IngredientQuantity = $"{(lowStockIngredient?.InventoryItem.Quantity ?? 0)} {lowStockIngredient.Ingredient.Unit}",
                        BranchName = lowStockIngredient?.Inventory.Branch.Name ?? "N/A",
                        ImportDate = lowStockIngredient.InventoryItem.LastRestocked.ToString("dd-MM-yyyy"),
                        ExpiryDate = lowStockIngredient.InventoryItem.ExpiryDate.ToString("dd-MM-yyyy"),
                        AlertMessage = lowStockIngredient.AlertMessage
                    };
                }).ToList();

                return new GetInventoryItemsLowStockResponseDto
                {
                    Items = inventoryItemWarningDtos,
                    TotalItems = totalItems,
                    CurrentPage = page,
                    PageSize = size,
                    TotalPages = totalPages
                };


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting low stock ingredients");
                throw;
            }
        }

        public async Task<bool> RestockInventory(RestockInventoryDto restockInventoryDto, Guid userId)
        {
            return await _inventoryRepository.RestockInventory(restockInventoryDto, userId);
        }

        private string GetInventoryReceiver(Guid createdBy)
        {
            return _inventoryRepository.GetInventoryReceicer(createdBy).Result;
        }
    }
}

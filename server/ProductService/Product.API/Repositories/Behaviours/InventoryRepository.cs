using Domain.Constanst;
using Domain.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Product.API.Dtos.Ingredient;
using Product.API.Dtos.InventoryItem;
using Product.API.Repositories.Interfaces;

namespace Product.API.Repositories.Behaviours
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ILogger<InventoryRepository> _logger;
        private readonly SCMDbContext _context;

        public InventoryRepository(ILogger<InventoryRepository> logger, SCMDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<List<LowStockInventoryItemDto>> GetAllExpiredIngredients()
        {
            var result = await _context.LowStockAlerts
                        .Include(i => i.Ingredient)
                        .Include(i => i.Inventory)
                            .ThenInclude(inv => inv.Branch)
                        .Include(i => i.InventoryItem)
                        .OrderByDescending(i => i.CreatedAt)
                        .Select(i => new LowStockInventoryItemDto
                        {
                            InventoryName = i.Inventory.Branch.Name,
                            IngredientName = i.Ingredient.Name,
                            RecentQuantity = i.InventoryItem != null ? i.InventoryItem.Quantity : 0,
                            AlertQuantity = i.InventoryItem != null ? i.InventoryItem.LowStockThreshold : 0,
                            LastRestock = i.InventoryItem.LastRestocked != null
                                ? i.InventoryItem.LastRestocked.ToString("dd-MM-yyyy")
                                : string.Empty,
                            ExpireDate = i.InventoryItem.ExpiryDate != null
                                ? i.InventoryItem.ExpiryDate.ToString("dd-MM-yyyy")
                                : string.Empty,
                            AlertMessage = i.AlertMessage,
                            CreateAt = i.CreatedAt.ToString("dd-MM-yyyy")
                        })
                        .ToListAsync();
            return result;
        }


        public async Task<List<Ingredient>> GetAllIngredients()
        {

            //return await _context.Ingredients.ToListAsync();
            return await _context.Ingredients
                                .Include(i => i.InventoryItems)
                                    .ThenInclude(ii => ii.Inventory)
                                    .ThenInclude(inv => inv.Branch)
                                .Include(i => i.InventoryReceipts)
                                .Where(i => !i.IsDeleted)
                                .ToListAsync();

        }

        public async Task<List<InventoryReceipt>> GetAllInventoryReceipts()
        {
            var result = await _context.InventoryReceipts
                .Include(i => i.Inventory)
                    .ThenInclude(inv => inv.Branch)
                    .ThenInclude(b => b.Feedbacks)
                    .ThenInclude(f => f.User)
                .Include(i => i.Ingredient)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            return result;
        }

        public async Task<IngredientDetailDto> GetIngredientDetail(Guid branchId, Guid ingredientId)
        {
            var result = await _context.Ingredients
                .Where(i => i.Id == ingredientId)
                .Include(i => i.InventoryItems)
                    .ThenInclude(ii => ii.Inventory)
                    .ThenInclude(inv => inv.Branch)
                .Where(i => i.InventoryItems.Any(ii => ii.Inventory.BranchId == branchId) && !i.IsDeleted)
                .Include(i => i.InventoryReceipts)
                .Select(i => new
                {
                    IngredientId = i.Id,
                    Name = i.Name,

                    branchName = i.InventoryItems
                        .Where(ii => ii.Inventory.BranchId == branchId)
                        .Select(ii => ii.Inventory.Branch.Name)
                        .FirstOrDefault(),

                    Quantity = i.InventoryItems
                        .Where(ii => ii.Inventory.BranchId == branchId)
                        .GroupBy(ii => new { ii.InventoryId, ii.IngredientId })
                        .Select(g => g.Sum(ii => ii.Quantity))
                        .FirstOrDefault(),

                    LowStockThreshold = i.InventoryItems
                        .Where(ii => ii.Inventory.BranchId == branchId)
                        .GroupBy(ii => ii.IngredientId)
                        .Select(g => g.Min(ii => ii.LowStockThreshold))
                        .FirstOrDefault(),


                    // Thông tin các lô hàng
                    BatchDetails = i.InventoryItems
                        .Where(ii => ii.Inventory.BranchId == branchId)
                        .Select(ii => new BatchDetailDto
                        {
                            Quantity = $"{ii.Quantity} {_context.Ingredients.Where(i => i.Id == ii.IngredientId).Select(i => i.Unit).FirstOrDefault()}",
                            ImportDate = ii.InventoryReceipt.CreatedAt.ToString("yyyy-MM-dd"),
                            ExpiryDate = ii.InventoryReceipt.ExpiryDate.HasValue ? ii.ExpiryDate.ToString("yyyy-MM-dd") : "N/A",
                            UnitPrice = $"{ii.InventoryReceipt.UnitPrice:N0} đ",
                            Status = ii.InventoryReceipt.ExpiryDate.HasValue && ii.ExpiryDate > DateTime.UtcNow ? "Còn hạn" : "Hết hạn"
                        }).ToList(),

                    Supplier = i.InventoryReceipts
                        .Where(ir => ir.Inventory.BranchId == branchId)
                        .Select(ir => ir.Supplier)
                        .FirstOrDefault(),
                    AvgPrice = i.InventoryReceipts
                        .Where(ir => ir.Inventory.BranchId == branchId && ir.IngredientId == i.Id)
                        .Select(ir => (decimal?)ir.UnitPrice)
                        .Average() ?? 0,

                }).FirstOrDefaultAsync();

            if (result == null) return null;

            return new IngredientDetailDto
            {
                IngredientId = result.IngredientId,
                Name = result.Name,
                Quantity = $"{result.Quantity.ToString("0")} {_context.Ingredients.Where(i => i.Id == result.IngredientId).Select(i => i.Unit).FirstOrDefault()}",
                LowStockThreshold = $"{result.LowStockThreshold.ToString("0")} {_context.Ingredients.Where(i => i.Id == result.IngredientId).Select(i => i.Unit).FirstOrDefault()}",

                branchName = result.branchName,
                Supplier = result.Supplier,
                AvgPrice = $"{result.AvgPrice.ToString("N0")} đ/{_context.Ingredients.Where(i => i.Id == result.IngredientId).Select(i => i.Unit).FirstOrDefault()}",
                TotalPrice = $"{(result.Quantity * result.AvgPrice).ToString("N0")} đ",

                BatchDetails = result.BatchDetails
            };
        }

        public async Task<string> GetInventoryReceicer(Guid createdBy)
        {
            return await _context.Users.Where(u => u.Id == createdBy).Select(u => u.FullName).FirstOrDefaultAsync();
        }

        public Task<InventoryStatisticsDto> GetInventoryStatistics()
        {
            var TotalIngredients = _context.Ingredients.Count();
            var TotalWarnings = _context.LowStockAlerts.Count();
            var TotalInventories = _context.Inventories.Count();

            return Task.FromResult(new InventoryStatisticsDto
            {
                TotalIngredients = TotalIngredients,
                TotalWarnings = TotalWarnings,
                TotalInventories = TotalInventories
            });
        }

        public async Task<List<LowStockAlert>> GetLowStockIngredients()
        {

            var result = await _context.LowStockAlerts
                        .Include(i => i.Ingredient)
                        .Include(i => i.Inventory)
                            .ThenInclude(inv => inv.Branch)
                        .Include(i => i.Inventory)
                        .Include(i => i.InventoryItem)
                        .OrderByDescending(i => i.CreatedAt)
                        .ToListAsync();
            return result;
        }

        public async Task<bool> RestockInventory(RestockInventoryDto restockInventoryDto, Guid userId)
        {
            // kiểm tra xem ingredient đã tồn tại chưa
            var ingredientInfo = await _context.Ingredients.FirstOrDefaultAsync(i => i.Id == restockInventoryDto.IngredientId && i.Name.ToLower() == restockInventoryDto.IngredientName.ToLower());

            if (ingredientInfo == null && restockInventoryDto.IngredientId != Guid.Empty)
            {
                _logger.LogError("Ingredient not found with the provided IngredientId.");
                return false;
            }
            else if (ingredientInfo != null && restockInventoryDto.IngredientId == Guid.Empty)
            {
                _logger.LogError("IngredientId is empty but Ingredient already exists.");
                return false;
            }
            else if (ingredientInfo == null)
            {
                restockInventoryDto.IngredientId = Guid.NewGuid();
                ingredientInfo = new Ingredient
                {
                    Id = restockInventoryDto.IngredientId,
                    Name = restockInventoryDto.IngredientName,
                    Unit = restockInventoryDto.IngredientUnit,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = AppConstants.STAFF_ROLE_GUID,
                    CreatedBy = userId,
                    IsDeleted = false
                };
                await _context.Ingredients.AddAsync(ingredientInfo);
            }

            var inventoryId = await _context.Inventories
                                .Where(i => i.BranchId == restockInventoryDto.BranchId)
                                .Select(i => i.Id)
                                .FirstOrDefaultAsync();
            if (inventoryId == null)
            {
                _logger.LogError("Inventory not found for the provided BranchId.");
                return false;
            }

            decimal totalCost = restockInventoryDto.QuantityReceived * restockInventoryDto.UnitPrice;
            var receipt = new InventoryReceipt
            {
                InventoryId = inventoryId,
                IngredientId = restockInventoryDto.IngredientId,
                QuantityReceived = restockInventoryDto.QuantityReceived,
                UnitPrice = restockInventoryDto.UnitPrice,
                TotalCost = totalCost,
                ExpiryDate = restockInventoryDto.ExpiryDate,
                Supplier = restockInventoryDto.SupplierName,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                UpdatedBy = AppConstants.STAFF_ROLE_GUID,
                CreatedBy = userId,
                IsDeleted = false
            };

            try
            {
                _context.InventoryReceipts.Add(receipt);
                await _context.SaveChangesAsync();

                var inventoryItem = new InventoryItem
                {
                    InventoryId = inventoryId,
                    IngredientId = restockInventoryDto.IngredientId,
                    Quantity = restockInventoryDto.QuantityReceived,
                    LowStockThreshold = restockInventoryDto.LowStockThreshold,
                    ExpiryDate = restockInventoryDto.ExpiryDate,
                    LastRestocked = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = AppConstants.STAFF_ROLE_GUID,
                    CreatedBy = userId,
                    IsDeleted = false,
                    InventoryReceiptId = receipt.Id
                };

                await _context.InventoryItem.AddAsync(inventoryItem);
                return await Save();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while restocking inventory: {ex.Message}");
                if (ex.InnerException != null)
                {
                    _logger.LogError($"Inner exception: {ex.InnerException.Message}");
                }
                return false;
            }
        }

        public async Task<bool> Save()
        {
            try
            {
                _logger.LogInformation("Entered InventoryRepository");
                var saved = await _context.SaveChangesAsync();
                _logger.LogInformation("Leaving InventoryRepository");
                return saved > 0;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Lỗi khi lưu vào database: {ex.Message}");
                if (ex.InnerException != null)
                {
                    _logger.LogError($"Chi tiết lỗi: {ex.InnerException.Message}");
                }
                return false;
            }
        }
    }
}
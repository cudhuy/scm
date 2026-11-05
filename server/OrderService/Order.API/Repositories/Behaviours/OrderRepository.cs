using Domain.Constanst;
using Domain.Context;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Order.API.Repositories.Interfaces;

namespace Order.API.Repositories.Behaviours
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ILogger<OrderRepository> _logger;
        private readonly SCMDbContext _context;

        public OrderRepository(ILogger<OrderRepository> logger, SCMDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        // user 
        public async Task<List<Domain.Entities.Order>> GetOrdersByCustomer(Guid customerId, string? orderCode, int? status, string? type)
        {
            _logger.LogInformation("Entered method GetOrdersByCustomer in OrderRepository");

            try
            {
                var query = _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.OrderDetails)
                    .Where(o => o.CustomerId == customerId && !o.IsDeleted)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(orderCode))
                    query = query.Where(o => o.OrderCode.Contains(orderCode));

                if (status.HasValue)
                    query = query.Where(o => o.Status == status.Value);
                if (!string.IsNullOrEmpty(type))
                    query = query.Where(o => o.OrderType == type);

                var result = await query
           .OrderByDescending(o => o.CreatedAt)
           .ToListAsync();
                _logger.LogInformation("Successfully retrieved {Count} orders for customer {CustomerId}", result.Count, customerId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching orders for customer {CustomerId}", customerId);
                return new List<Domain.Entities.Order>();
            }
        }

        public async Task<bool> ExistCustomerById(Guid userId)
        {
            return await _context.Users.Where(d => !d.IsDeleted).AnyAsync(u => u.Id == userId);
        }


        public async Task<OrderCancellation?> GetOrderCancellationByOrderId(Guid orderId)
        {
            return await _context.OrderCancellations.Include(c => c.CancelReason)
                .FirstOrDefaultAsync(oc => oc.OrderId == orderId);
        }
      
        public async Task<Domain.Entities.Order> GetOrderDetailById(Guid orderId)
        {
            _logger.LogInformation("Entered method GetOrderDetailById in OrderRepository");

            try
            {
                return await _context.Orders
                    .Where(d => !d.IsDeleted)
                    .Include(o => o.User)
                    .Include(b => b.Branch).ThenInclude(t => t.BranchTables)
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                    .FirstOrDefaultAsync(o => o.Id == orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOrderByIdAsync");
                throw;
            }
        }



        public async Task<List<string>> GetListOrderCodeByCustomerId(Guid customerId)
        {
            _logger.LogInformation("Entered method GetListOrderCodeByCustomerId in OrderRepository");

            try
            {
                return await _context.Orders
                                     .Where(o => o.CustomerId == customerId && !o.IsDeleted)
                                     .Select(o => o.OrderCode)
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetListOrderCodeByCustomerId");
                throw;
            }
        }

        public async Task<List<CancelReason>> GetListCancelReason()
        {
            _logger.LogInformation("Entered method GetListCancelReason in OrderRepository");

            try
            {
                return await _context.CancelReasons
                                     .Where(o => !o.IsDeleted)
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetListCancelReason");
                throw;
            }
        }

        public async Task<Domain.Entities.Order> GetOrderById(Guid orderId)
        {
            return await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && !o.IsDeleted);
        }

        public async Task UpdateStatusOrder(Domain.Entities.Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task AddOrderCancellation(OrderCancellation orderCancellation)
        {
            try
            {
                _context.OrderCancellations.Add(orderCancellation);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error: {ex.InnerException?.Message}");
                throw;
            }
        }

        public async Task<BranchTable?> GetBranchTableById(Guid tableId)
        {
            return await _context.BranchTables.FirstOrDefaultAsync(t => t.Id == tableId);
        }

        public async Task<Domain.Entities.Order> CreateOrderOnlAsync(Domain.Entities.Order order)
        {
            _logger.LogInformation("Entered method CreateOrderOnlAsync in OrderRepository");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.Orders.AddAsync(order);

                if (order.OrderDetails != null && order.OrderDetails.Any())
                {
                    await _context.OrderDetails.AddRangeAsync(order.OrderDetails);
                }

                foreach (var orderDetail in order.OrderDetails)
                {
                    _logger.LogInformation("Checking product with ID {ProductId} in order detail...", orderDetail.ProductId);

                    var product = await _context.Products.FindAsync(orderDetail.ProductId);
                    if (product == null)
                    {
                        _logger.LogError("Product with ID {ProductId} not found.", orderDetail.ProductId);
                        throw new KeyNotFoundException($"Product with ID {orderDetail.ProductId} not found.");
                    }
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                _logger.LogInformation("Successfully created order online  with ID {OrderId}", order.Id);
                return order;
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Product not found in order creation.");
                await transaction.RollbackAsync();
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update failed while creating order.");
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Failed to save order to the database. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating order online with ID {OrderId}", order.Id);
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> IsTableNumberExistInBranch(Guid branchId, string tableNumber)
        {
            return await _context.BranchTables
                .AnyAsync(bt => !bt.IsDeleted && bt.BranchId == branchId && bt.TableNumber == tableNumber);
        }
        public async Task<bool> IsIdTableIdExistInBranch(Guid branchId, Guid tableId)
        {
            return await _context.BranchTables
                .AnyAsync(t => t.Id == tableId && t.BranchId == branchId && !t.IsDeleted);
        }

        public async Task<List<Guid>> GetListIdBranch()
        {
            _logger.LogInformation("Entered method GetListIdBranch in OrderRepository");

            try
            {
                var ids = await _context.Branches
                    .Where(v => !v.IsDeleted)
                     .Select(v => v.Id)
                    .ToListAsync();

                _logger.LogInformation("Successfully retrieved {Count} ids of Branches .", ids.Count);
                return ids;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving id Branches.");
                throw;
            }
        }

        public async Task<VoucherUsage?> GetVoucherUsageByUserAndVoucherAsync(Guid userId, Guid voucherId)
        {
            return await _context.VoucherUsages
                .Include(vu => vu.Voucher)
                .FirstOrDefaultAsync(vu => vu.UserId == userId && vu.VoucherId == voucherId && !vu.IsDeleted);
        }

        public async Task<CancelReason?> GetCancelReasonById(Guid cancelReasonId)
        {
            return await _context.CancelReasons
                .FirstOrDefaultAsync(cr => cr.Id == cancelReasonId && !cr.IsDeleted);
        }



        // manager 
        public async Task<List<Domain.Entities.Order>> GetActiveOrdersByTableId(Guid tableId)
        {
            return await _context.Orders
                .Where(o => o.TableId == tableId &&
                            o.Status != (int)OrderEnum.EStatus.Completed &&
                            o.Status != (int)OrderEnum.EStatus.Canceled &&
                            o.Status != (int)OrderEnum.EStatus.Delivering &&
                            o.OrderType == OrderEnum.EOrderType.Offline.ToString()&&
                            !o.IsDeleted)
                .ToListAsync();
        }


        public async Task<BranchTable> GetTableByIdAsync(Guid tableId)
        {
            return await _context.BranchTables
                .FirstOrDefaultAsync(i => i.Id == tableId && !i.IsDeleted);
        }

        public async Task UpdateIsOccupiedAsync(BranchTable table)
        {
            _context.BranchTables.Update(table);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Domain.Entities.Order>> GetOrders(string? orderCode, int? status)
        {
            _logger.LogInformation("Entered method GetOrdersAsync in OrderRepository");

            try
            {
                var query = _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.OrderDetails)
                    .Where(o => !o.IsDeleted)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(orderCode))
                    query = query.Where(o => o.OrderCode.Contains(orderCode));

                if (status.HasValue)
                    query = query.Where(o => o.Status == status.Value);

                var result = await query.ToListAsync();
                _logger.LogInformation("Successfully retrieved {Count} orders", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching orders");
                return new List<Domain.Entities.Order>();
            }
        }
        public async Task<string> GetTableNameByTableId(Guid branchId, Guid tableId)
        {
            var tableName = await _context.BranchTables
                .Where(b => b.BranchId == branchId && b.Id == tableId && !b.IsDeleted && !b.Branch.IsDeleted)
                .Select(b => b.TableNumber) 
                .FirstOrDefaultAsync();    

            return tableName; 
        }
        public async Task<List<Domain.Entities.Order>> GetOrderOffs(string? orderCode, int? status, Guid? branchID ,string? tableNumber)
        {
            _logger.LogInformation("Entered method GetOrderOnls in OrderRepository");

            try
            {
                var query = _context.Orders.AsNoTracking()
                    .Include(o => o.User)
                    .Include(o => o.OrderDetails)
                    .Where(o => !o.IsDeleted && o.OrderType == OrderEnum.EOrderType.Offline.ToString())
                    .Include(o => o.Branch).ThenInclude( b => b.BranchTables )
                    .OrderByDescending(o => o.CreatedAt)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(orderCode))
                    query = query.Where(o => o.OrderCode.Contains(orderCode));

                if (status.HasValue)
                    query = query.Where(o => o.Status == status.Value);
                query = query.Where(o => o.Status == 2 || o.Status == 3);
                if (branchID.HasValue)
                    query = query.Where(o => o.BranchId == branchID.Value);
                if (!string.IsNullOrEmpty(tableNumber))
                {
                    query = query.Where(o =>
                        o.Table != null &&  
                        o.Table.TableNumber == tableNumber && 
                        o.Table.BranchId == branchID); 
                }


                var result = await query.OrderByDescending(o => o.CreatedAt).ToListAsync();
                _logger.LogInformation("Successfully retrieved {Count} orders", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching orders");
                return new List<Domain.Entities.Order>();
            }
        }
        public async Task<List<Domain.Entities.Order>> GetOrderOnls(string? orderCode, int? status, Guid? branchID)
        {
            _logger.LogInformation("Entered method GetOrderOnls in OrderRepository");

            try
            {
                var query = _context.Orders.AsNoTracking()
                    .Include(o => o.User)
                    .Include(o => o.OrderDetails)
                    .Where(o => !o.IsDeleted && o.OrderType == OrderEnum.EOrderType.Online.ToString() && o.User.RoleId == AppConstants.USER_ROLE_GUID)
                    .Include(o => o.Branch)
                    .OrderByDescending(o => o.CreatedAt)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(orderCode))
                    query = query.Where(o => o.OrderCode.Contains(orderCode));

                if (status.HasValue)
                    query = query.Where(o => o.Status == status.Value);

                if (branchID.HasValue)
                    query = query.Where(o => o.BranchId == branchID.Value);

                var result = await query.OrderByDescending(o => o.CreatedAt).ToListAsync();
                _logger.LogInformation("Successfully retrieved {Count} orders", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching orders");
                return new List<Domain.Entities.Order>();
            }
        }

        public async Task<Domain.Entities.Order> GetOrderByIdForManager(Guid orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ThenInclude(p => p.ProductIngredients)
                .FirstOrDefaultAsync(o => o.Id == orderId && !o.IsDeleted);
        }

        public async Task<Voucher?> GetVoucherByIdAsync(Guid voucherId)
        {
            return await _context.Vouchers
                .Where(v => v.Id == voucherId && !v.IsDeleted)
                .FirstOrDefaultAsync();
        }
        public async Task<User?> GetCustomerByIdAsync(Guid customerId)
        {
            return await _context.Users
                           .Where(v => v.Id == customerId && !v.IsDeleted)
                           .FirstOrDefaultAsync();
        }

        public async Task UpdateVoucherAsync(Voucher voucher)
        {
            _context.Vouchers.Update(voucher);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateVoucherUsage(VoucherUsage voucherUsage)
        {
            _context.VoucherUsages.Update(voucherUsage);
            await _context.SaveChangesAsync();
        }
        public async Task AddVoucherUsageAsync(VoucherUsage voucherUsage)
        {
            if (voucherUsage == null)
            {
                throw new ArgumentNullException(nameof(voucherUsage), "VoucherUsage cannot be null.");
            }

            await _context.VoucherUsages.AddAsync(voucherUsage);
            await _context.SaveChangesAsync();

            _logger.LogInformation("VoucherUsage added: VoucherId={VoucherId}, UserId={UserId}, UsedAt={UsedAt}",
                voucherUsage.VoucherId, voucherUsage.UserId, voucherUsage.UsedAt);
        }


        public async Task<Domain.Entities.Order?> GetOrderByIdAsync(Guid orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .Include(o => o.User)
                .Include(o => o.Branch)
                .FirstOrDefaultAsync(o => o.Id == orderId && !o.IsDeleted);
        }
        public async Task<List<BranchTable>> GetTablesAsync(Guid? branchId, bool? isOccupied, string? tableNumber)
        {
            var query = _context.BranchTables.AsQueryable();
            query = query.Where(t => !t.IsDeleted);


            if (branchId.HasValue)
            {
                query = query.Where(t => t.BranchId == branchId.Value);
            }

            if (isOccupied.HasValue)
            {
                query = query.Where(t => t.IsOccupied == isOccupied.Value);
            }

            if (!string.IsNullOrEmpty(tableNumber))
            {
                query = query.Where(t => t.TableNumber.Contains(tableNumber));
            }

            var tables = await query.Include(t => t.Branch)
                                    .Include(t => t.Orders)
                                    .ToListAsync();

            return tables;
        }
        public async Task<BranchTable> GetBranchTableWithOrdersAsync(Guid tableId)
        {
            return await _context.BranchTables
                .Include(t => t.Orders)
                    .ThenInclude(o => o.OrderDetails)
                        .ThenInclude(od => od.Product) 
                .Where(t => t.Id == tableId && !t.IsDeleted)
                .FirstOrDefaultAsync();
        }
        public async Task UpdateBranchTableAsync(BranchTable table)
        {
            _context.BranchTables.Update(table);
            await _context.SaveChangesAsync();
        }
        //for  accept order
        public async Task<List<InventoryItem>> GetInventoryBatchesByIngredientAndBranch(Guid ingredientId, Guid branchId)
        {
            return await _context.InventoryItem
                .Where(ii => ii.IngredientId == ingredientId && ii.Inventory.BranchId == branchId && ii.Quantity > 0 && !ii.IsDeleted && ii.ExpiryDate > DateTime.Now)
                .OrderBy(ii => ii.CreatedAt)
                .ToListAsync();
        }
        public async Task<Product> GetProductById(Guid productId)
        {
            return await _context.Products
                .Include(p => p.ProductIngredients)
                .ThenInclude(pi => pi.Ingredient)
                .FirstOrDefaultAsync(p => p.Id == productId && !p.IsDeleted);
        }

        public async Task<InventoryItem> GetInventoryItemByIngredientAndBranch(Guid ingredientId, Guid branchId)
        {
            return await _context.InventoryItem
                .FirstOrDefaultAsync(ii => ii.IngredientId == ingredientId && ii.Inventory.BranchId == branchId && !ii.IsDeleted);
        }


        public async Task UpdateInventoryItem(InventoryItem inventoryItem)
        {
            _context.InventoryItem.Update(inventoryItem);
            await _context.SaveChangesAsync();
        }
        public async Task<Ingredient> GetIngredientById(Guid ingredientId)
        {
            return await _context.Ingredients.FirstOrDefaultAsync(i => i.Id == ingredientId && !i.IsDeleted);
        }

        public async Task<int> GetTodayOrderCount()
        {
            _logger.LogInformation("Entered method GetTodayOrderCount");
            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);

            int count = await _context.Orders
                .Where(o => o.CreatedAt >= today && o.CreatedAt < tomorrow && !o.IsDeleted)
                .CountAsync();
            _logger.LogInformation("Leaving method GetTodayOrderCount");
            return count + 1;
        }

        public async Task<List<Domain.Entities.Order>> GetOrderOff(string? orderCode, int? status, Guid? branchID, Guid? tableId)
        {
            _logger.LogInformation("Entered method GetOrderOffs in OrderRepository");

            try
            {
                var query = _context.Orders.AsNoTracking()
                    .Include(o => o.User)
                    .Include(o => o.OrderDetails)
                    .Include(o => o.Branch).ThenInclude(b => b.BranchTables)
                    .Include(o => o.Table)
                    .Where(o => !o.IsDeleted && o.OrderType == OrderEnum.EOrderType.Offline.ToString())
                    .Where(o => o.Status == 2 || o.Status == 3)
                    .OrderByDescending(o => o.CreatedAt)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(orderCode))
                    query = query.Where(o => o.OrderCode.Contains(orderCode));

                if (status.HasValue)
                    query = query.Where(o => o.Status == status.Value);

                if (branchID.HasValue)
                    query = query.Where(o => o.BranchId == branchID.Value);

                if (tableId.HasValue)
                    query = query.Where(o => o.TableId != null && o.TableId == tableId.Value);

                var result = await query.ToListAsync();
                _logger.LogInformation("Successfully retrieved {Count} orders", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching orders");
                return new List<Domain.Entities.Order>();
            }
        }


        public async Task<Domain.Entities.Order> GetOrderByCode(string content)
            => await _context.Orders.FirstOrDefaultAsync(item => item.OrderCode == content);
    }
}

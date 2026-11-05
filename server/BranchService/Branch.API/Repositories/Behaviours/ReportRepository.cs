using Branch.API.Dtos.Report;
using Branch.API.Services.Interfaces;
using Domain.Constanst;
using Domain.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Branch.API.Services.Behaviours
{
    public class ReportRepository : IReportRepository
    {
        private readonly SCMDbContext _context;
        private readonly ILogger<ReportRepository> _logger;

        public ReportRepository(SCMDbContext context, ILogger<ReportRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<MonthlyRevenueDto>> GetRevenueDataAsync(Guid? categoryId, int year, Guid? branchId)
        {
            try
            {
                var baseData = await (
                    from order in _context.Orders
                    join orderDetail in _context.OrderDetails on order.Id equals orderDetail.OrderId
                    join product in _context.Products on orderDetail.ProductId equals product.Id
                    join category in _context.Categories on product.CategoryId equals category.Id
                    where order.CreatedAt.Year == year
                          && (!categoryId.HasValue || product.CategoryId == categoryId.Value)
                          && (branchId == null || order.BranchId == branchId)
                          && order.Status == 2
                    select new
                    {
                        OrderMonth = order.CreatedAt.Month,
                        Revenue = orderDetail.Quantity * orderDetail.Price,
                        CategoryName = category.Name,
                        BranchId = branchId == null ? Guid.Empty : order.BranchId,
                        BranchName = branchId == null ? "All Branch" : order.Branch.Name
                    }).ToListAsync();

                var groupedData = baseData
                    .GroupBy(x => new { x.OrderMonth, x.CategoryName, x.BranchId, x.BranchName })
                    .Select(g => new MonthlyRevenueDto
                    {
                        Month = g.Key.OrderMonth.ToString(),
                        Revenue = g.Sum(x => x.Revenue),
                        CategoryName = g.Key.CategoryName,
                        BranchName = g.Key.BranchName
                    })
                    .ToList();

                // Tạo dữ liệu đầy đủ cho cả năm
                var fullYearData = Enumerable.Range(1, 12)
                    .Select(m =>
                    {
                        // Tìm tất cả dữ liệu cho tháng này
                        var monthData = groupedData
                            .Where(x => int.TryParse(x.Month, out int month) && month == m)
                            .ToList();

                        return new MonthlyRevenueDto
                        {
                            Month = new DateTime(year, m, 1).ToString("MMMM"),
                            Revenue = monthData.Sum(x => x.Revenue), // Tính tổng tất cả doanh thu trong tháng
                            CategoryName = categoryId != null ?
                                (monthData.FirstOrDefault()?.CategoryName ?? "No Data") :
                                "All Categories",
                            BranchName = branchId != null ?
                                (monthData.FirstOrDefault()?.BranchName ?? "Unknown Branch") :
                                 "All Branch"
                        };
                    }).ToList();

                return fullYearData;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetRevenueDataAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Order>> GetOrderStatisticsInWeek(DateTime startOfWeek, DateTime endOfWeek, Guid? branchId)
        {
            _logger.LogInformation("Entered method GetOrderStatisticsInWeek ReportRepository");

            // Đảm bảo endOfWeek lấy hết ngày cuối cùng
            endOfWeek = endOfWeek.Date.AddDays(1).AddSeconds(-1);

            IQueryable<Order> query = _context.Orders.AsNoTracking()
                .Where(x => x.CreatedAt >= startOfWeek
                            && x.CreatedAt <= endOfWeek
                            && !x.IsDeleted);

            if (branchId != null)
            {
                _logger.LogInformation("Filtering by branchId: {BranchId}", branchId);
                query = query.Where(x => x.BranchId == branchId);
            }
            else
            {
                _logger.LogInformation("Fetching all branches");
            }

            return await query.ToListAsync();
        }
        public async Task<int> GetTotalCustomers()
        {
            _logger.LogInformation("Entered method GetTotalCustomers in ReportRepository");

            try
            {
                Guid userRoleId = AppConstants.USER_ROLE_GUID;

                int totalCustomers = await _context.Users
                    .Where(u => u.RoleId == userRoleId && !u.IsDeleted)
                    .CountAsync();

                _logger.LogInformation("Successfully retrieved total customers: {Total}", totalCustomers);
                return totalCustomers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while counting customers.");
                throw;
            }
        }

        public async Task<decimal> GetTotalAmountForStatus2Orders(Guid? branchId)
        {
            _logger.LogInformation("Entered method GetTotalAmountForStatus2Orders in OrderRepository");

            try
            {
                Guid userRoleId = AppConstants.USER_ROLE_GUID;
                var query = _context.Orders.Where(o => o.Status == 2 && !o.IsDeleted && o.User.RoleId == userRoleId);

                if (branchId.HasValue)
                {
                    query = query.Where(o => o.BranchId == branchId.Value);
                }

                var totalAmount = await query.SumAsync(o => o.TotalAmount);

                _logger.LogInformation("Successfully computed total amount for orders with status 2: {TotalAmount}", totalAmount);
                return totalAmount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while computing total amount for orders with status 2.");
                throw;
            }
        }



        public async Task<int> GetTotalOrdersForStatus2(Guid? branchId)
        {
            _logger.LogInformation("Entered method GetTotalOrdersForStatus2 in OrderRepository");

            try
            {
                var query = _context.Orders
                .Where(o => o.Status == 2 && !o.IsDeleted && o.User.RoleId == AppConstants.USER_ROLE_GUID);
                if (branchId.HasValue)
                {
                    query = query.Where(o => o.BranchId == branchId.Value);
                }

                int totalOrders = await query.CountAsync();

                _logger.LogInformation("Successfully retrieved total orders with status 2: {TotalOrders}", totalOrders);
                return totalOrders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while counting orders with status 2.");
                throw;
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

        public async Task<int> GetTotalProductsCount()
        {
            _logger.LogInformation("Entered method GetTotalProductsCount in ProductRepository");

            try
            {
                return await _context.Products
                    .Where(p => !p.IsDeleted)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetTotalProductsCount");
                throw;
            }
        }




        public async Task<Domain.Entities.Order?> GetOrderDetailById(Guid orderId)
        {
            _logger.LogInformation("Entered method GetOrderDetailById in OrderRepository");

            try
            {
                return await _context.Orders
                    .Where(o => !o.IsDeleted)
                    .Include(o => o.User)
                    .Include(o => o.Branch)
                        .ThenInclude(b => b.BranchTables)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Product)
                    .FirstOrDefaultAsync(o => o.Id == orderId && !o.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOrderDetailById");
                throw;
            }
        }
        public async Task<OrderCancellation?> GetOrderCancellationByOrderId(Guid orderId)
        {
            try
            {
                return await _context.OrderCancellations
                    .Include(oc => oc.CancelReason)
                    .FirstOrDefaultAsync(oc => oc.OrderId == orderId && !oc.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOrderCancellationByOrderId");
                throw;
            }
        }

        public async Task<IEnumerable<Product>> GetTopSellingProducts(Guid? branchId)
        {
            _logger.LogInformation("Entered method GetTopSellingProducts in ProductRepository");

            try
            {
                var query = _context.OrderDetails
                    .Where(od => !od.Order.IsDeleted && od.Order.User.RoleId == AppConstants.USER_ROLE_GUID);
                if (branchId.HasValue)
                {
                    query = query.Where(od => od.Order.BranchId == branchId.Value);
                }

                var topProducts = await query
                    .GroupBy(od => od.ProductId)
                    .Select(g => new
                    {
                        ProductId = g.Key,
                        TotalQuantity = g.Sum(od => od.Quantity)
                    })
                    .OrderByDescending(g => g.TotalQuantity)
                    .Take(5)
                    .Join(_context.Products
                            .Include(p => p.Category)
                            .Where(p => !p.IsDeleted),
                        od => od.ProductId,
                        p => p.Id,
                        (od, p) => p)
                    .ToListAsync();

                return topProducts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the top-selling products.");
                throw new Exception("An error occurred while fetching the top-selling products.", ex);
            }
        }



        public async Task<IEnumerable<Order>> GetRecentCustomers(Guid? branchId)
        {
            _logger.LogInformation("Entered method GetRecentCustomers in ReportRepository");

            try
            {
                Guid userRoleId = AppConstants.USER_ROLE_GUID;

                var query = _context.Orders
                    .Include(o => o.User)
                    .Where(o => !o.IsDeleted && o.CustomerId != null && o.User.RoleId == userRoleId && !o.User.IsDeleted);

                if (branchId.HasValue)
                {
                    query = query.Where(o => o.BranchId == branchId.Value);
                }

                return await query
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching recent customers.");
                throw;
            }
        }

        public async Task<List<Domain.Entities.Order>> GetLatestFiveOrdersAsync(Guid? branchId)
        {
            _logger.LogInformation("Fetching the latest 5 orders from OrderRepository. BranchId: {BranchId}", branchId);

            try
            {
                IQueryable<Domain.Entities.Order> query = _context.Orders
           .Where(o => !o.IsDeleted && o.User.RoleId == AppConstants.USER_ROLE_GUID)
           .Include(o => o.User);

                if (branchId.HasValue)
                {
                    query = query.Where(o => o.BranchId == branchId.Value);
                }

                var orders = await query
                    .OrderByDescending(o => o.CreatedAt)
                    .Take(5)
                    .ToListAsync();

                _logger.LogInformation("Successfully retrieved {Count} latest orders.", orders.Count);
                return orders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving latest orders for BranchId: {BranchId}", branchId);
                throw;
            }
        }

        public async Task<Domain.Entities.User?> GetUserByIdAsync(Guid id)
        {
            return await _context.Users
                .Where(u => u.Id == id && !u.IsDeleted && u.RoleId == AppConstants.USER_ROLE_GUID)
                .Include(u => u.Ranks)
                .FirstOrDefaultAsync();
        }
    }
}


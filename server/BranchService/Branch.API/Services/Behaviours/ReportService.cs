using AutoMapper;
using Branch.API.Dtos.Order;
using Branch.API.Dtos.Report;
using Branch.API.Services.Interfaces;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Branch.API.Services.Behaviours
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ReportService> _logger;

        public ReportService(IReportRepository reportRepository, IMapper mapper, ILogger<ReportService> logger)
        {
            _reportRepository = reportRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<RevenueReportDto> GetRevenueReportAsync(Guid? branchId, Guid? categoryId, int year)
        {
            try
            {
                // Truyền branchId vào repository
                List<MonthlyRevenueDto> reportData = await _reportRepository.GetRevenueDataAsync(categoryId, year, branchId);

                // Xử lý tổng doanh thu
                var totalRevenue = reportData.Sum(r => r.Revenue);

                // Xác định tháng có doanh thu cao nhất
                var leadingMonth = reportData.OrderByDescending(r => r.Revenue).FirstOrDefault()?.Month ?? "Unknown";

                return new RevenueReportDto
                {
                    Total = totalRevenue,
                    MonthlyRevenue = reportData.ToList(),
                    LeadingMonth = leadingMonth,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetRevenueReportAsync: {ex.Message}");
                throw;
            }
        }
        public async Task<ProductDetailDto?> GetDetailProductsAsync(Guid id)
        {
            var product = await _reportRepository.GetProductByIdAsync(id);
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
        public async Task<OrderStatisticsResponseDto> GetOrderStatistics(Guid? branchId, int week, int month, int year)
        {
            try
            {
                _logger.LogInformation("Entered method GetOrderStatistics ReportService");
                var startOfMonth = new DateTime(year, month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                DateTime startOfWeek, endOfWeek;
                if (week == 1)
                {
                    startOfWeek = startOfMonth;
                    endOfWeek = startOfMonth.AddDays(6);
                }
                else
                {
                    startOfWeek = startOfMonth.AddDays((week - 1) * 7);
                    endOfWeek = (week == 4) ? endOfMonth : startOfWeek.AddDays(6);
                }

                var ordersInWeek = await _reportRepository.GetOrderStatisticsInWeek(startOfWeek, endOfWeek, branchId);
                var ordersInWeelGrouped = ordersInWeek
                    .GroupBy(x => x.CreatedAt.DayOfWeek)
                    .Select(g => new OrderData { Day = g.Key.ToString(), Count = g.Count() })
                    .ToList();
                var result = new OrderStatisticsResponseDto
                {
                    BranchId = branchId,
                    Week = week,
                    Month = month,
                    Year = year,
                    OrderStatisticsData = ordersInWeelGrouped,
                    HighestOrderInWeek = ordersInWeelGrouped.Any() ? ordersInWeelGrouped.Max(x => x.Count) : 0
                };
                _logger.LogInformation("Leaving method GetOrderStatistics ReportService");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while GetOrderStatistics in ReportService: {ex}");
                return null;
            }
        }

        public async Task<int> GetTotalCustomers()
        {
            return await _reportRepository.GetTotalCustomers();
        }

        public async Task<decimal> GetTotalAmountForStatus2Orders(Guid? branchId = null)
        {
            return await _reportRepository.GetTotalAmountForStatus2Orders(branchId);
        }

        public async Task<int> GetTotalOrdersForStatus2(Guid? branchId = null)
        {
            return await _reportRepository.GetTotalOrdersForStatus2(branchId);
        }

        public async Task<int> GetTotalProductsCount()
        {
            _logger.LogInformation("Entered method GetTotalProductsCount in ProductService");

            try
            {
                return await _reportRepository.GetTotalProductsCount();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetTotalProductsCount in ProductService");
                throw;
            }
        }

        public async Task<IEnumerable<OrderDto>> GetLatestFiveOrders(Guid? branchId)
        {
            _logger.LogInformation("Fetching latest five orders in OrderService");

            var orders = await _reportRepository.GetLatestFiveOrdersAsync(branchId);

            var orderDtos = orders.Select(o => new OrderDto
            {
                OrderId = o.Id,
                CustomerName = o.User != null ? o.User.FullName : "Unknown",
                Type = o.OrderType,
                Status = o.Status,
                CreatedAt = o.CreatedAt,
                TotalAmount = o.TotalAmount
            });

            return orderDtos;
        }

        public async Task<OrderDetailDto> GetOrderByIdAsync(Guid orderId)
        {
            _logger.LogInformation("Entered method GetOrderByIdAsync in OrderService");

            try
            {
                var order = await _reportRepository.GetOrderDetailById(orderId);
                if (order == null)
                {
                    _logger.LogWarning("Order with id {OrderId} not found", orderId);
                    throw new KeyNotFoundException("Order not found");
                }

                var cancellation = await _reportRepository.GetOrderCancellationByOrderId(order.Id);

                return new OrderDetailDto
                {
                    OrderId = order.Id,
                    OrderCode = order.OrderCode,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                    OrderType = order.OrderType,
                    CreatedAt = order.CreatedAt,
                    CustomerName = order.User?.FullName,
                    CustomerEmail = order.User?.Email,
                    RecipientName = order.RecipientName,
                    RecipientPhone = order.RecipientPhone,
                    ShippingAddress = order.ShippingAddress,
                    CancellationReason = order.OrderType == OrderEnum.EOrderType.Online.ToString()
                        ? (cancellation?.CancelReasonId != null ? cancellation.CancelReason.Reason : cancellation?.CustomReason)
                        : null,
                    BranchName = order.Branch?.Name,
                    TableName = order.OrderType == OrderEnum.EOrderType.Offline.ToString()
                        ? order.Branch?.BranchTables.FirstOrDefault(bt => bt.Id == order.TableId)?.TableNumber
                        : null,
                    OrderDetails = order.OrderDetails.Select(od => new OrderDetailDto.OrderDetailItem
                    {
                        ProductName = od.Product.Name,
                        Quantity = od.Quantity,
                        Price = od.Price
                    }).ToList()
                };
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while fetching order {OrderId}", orderId);
                throw new Exception("Database error occurred. Please try again later.", ex);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Invalid operation error while fetching order {OrderId}", orderId);
                throw new Exception("An unexpected error occurred. Please contact support.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOrderByIdAsync in OrderService");
                throw;
            }
        }

        public async Task<IEnumerable<ProductBestSellingDto>> GetTopSellingProductsAsync(Guid? branchId)
        {
            _logger.LogInformation("Entered method GetTopSellingProducts in ProductService");

            try
            {
                var products = await _reportRepository.GetTopSellingProducts(branchId);

                if (!products.Any())
                {
                    _logger.LogInformation("No top-selling products found.");
                    return Enumerable.Empty<ProductBestSellingDto>();
                }

                return products.Select(p => new ProductBestSellingDto
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
                _logger.LogError($"Error in {nameof(GetTopSellingProductsAsync)}: {ex.Message}");
                throw;
            }
        }


        public async Task<IEnumerable<RecentCustomerDto>> GetRecentCustomers(Guid? branchId)
        {
            _logger.LogInformation("Entered method GetRecentCustomers in ReportService");

            try
            {
                var orders = await _reportRepository.GetRecentCustomers(branchId);
                if (orders == null || !orders.Any())
                {
                    _logger.LogInformation("No recent customers found.");
                    return new List<RecentCustomerDto>();
                }

                var recentCustomers = orders
                    .GroupBy(o => o.CustomerId)
                    .Select(g => new RecentCustomerDto
                    {
                        CustomerId = g.Key.Value,
                        CustomerName = g.First().User.FullName,
                        Email = g.First().User.Email,
                        TotalSpent = g.Sum(o => o.TotalAmount),
                        LastPurchase = g.Max(o => o.CreatedAt)
                    })
                    .OrderByDescending(c => c.LastPurchase)
                    .Take(5)
                    .ToList();

                return recentCustomers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetRecentCustomers");
                throw;
            }
        }

        public async Task<UserDetailDto?> GetDetailUserAsync(Guid id)
        {
            var user = await _reportRepository.GetUserByIdAsync(id);
            if (user == null) return null;

            return new UserDetailDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Dob = user.Dob,
                Address = user.Address,
                IsVerified = user.IsVerified,
               // RoleName = user.Role?.Name,
                RankName = user.Ranks?.Name,
                Status = user.Status,
                Avatar = user.Avatar
             
            };
        }
    }
}

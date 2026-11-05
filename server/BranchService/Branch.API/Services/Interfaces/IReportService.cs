using Branch.API.Dtos.Order;
using Branch.API.Dtos.Report;

namespace Branch.API.Services.Interfaces
{
    public interface IReportService
    {
        Task<RevenueReportDto> GetRevenueReportAsync(Guid? branchId, Guid? categoryId, int year);
        Task<OrderStatisticsResponseDto> GetOrderStatistics(Guid? branchId, int week, int month, int year);
        Task<int> GetTotalCustomers();
        Task<decimal> GetTotalAmountForStatus2Orders(Guid? branchId = null);
        Task<int> GetTotalOrdersForStatus2(Guid? branchId = null);
        Task<int> GetTotalProductsCount();
        Task<IEnumerable<OrderDto>> GetLatestFiveOrders(Guid? branchId);

        Task<OrderDetailDto> GetOrderByIdAsync(Guid orderId);
        Task<IEnumerable<ProductBestSellingDto>> GetTopSellingProductsAsync(Guid? branchId);
        Task<IEnumerable<RecentCustomerDto>> GetRecentCustomers(Guid? branchId);
        Task<ProductDetailDto?> GetDetailProductsAsync(Guid id);
        Task<UserDetailDto?> GetDetailUserAsync(Guid id);


    }
}

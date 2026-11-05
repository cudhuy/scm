using Branch.API.Dtos.Report;
using Domain.Entities;
namespace Branch.API.Services.Interfaces
{
    public interface IReportRepository
    {
        Task<List<MonthlyRevenueDto>> GetRevenueDataAsync(Guid? categoryId, int year, Guid? branchId);
        Task<List<Order>> GetOrderStatisticsInWeek(DateTime startOfWeek, DateTime endOfWeek, Guid? branchId);

        Task<int> GetTotalCustomers();
        Task<decimal> GetTotalAmountForStatus2Orders(Guid? branchId);
        Task<int> GetTotalOrdersForStatus2(Guid? branchId);
        Task<List<Domain.Entities.Order>> GetLatestFiveOrdersAsync(Guid? branchId);
        Task<int> GetTotalProductsCount();
        Task<OrderCancellation?> GetOrderCancellationByOrderId(Guid orderId);
        Task<Domain.Entities.Order?> GetOrderDetailById(Guid orderId);
        Task<IEnumerable<Product>> GetTopSellingProducts(Guid? branchId);
        Task<IEnumerable<Order>> GetRecentCustomers(Guid? branchId);
        Task<Domain.Entities.Product> GetProductByIdAsync(Guid id);
        Task<Domain.Entities.User?> GetUserByIdAsync(Guid id);


    }
}

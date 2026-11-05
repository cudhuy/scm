using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Order.API.Repositories.Interfaces
{
    public interface IOrderRepository
    { // user
        Task<List<Domain.Entities.Order>> GetOrdersByCustomer(Guid customerId, string? orderCode, int? status, string? type);
        Task<bool> ExistCustomerById(Guid userId);
        Task<OrderCancellation?> GetOrderCancellationByOrderId(Guid orderId);
        Task<Domain.Entities.Order> GetOrderDetailById(Guid orderId);
        Task<List<string>> GetListOrderCodeByCustomerId(Guid customerId);
        Task<List<CancelReason>> GetListCancelReason();
        Task<Domain.Entities.Order> GetOrderById(Guid orderId);
        Task UpdateStatusOrder(Domain.Entities.Order order);
        Task AddOrderCancellation(OrderCancellation orderCancellation);
        Task<Domain.Entities.Order> CreateOrderOnlAsync(Domain.Entities.Order order);
        Task<List<InventoryItem>> GetInventoryBatchesByIngredientAndBranch(Guid ingredientId, Guid branchId);

        Task<List<Guid>> GetListIdBranch();
        Task<BranchTable?> GetBranchTableById(Guid tableId);

        Task<Voucher?> GetVoucherByIdAsync(Guid voucherId);

        Task UpdateVoucherAsync(Voucher voucher);
        Task AddVoucherUsageAsync(VoucherUsage voucherUsage);
        Task<VoucherUsage?> GetVoucherUsageByUserAndVoucherAsync(Guid userId, Guid voucherId);
        Task<CancelReason?> GetCancelReasonById(Guid cancelReasonId);
        Task<User?> GetCustomerByIdAsync(Guid customerId);
        Task<Domain.Entities.Order?> GetOrderByIdAsync(Guid orderId);
        Task UpdateVoucherUsage(VoucherUsage voucherUsage);
        Task UpdateBranchTableAsync(BranchTable table);
        // manager
        Task<bool> IsTableNumberExistInBranch(Guid branchId, string tableNumber);
        Task<Domain.Entities.Order> GetOrderByIdForManager(Guid orderId);
        Task<List<Domain.Entities.Order>> GetOrders(string? orderCode, int? status);
        Task<Product> GetProductById(Guid productId);
        Task<InventoryItem> GetInventoryItemByIngredientAndBranch(Guid ingredientId, Guid branchId);
        Task UpdateInventoryItem(InventoryItem inventoryItem);
        Task<Ingredient> GetIngredientById(Guid ingredientId);
        Task<int> GetTodayOrderCount();
        Task<Domain.Entities.Order> GetOrderByCode(string content);
        Task<List<Domain.Entities.Order>> GetOrderOffs(string? orderCode, int? status, Guid? branchID, string? tableNumber);
        Task<List<Domain.Entities.Order>> GetOrderOnls(string? orderCode, int? status, Guid? branchID);
        Task<string> GetTableNameByTableId(Guid branchId, Guid tableId);
        Task<List<BranchTable>> GetTablesAsync(Guid? branchId, bool? isOccupied, string? tableNumber);
        Task<BranchTable> GetBranchTableWithOrdersAsync(Guid tableId);
        Task<List<Domain.Entities.Order>> GetActiveOrdersByTableId(Guid tableId);
        Task<BranchTable> GetTableByIdAsync(Guid tableId);
        Task UpdateIsOccupiedAsync(BranchTable table);
        Task<bool> IsIdTableIdExistInBranch(Guid branchId, Guid tableId);
        Task<List<Domain.Entities.Order>> GetOrderOff(string? orderCode, int? status, Guid? branchID, Guid? tableId);
    }
}

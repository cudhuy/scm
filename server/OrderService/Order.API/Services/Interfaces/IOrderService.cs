using Order.API.Dtos.CancelReason;
using Order.API.Dtos.OrderManagement;
using Order.API.Dtos.OrderOfCustomer;

namespace Order.API.Services.Interfaces
{
    public interface IOrderService
    {//customer
        Task<GetOrdersOfCustomerResponseDto> GetCustomerOrdersAsync(Guid customerId, string? orderCode, int? status, string? type, int page, int pageSize);
        Task<OrderDetailDto> GetOrderByIdAsync(Guid orderId);
        Task<List<string>> GetListOrderCodeAsync(Guid customerId);
        Task<List<CancelReasonDto>> GetListCancelReasonAsync();
        Task<bool> CancelOrderAsync(Guid orderId, Guid? cancelReasonId, string customReason);
        Task<Guid> CreateOrderOnl(CreateOrderOnlDto orderDto);
        Task<Guid> CreateOrderOff(CreateOrderOffDto orderDto);
        Task<bool> CancelOrderOffAsync(Guid orderId);
        Task<bool> UpdateCompletedOrderOnlAsync(Guid orderId);
        Task<OrderDto> GetOrderById(Guid orderId);
        Task<OrderDto> GetOrderByIdHub(Guid orderId);

        // manager 
        Task<GetOrdersResponseDto> GetOrderOnlsAsync(string? orderCode, int? status, Guid? branchID, int page, int pageSize);
        Task<List<OrderDto>> GetOrdersAsync(string? orderCode, int? status);
        Task<bool> AcceptOrderOnlAsync(Guid orderId);
        Task<bool> AcceptOrderOffAsync(Guid orderId);
        Task<GetOrdersResponseOffDto> GetOrderOffsAsync(string? orderCode, int? status, Guid? branchID, string? tableNumber, int page, int pageSize);        Task<bool> CancelOrderByManagerAsync(Guid orderId, Guid? cancelReasonId, string customReason);
        Task<bool> DeliverOrderAsync(Guid orderId);//for online order
       // Task<bool> ProcessOfflineOrderAsync(Guid orderId);// for offline order
        Task<bool> CompleteOfflineOrderAsync(Guid orderId, int paymentMethod);
        Task<List<TableDto>> GetTablesAsync(Guid? branchId, bool? isOccupied, string? tableNumber);
        Task<BranchTableDetailDto> GetBranchTableDetailAsync(Guid tableId);
        Task<GetOrdersResponseOffDto> GetOrderOff(string? orderCode, int? status, Guid? branchID, Guid? tableId, int page, int pageSize);
    }
}

using AutoMapper;
using Domain.Constanst;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Order.API.Dtos.CancelReason;
using Order.API.Dtos.OrderManagement;
using Order.API.Dtos.OrderOfCustomer;
using Order.API.Repositories.Interfaces;
using Order.API.Services.Interfaces;
using static Domain.Enums.OrderEnum;

namespace Order.API.Services.Behaviours
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IOrderRepository orderRepository, IMapper mapper, ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<GetOrdersOfCustomerResponseDto> GetCustomerOrdersAsync(
    Guid customerId, string? orderCode, int? status, string? type, int page, int pageSize)
        {
            _logger.LogInformation("Fetching orders for customer {CustomerId}", customerId);

            try
            {
                bool customerExists = await _orderRepository.ExistCustomerById(customerId);
                if (!customerExists)
                {
                    _logger.LogWarning("Customer {CustomerId} not found", customerId);
                    throw new KeyNotFoundException("Không tìm thấy khách hàng");
                }

                var orders = await _orderRepository.GetOrdersByCustomer(customerId, orderCode, status, type);
                var totalItems = orders.Count();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                var paginatedOrders = orders
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(o => new OrderOfACustomerDto
                    {
                        OrderId = o.Id,
                        OrderCode = o.OrderCode,
                        TotalAmount = o.TotalAmount,
                        Status = o.Status,
                        OrderType = o.OrderType,
                        RecipientName = o.RecipientName,
                        RecipientPhone = o.RecipientPhone,
                        ShippingAddress = o.ShippingAddress,
                        PaymentMethod = o.PaymentMethod,
                        IsPaid = o.IsPaid,
                        CreatedAt = o.CreatedAt,
                        ShippingFee = o.ShippingFee,
                        VoucherAmount = o.VoucherAmount
                    })
                    .ToList();

                return new GetOrdersOfCustomerResponseDto
                {
                    Items = paginatedOrders,
                    TotalItems = totalItems,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalPages = totalPages
                };
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while retrieving orders for customer {CustomerId}", customerId);
                throw new InvalidOperationException("Database error occurred. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching orders for customer {CustomerId}", customerId);
                throw;
            }
        }

        public async Task<OrderDetailDto> GetOrderByIdAsync(Guid orderId)
        {
            _logger.LogInformation("Entered method GetOrderByIdAsync in OrderService");

            try
            {
                var order = await _orderRepository.GetOrderDetailById(orderId);
                if (order == null)
                {
                    _logger.LogWarning("Order with id {OrderId} not found", orderId);
                    throw new KeyNotFoundException("Không tìm thấy đơn hàng");
                }
                var cancellation = await _orderRepository.GetOrderCancellationByOrderId(order.Id);
                string? cancellationReason = cancellation?.CustomReason;

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
                    ShippingFee = order.ShippingFee,
                    VoucherAmount = order.VoucherAmount,
                    PaymentMethod = order.PaymentMethod,
                    IsPaid = order.IsPaid,
                    CancellationReason =
    cancellation?.CancelReasonId == Guid.Parse("ff4e33fe-1d30-4055-b6c8-454662c32e23")
        ? cancellation?.CustomReason
        : cancellation?.CancelReason?.Reason,

                    BranchName = order.Branch.Name,
                    TableName = order.OrderType == OrderEnum.EOrderType.Offline.ToString()
    ? order.Branch.BranchTables.FirstOrDefault(bt => bt.Id == order.TableId)?.TableNumber
    : null,
                    OrderDetails = order.OrderDetails.Select(od => new OrderDetailDto.OrderDetailItem
                    {
                        ProductName = od.Product.Name,
                        Quantity = od.Quantity,
                        Price = od.Price,
                        Img = od.Product.Image,
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

        public async Task<List<string>> GetListOrderCodeAsync(Guid customerId)
        {
            _logger.LogInformation("Fetching order codes for customer {CustomerId}", customerId);

            try
            {
                bool customerExists = await _orderRepository.ExistCustomerById(customerId);
                if (!customerExists)
                {
                    _logger.LogWarning("Customer {CustomerId} not found", customerId);
                    throw new KeyNotFoundException("Không tìm thấy khách hàng");
                }

                var orderCodes = await _orderRepository.GetListOrderCodeByCustomerId(customerId);
                return orderCodes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching order codes for customer {CustomerId}", customerId);
                throw;
            }
        }

        public async Task<List<CancelReasonDto>> GetListCancelReasonAsync()
        {
            _logger.LogInformation("Fetching cancel reasons");

            try
            {
                var cancelReasons = await _orderRepository.GetListCancelReason();

                if (cancelReasons == null || !cancelReasons.Any())
                {
                    _logger.LogWarning("No cancel reasons found.");
                    return new List<CancelReasonDto>();
                }

                return _mapper.Map<List<CancelReasonDto>>(cancelReasons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching cancel reasons");
                throw;
            }
        }

        public async Task<bool> CancelOrderAsync(Guid orderId, Guid? cancelReasonId, string customReason)
        {
            _logger.LogInformation("Processing cancellation for order {OrderId}", orderId);

            var order = await _orderRepository.GetOrderById(orderId);
            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} not found", orderId);
                throw new KeyNotFoundException("Không tìm thấy đơn hàng");
            }
            if (cancelReasonId.HasValue)
            {
                var reasonExists = await _orderRepository.GetCancelReasonById(cancelReasonId.Value);
                if (reasonExists == null)
                {
                    _logger.LogWarning("Invalid CancelReasonId {CancelReasonId} for order {OrderId}", cancelReasonId, orderId);
                    throw new ArgumentException("Lý do có sẵn được cung cấp không hợp lệ.");
                }
            }

            var orderStatus = (OrderEnum.EStatus)order.Status;
            if (orderStatus == OrderEnum.EStatus.Completed || orderStatus == OrderEnum.EStatus.Processing || orderStatus == OrderEnum.EStatus.Canceled)
            {
                _logger.LogWarning("Order {OrderId} cannot be canceled because its status is {Status}", orderId, orderStatus);
                return false;
            }

            if (orderStatus == OrderEnum.EStatus.Pending)
            {
                if (order.CustomerId.HasValue && order.voucherId != Guid.Empty)
                {
                    var voucherUsage = await _orderRepository.GetVoucherUsageByUserAndVoucherAsync(order.CustomerId.Value, order.voucherId);
                    if (voucherUsage != null && voucherUsage.UsedAt.Date == order.CreatedAt.Date)
                    {
                        var voucher = voucherUsage.Voucher;
                        if (voucher != null)
                        {
                            voucherUsage.IsDeleted = true;
                            await _orderRepository.UpdateVoucherUsage(voucherUsage);
                            voucher.noOfVoucherUsed = Math.Max(0, voucher.noOfVoucherUsed - 1);
                            await _orderRepository.UpdateVoucherAsync(voucher);
                        }
                    }
                }

                var orderCancellation = new OrderCancellation
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderId,
                    CancelReasonId = cancelReasonId,
                    CustomReason = string.IsNullOrEmpty(customReason) ? string.Empty : customReason,
                    CreatedAt = DateTime.Now,
                    CreatedBy = AppConstants.SYSTEM_ROLE_GUID,
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = AppConstants.SYSTEM_ROLE_GUID,
                    CanceledBy = AppConstants.SYSTEM_ROLE_GUID,
                    IsDeleted = false
                };

                await _orderRepository.AddOrderCancellation(orderCancellation);

                order.Status = (int)OrderEnum.EStatus.Canceled;
                order.UpdatedAt = DateTime.Now;
                try
                {
                    await _orderRepository.UpdateStatusOrder(order);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while updating order status for {OrderId}", orderId);
                    throw new Exception("Failed to update order status. Please try again.");
                }

                _logger.LogInformation("Order {OrderId} successfully canceled", orderId);
                return true;
            }

            return false;
        }


        public async Task<bool> UpdateCompletedOrderOnlAsync(Guid orderId)
        {
            _logger.LogInformation("Processing UpdateCompletedOrderOnlAsync for order {OrderId}", orderId);
            if (orderId == Guid.Empty)
            {
                _logger.LogWarning("Invalid orderId: {OrderId}", orderId);
                throw new ArgumentException("Mã đơn hàng không hợp lệ.");
            }
            try
            {
                var order = await _orderRepository.GetOrderById(orderId);
                if (order == null)
                {
                    _logger.LogWarning("Order {OrderId} not found", orderId);
                    throw new KeyNotFoundException("Không tìm thấy đơn hàng");
                }
                if (order.OrderType == OrderEnum.EOrderType.Offline.ToString())
                {
                    _logger.LogWarning("Order {OrderId} is offline and cannot be completed", orderId);
                    throw new InvalidOperationException("Không thể hoàn tất đơn hàng tại cửa hàng .");
                }

                var orderStatus = (OrderEnum.EStatus)order.Status;

                switch (orderStatus)
                {
                    case OrderEnum.EStatus.Completed:
                        _logger.LogWarning("Order {OrderId} is already completed, cannot be updated", orderId);
                        throw new InvalidOperationException("Đơn hàng đã hoàn tất và không thể cập nhật.");

                    case OrderEnum.EStatus.Canceled:
                        _logger.LogWarning("Order {OrderId} has been canceled, cannot be updated", orderId);
                        throw new InvalidOperationException("Đơn hàng đã bị hủy và không thể cập nhật.");

                    case OrderEnum.EStatus.Pending:
                        _logger.LogWarning("Order {OrderId} has not been processed yet, so it cannot be completed", orderId);
                        throw new InvalidOperationException("Đơn hàng vẫn đang chờ xử lý và không thể hoàn tất.");
                    case OrderEnum.EStatus.Processing:
                        _logger.LogWarning("Order {OrderId} is still processing and cannot be completed directly", orderId);
                        throw new InvalidOperationException("Đơn hàng vẫn đang được xử lý và phải được giao trước khi hoàn tất.");
                    case OrderEnum.EStatus.Delivering:
                        order.Status = (int)OrderEnum.EStatus.Completed;
                        order.IsPaid = true;
                        order.UpdatedAt = DateTime.Now;

                        await _orderRepository.UpdateStatusOrder(order);

                        _logger.LogInformation("Order {OrderId} successfully updated to Completed", orderId);
                        return true;

                    default:
                        _logger.LogWarning("Order {OrderId} has an unknown status: {Status}", orderId, orderStatus);
                        throw new InvalidOperationException("Đơn hàng có trạng thái không xác định và không thể cập nhật.");
                }
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update error while updating order {OrderId}", orderId);
                throw;
            }
            catch (TaskCanceledException tcEx)
            {
                _logger.LogError(tcEx, "Async operation was canceled for order {OrderId}", orderId);
                throw new Exception("The request was canceled. Please try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating order status for {OrderId}", orderId);
                throw;
            }
        }


        public async Task<Guid> CreateOrderOnl(CreateOrderOnlDto orderDto)
        {
            _logger.LogInformation("Entered method CreateOrderOnl in OrderService");

            try
            {
                var customer = await _orderRepository.GetCustomerByIdAsync(orderDto.CustomerId);
                if (customer == null)
                {
                    _logger.LogError("Customer {CustomerId} does not exist.", orderDto.CustomerId);
                    throw new KeyNotFoundException("Khách hàng không tồn tại..");
                }
                var existingBranches = await _orderRepository.GetListIdBranch();
                if (!existingBranches.Contains(orderDto.BranchId))
                {
                    _logger.LogError("Invalid Branch ID: {BranchId}", orderDto.BranchId);
                    throw new ArgumentException($"ID chi nhánh '{orderDto.BranchId}' không khả dụng.");
                }
                Voucher voucher = null;
                if (orderDto.VoucherId.HasValue)
                {
                    voucher = await _orderRepository.GetVoucherByIdAsync(orderDto.VoucherId.Value);
                    if (voucher == null)
                    {
                        _logger.LogError("Voucher {VoucherId} does not exist.", orderDto.VoucherId);
                        throw new KeyNotFoundException("Phiếu mua hàng không tồn tại..");
                    }
                    var voucherUsage = await _orderRepository.GetVoucherUsageByUserAndVoucherAsync(orderDto.CustomerId, orderDto.VoucherId.Value);
                    if (voucherUsage != null)
                    {
                        _logger.LogError("Customer {CustomerId} has already used voucher {VoucherId}.", orderDto.CustomerId, orderDto.VoucherId);
                        throw new InvalidOperationException("Bạn đã sử dụng phiếu giảm giá này.");
                    }
                    //if (orderDto.TotalAmount < (decimal)voucher.MinOrder)
                    //{
                    //    _logger.LogError("Order total {TotalAmount} is less than minimum required {MinOrder} for voucher {VoucherId}.",
                    //        orderDto.TotalAmount, voucher.MinOrder, orderDto.VoucherId);
                    //    throw new InvalidOperationException($"Đơn hàng phải có giá trị ít nhất là {voucher.MinOrder} để sử dụng phiếu giảm giá này.");
                    //}

                    if (voucher.ExpireDate < DateTime.Now)
                    {
                        _logger.LogError("Voucher {VoucherId} has expired.", orderDto.VoucherId);
                        throw new InvalidOperationException("Phiếu giảm giá này đã hết hạn.");
                    }
                    if (voucher.noOfVoucherUsed >= voucher.NoOfVoucher)
                    {
                        _logger.LogError("Voucher {VoucherId} is out of stock.", orderDto.VoucherId);
                        throw new InvalidOperationException("Phiếu giảm giá này đã hết hàng.");
                    }
                    if (orderDto.PreTotalAmount < (decimal)voucher.MinOrder)
                    {
                        _logger.LogError("Order total {TotalAmount} is less than minimum required {MinOrder} for voucher {VoucherId}.",
                            orderDto.TotalAmount, voucher.MinOrder, orderDto.VoucherId);
                        throw new InvalidOperationException($"Đơn hàng phải có giá trị ít nhất là {voucher.MinOrder} để sử dụng phiếu giảm giá này.");
                    }
                }

                int orderNumber = await _orderRepository.GetTodayOrderCount();
                string orderCode = GenerateOrderCode(orderNumber);
                var order = new Domain.Entities.Order
                {
                    Id = Guid.NewGuid(),
                    OrderCode = orderCode,
                    CustomerId = orderDto.CustomerId,
                    Status = (int)OrderEnum.EStatus.Pending,
                    OrderType = OrderEnum.EOrderType.Online.ToString(),
                    BranchId = orderDto.BranchId,
                    TableId = null,
                    ShippingAddress = orderDto.ShippingAddress,
                    RecipientPhone = orderDto.RecipientPhone,
                    RecipientName = orderDto.RecipientName,
                    TotalAmount = orderDto.TotalAmount,
                    PaymentMethod = orderDto.PaymentMethod,
                    OrderDetails = new List<OrderDetail>(),
                    voucherId = orderDto.VoucherId ?? Guid.Empty,
                    ShippingFee = orderDto.ShippingFee,
                    VoucherAmount = orderDto.VoucherAmount,
                };



                foreach (var detail in orderDto.OrderDetails)
                {
                    var product = await _orderRepository.GetProductById(detail.ProductId);
                    if (product == null)
                    {
                        throw new KeyNotFoundException($"Sản phẩm {detail.ProductId} không tồn tại.");
                    }
                    decimal price = product.Price;
                    decimal itemTotal = price * detail.Quantity;
                    var orderDetail = new OrderDetail
                    {
                        Id = Guid.NewGuid(),
                        ProductId = detail.ProductId,
                        Quantity = detail.Quantity,
                        Price = itemTotal
                    };

                    order.OrderDetails.Add(orderDetail);

                }
                if (voucher != null)
                {
                    await UpdateVoucherUsageAsync(orderDto.VoucherId.Value, orderDto.CustomerId);
                }


                _logger.LogInformation("Order {OrderId} created successfully.", order.Id);
                try
                {

                    await _orderRepository.CreateOrderOnlAsync(order);
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error while saving order.");
                    throw new Exception("Database error. Please try again later.", ex);
                }


                return order.Id;
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Key not found: {Message}", ex.Message);
                throw;
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid argument: {Message}", ex.Message);
                throw;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Invalid operation: {Message}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating order with customer ID {CustomerId}", orderDto.CustomerId);
                throw new Exception("An unexpected error occurred while creating the order.", ex);
            }
        }

        private string GenerateOrderCode(int orderNumber)
        {
            string datePart = DateTime.Now.ToString("yyMMdd");
            string orderPart = orderNumber.ToString("D4");

            return $"ORD{datePart}{orderPart}";
        }

        private async Task UpdateVoucherUsageAsync(Guid voucherId, Guid userId)
        {
            var voucher = await _orderRepository.GetVoucherByIdAsync(voucherId);
            if (voucher == null)
            {
                _logger.LogError("Voucher {VoucherId} does not exist.", voucherId);
                throw new KeyNotFoundException($"Mã giảm giá {voucherId} không tồn tại.");
            }

            if (voucher.NoOfVoucher - voucher.noOfVoucherUsed <= 0)
            {
                _logger.LogError("Voucher {VoucherId} has been fully used.", voucherId);
                throw new InvalidOperationException("Phiếu giảm giá này hiện không còn nữa.");
            }

            voucher.noOfVoucherUsed += 1;
            await _orderRepository.UpdateVoucherAsync(voucher);

            var voucherUsage = new VoucherUsage
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                VoucherId = voucherId,
                UsedAt = DateTime.Now,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsDeleted = false,
                CreatedBy = userId,
                UpdatedBy = userId
            };
            await _orderRepository.AddVoucherUsageAsync(voucherUsage);

            _logger.LogInformation("Voucher {VoucherId} used by User {UserId} at {UsedAt}",
                voucherId, userId, voucherUsage.UsedAt);
        }



        public async Task<Guid> CreateOrderOff(CreateOrderOffDto orderDto)
        {
            _logger.LogInformation("Entered method CreateOrderOff in OrderService");

            try
            {
                var branchTable = await _orderRepository.GetBranchTableById(orderDto.TableId);
                if (branchTable == null)
                {
                    _logger.LogError("Invalid Table ID: {TableId}", orderDto.TableId);
                    throw new ArgumentException($"The Table ID '{orderDto.TableId}' không tồn tại.");
                }

                if (branchTable.IsOccupied)
                {
                    _logger.LogError("Table {TableId} is already occupied.", orderDto.TableId);
                    throw new ArgumentException($"Bàn đã có khách.");
                }
                bool isGuest = !orderDto.CustomerId.HasValue;
                if (!isGuest)
                {
                    var customer = await _orderRepository.GetCustomerByIdAsync(orderDto.CustomerId.Value);
                    if (customer == null)
                    {
                        _logger.LogError("Customer {CustomerId} does not exist.", orderDto.CustomerId);
                        throw new KeyNotFoundException($"Khách hàng {orderDto.CustomerId} không tồn tại.");
                    }
                }
                var branchId = branchTable.BranchId;
                Voucher voucher = null;
                if (!isGuest && orderDto.VoucherId.HasValue)
                {
                    voucher = await _orderRepository.GetVoucherByIdAsync(orderDto.VoucherId.Value);
                    if (voucher == null)
                    {
                        _logger.LogError("Voucher {VoucherId} does not exist.", orderDto.VoucherId);
                        throw new KeyNotFoundException($"Phiếu mua hàng {orderDto.VoucherId} không tồn tại.");
                    }
                    var voucherUsage = orderDto.CustomerId.HasValue
              ? await _orderRepository.GetVoucherUsageByUserAndVoucherAsync(orderDto.CustomerId.Value, orderDto.VoucherId.Value)
              : null;
                    if (voucherUsage != null)
                    {
                        _logger.LogError("Customer {CustomerId} has already used voucher {VoucherId}.", orderDto.CustomerId, orderDto.VoucherId);
                        throw new InvalidOperationException("Bạn đã sử dụng phiếu giảm giá này.");
                    }
                    if (voucher.ExpireDate < DateTime.Now)
                    {
                        _logger.LogError("Voucher {VoucherId} has expired.", orderDto.VoucherId);
                        throw new InvalidOperationException($"Phiếu giảm giá đã hết hạn.");
                    }
                    if (voucher.noOfVoucherUsed >= voucher.NoOfVoucher)
                    {
                        _logger.LogError("Voucher {VoucherId} is out of stock.", orderDto.VoucherId);
                        throw new InvalidOperationException("Phiếu giảm giá này đã hết hàng.");
                    }
                    if (orderDto.PreTotalAmount < (decimal)voucher.MinOrder)
                    {
                        _logger.LogError("Order total {TotalAmount} is less than minimum required {MinOrder} for voucher {VoucherId}.",
                            orderDto.TotalAmount, voucher.MinOrder, orderDto.VoucherId);
                        throw new InvalidOperationException($"Đơn hàng phải có giá trị ít nhất là {voucher.MinOrder} để sử dụng phiếu giảm giá này.");
                    }
                }
                else if (isGuest && orderDto.VoucherId.HasValue)
                {
                    throw new InvalidOperationException("Khách chưa đăng ký tài khoản không thể sử dụng phiếu giảm giá.");
                }
                int orderNumber = await _orderRepository.GetTodayOrderCount();
                string orderCode = GenerateOrderCode(orderNumber);
                decimal voucherAmount = isGuest ? 0 : orderDto.VoucherAmount;
                var order = new Domain.Entities.Order
                {
                    Id = Guid.NewGuid(),
                    OrderCode = orderCode,
                    CustomerId = orderDto.CustomerId,
                    Status = (int)OrderEnum.EStatus.Pending,
                    OrderType = OrderEnum.EOrderType.Offline.ToString(),
                    BranchId = branchId,
                    RecipientName = "",
                    RecipientPhone = "",
                    ShippingAddress = "",
                    TableId = orderDto.TableId,
                    TotalAmount = orderDto.TotalAmount,
                    voucherId = orderDto.VoucherId ?? Guid.Empty,
                    OrderDetails = new List<OrderDetail>(),
                    ShippingFee = 0,
                    VoucherAmount = voucherAmount

                };

                foreach (var detail in orderDto.OrderDetails)
                {
                    var product = await _orderRepository.GetProductById(detail.ProductId);
                    if (product == null)
                    {
                        throw new KeyNotFoundException($"Sản phẩm {detail.ProductId} không tồn tại.");
                    }
                    decimal price = product.Price;
                    decimal itemTotal = price * detail.Quantity;
                    var orderDetail = new OrderDetail
                    {
                        Id = Guid.NewGuid(),
                        ProductId = detail.ProductId,
                        Quantity = detail.Quantity,
                        Price = itemTotal
                        //    Price = detail.Price
                    };

                    order.OrderDetails.Add(orderDetail);
                }

                if (voucher != null)
                {
                    await UpdateVoucherUsageAsync(orderDto.VoucherId.Value, orderDto.CustomerId.Value);
                }
                try
                {
                    await _orderRepository.CreateOrderOnlAsync(order);
                    branchTable.IsOccupied = true;
                    await _orderRepository.UpdateBranchTableAsync(branchTable);
                }

                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error while saving order.");
                    throw new Exception("Database error. Please try again later.", ex);
                }


                return order.Id;
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Key not found: {Message}", ex.Message);
                throw;
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid argument: {Message}", ex.Message);
                throw;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Invalid operation: {Message}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating offline order for customer ID {CustomerId}", orderDto.CustomerId);
                throw;
            }
        }



        public async Task<bool> CancelOrderOffAsync(Guid orderId)
        {
            try
            {
                _logger.LogInformation("Processing cancellation for order {OrderId}", orderId);

                var order = await _orderRepository.GetOrderById(orderId);
                if (order == null)
                {
                    _logger.LogWarning("Order {OrderId} not found", orderId);
                    throw new KeyNotFoundException("Không tìm thấy đơn hàng");
                }
                if (order.OrderType == OrderEnum.EOrderType.Online.ToString())
                {
                    _logger.LogWarning("Order {OrderId} is an online order and cannot be canceled.", orderId);
                    throw new InvalidOperationException("Không thể hủy đơn hàng trực tuyến.");
                }

                var orderStatus = (OrderEnum.EStatus)order.Status;
                if (orderStatus == OrderEnum.EStatus.Completed || orderStatus == OrderEnum.EStatus.Processing || orderStatus == OrderEnum.EStatus.Canceled)
                {
                    _logger.LogWarning("Order {OrderId} cannot be canceled because its status is {Status}", orderId, orderStatus);
                    return false;
                }

                if (orderStatus == OrderEnum.EStatus.Pending)
                {
                    if (order.CustomerId.HasValue && order.voucherId != Guid.Empty)
                    {
                        var voucherUsage = await _orderRepository.GetVoucherUsageByUserAndVoucherAsync(order.CustomerId.Value, order.voucherId);
                        if (voucherUsage != null && voucherUsage.UsedAt.Date == order.CreatedAt.Date)
                        {
                            var voucher = voucherUsage.Voucher;
                            if (voucher != null)
                            {
                                voucherUsage.IsDeleted = true;
                                await _orderRepository.UpdateVoucherUsage(voucherUsage);

                                voucher.noOfVoucherUsed = Math.Max(0, voucher.noOfVoucherUsed - 1);
                                await _orderRepository.UpdateVoucherAsync(voucher);
                            }
                        }
                    }

                    order.Status = (int)OrderEnum.EStatus.Canceled;
                    order.UpdatedAt = DateTime.Now;
                    await _orderRepository.UpdateStatusOrder(order);

                    _logger.LogInformation("Order {OrderId} successfully canceled", orderId);
                    return true;
                }

                return false;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Invalid operation while canceling order {OrderId}", orderId);
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while canceling order {OrderId}", orderId);
                throw new InvalidOperationException("A database error occurred. Please try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while canceling order {OrderId}", orderId);
                throw;
            }
        }
        public async Task<OrderDto> GetOrderById(Guid orderId)
        {
            _logger.LogInformation("Fetching order with ID {OrderId}", orderId);

            try
            {
                var order = await _orderRepository.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    _logger.LogWarning("Order {OrderId} not found", orderId);
                    throw new KeyNotFoundException($"Không tìm thấy đơn hàng có ID {orderId}.");
                }

                return new OrderDto
                {
                    CustomerId = order.User.Id,
                    CustomerName = order.User.FullName,
                    OrderId = order.Id,
                    OrderCode = order.OrderCode,
                    TotalAmount = order.TotalAmount,
                    RecipientName = order.RecipientName,
                    RecipientPhone = order.RecipientPhone,
                    ShippingAddress = order.ShippingAddress,
                    Status = order.Status,
                    PaymentMethod = order.PaymentMethod,
                    IsPaid = order.IsPaid,
                    ShippingFee = order.ShippingFee,
                    VoucherAmount = order.VoucherAmount,
                    TableId = order.TableId,
                    BranchId = order.BranchId,
                    BranchName = order.Branch.Name,
                    OrderType = order.OrderType,
                    CreatedAt = order.CreatedAt
                };
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while retrieving order {OrderId}", orderId);
                throw new InvalidOperationException("Database error occurred. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching order {OrderId}", orderId);
                throw;
            }
        }
        public async Task<OrderDto> GetOrderByIdHub(Guid orderId)
        {
            _logger.LogInformation("Fetching order with ID {OrderId}", orderId);

            try
            {
                var order = await _orderRepository.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    _logger.LogWarning("Order {OrderId} not found", orderId);
                    throw new KeyNotFoundException($"Không tìm thấy đơn hàng có ID {orderId}.");
                }

                return new OrderDto
                {
                    CustomerId = order.User?.Id ?? Guid.Empty,
                    CustomerName = order.User?.FullName ?? string.Empty,
                    OrderId = order.Id,
                    OrderCode = order.OrderCode,
                    TotalAmount = order.TotalAmount,
                    RecipientName = order.RecipientName,
                    RecipientPhone = order.RecipientPhone,
                    ShippingAddress = order.ShippingAddress,
                    Status = order.Status,
                    PaymentMethod = order.PaymentMethod,
                    IsPaid = order.IsPaid,
                    ShippingFee = order.ShippingFee,
                    VoucherAmount = order.VoucherAmount,
                    TableId = order.TableId,
                    BranchId = order.Branch?.Id ?? Guid.Empty,
                    BranchName = order.Branch?.Name ?? string.Empty,
                    OrderType = order.OrderType,
                    CreatedAt = order.CreatedAt
                };
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while retrieving order {OrderId}", orderId);
                throw new InvalidOperationException("Database error occurred. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching order {OrderId}", orderId);
                throw;
            }
        }

        // manager
        public async Task<List<OrderDto>> GetOrdersAsync(string? orderCode, int? status)
        {
            _logger.LogInformation("Fetching all  orders");

            try
            {
                var orders = await _orderRepository.GetOrders(orderCode, status);
                if (orders == null || !orders.Any())
                {
                    _logger.LogWarning("No orders found with OrderCode: {OrderCode} and Status: {Status}", orderCode, status);
                    throw new KeyNotFoundException("Không tìm thấy đơn hàng");
                }
                return _mapper.Map<List<OrderDto>>(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching orders");
                throw;
            }
        }


        public async Task<GetOrdersResponseDto> GetOrderOnlsAsync(
     string? orderCode, int? status, Guid? branchID, int page, int pageSize)
        {
            _logger.LogInformation("Fetching all online orders");

            try
            {
                var branchIds = await _orderRepository.GetListIdBranch();
                if (branchID.HasValue && !branchIds.Contains(branchID.Value))
                {
                    _logger.LogWarning("Branch with ID {BranchID} not found.", branchID);
                    throw new KeyNotFoundException("Chi nhánh không tồn tại.");
                }
                var orders = await _orderRepository.GetOrderOnls(orderCode, status, branchID);
                var totalItems = orders.Count();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                if (totalItems == 0)
                {
                    _logger.LogWarning("No online orders found with OrderCode: {OrderCode} and Status: {Status}", orderCode, status);
                    throw new KeyNotFoundException("Không tìm thấy đơn hàng trực tuyến");
                }

                var paginatedOrders = orders
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return new GetOrdersResponseDto
                {
                    Items = _mapper.Map<List<OrderDto>>(paginatedOrders),
                    TotalItems = totalItems,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalPages = totalPages
                };
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Branch not found: {BranchID}", branchID);
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error occurred while fetching online orders.");
                throw new InvalidOperationException("A database error occurred. Please try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching online orders");
                throw;
            }
        }




        public async Task<GetOrdersResponseOffDto> GetOrderOffsAsync(string? orderCode, int? status, Guid? branchID, string? tableNumber, int page, int pageSize)
        {
            _logger.LogInformation("Fetching all offline orders");

            try
            {
                var branchIds = await _orderRepository.GetListIdBranch();
                if (branchID.HasValue && !branchIds.Contains(branchID.Value))
                {
                    _logger.LogWarning("Branch with ID {BranchID} not found.", branchID);
                    throw new KeyNotFoundException("Chi nhánh không tồn tại.");
                }
                if (!string.IsNullOrEmpty(tableNumber))
                {
                    if (!branchID.HasValue)
                        throw new ArgumentException("BranchID là bắt buộc khi lọc theo TableNumber.");

                    var isTableExists = await _orderRepository.IsTableNumberExistInBranch(branchID.Value, tableNumber);
                    if (!isTableExists)
                        throw new KeyNotFoundException($"Table '{tableNumber}' does not exist in the selected branch.");
                }
                var orders = await _orderRepository.GetOrderOffs(orderCode, status, branchID, tableNumber);
                var totalItems = orders.Count();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                if (totalItems == 0)
                {
                    _logger.LogWarning("No orders found with OrderCode: {OrderCode} and Status: {Status}", orderCode, status);
                    throw new KeyNotFoundException("Không tìm thấy đơn hàng nào");
                }

                var paginatedOrders = orders
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                var todayStart = DateTime.Today;
                var todayEnd = todayStart.AddDays(1).AddTicks(-1);

                var completedOrdersToday = paginatedOrders
                    .Where(o => o.Status == 2 && o.CreatedAt >= todayStart && o.CreatedAt <= todayEnd)
                    .ToList();
                var totalRevenue = completedOrdersToday.Sum(o => o.TotalAmount);
                var completedOrdersCount = completedOrdersToday.Count;
                var averageOrderValue = completedOrdersCount > 0 ? totalRevenue / completedOrdersCount : 0;
                return new GetOrdersResponseOffDto
                {
                    Items = _mapper.Map<List<OrderOffDto>>(paginatedOrders),
                    TotalItems = totalItems,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    CompletedOrdersCount = completedOrdersCount,
                    TotalRevenue = totalRevenue,
                    AverageOrderValue = averageOrderValue
                };
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Branch not found: {BranchID}", branchID);
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error occurred while fetching online orders.");
                throw new InvalidOperationException("A database error occurred. Please try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching offline orders");
                throw;
            }
        }
        public async Task<GetOrdersResponseOffDto> GetOrderOff(string? orderCode, int? status, Guid? branchID, Guid? tableId, int page, int pageSize)
        {
            _logger.LogInformation("Fetching all offline orders");

            try
            {
                var branchIds = await _orderRepository.GetListIdBranch();
                if (branchID.HasValue && !branchIds.Contains(branchID.Value))
                {
                    _logger.LogWarning("Branch with ID {BranchID} not found.", branchID);
                    throw new KeyNotFoundException("Chi nhánh không tồn tại.");
                }

                if (tableId.HasValue)
                {
                    if (!branchID.HasValue)
                        throw new ArgumentException("BranchID là bắt buộc khi lọc theo TableId.");

                    var isTableExists = await _orderRepository.IsIdTableIdExistInBranch(branchID.Value, tableId.Value);
                    if (!isTableExists)
                        throw new KeyNotFoundException($"Table với ID '{tableId}' không tồn tại trong chi nhánh.");
                }

                var orders = await _orderRepository.GetOrderOff(orderCode, status, branchID, tableId);
                var totalItems = orders.Count();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                if (totalItems == 0)
                {
                    _logger.LogWarning("No orders found with OrderCode: {OrderCode} and Status: {Status}", orderCode, status);
                    throw new KeyNotFoundException("Không tìm thấy đơn hàng nào");
                }

                var paginatedOrders = orders
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var todayStart = DateTime.Today;
                var todayEnd = todayStart.AddDays(1).AddTicks(-1);

                var completedOrdersToday = paginatedOrders
                    .Where(o => o.Status == 2 && o.CreatedAt >= todayStart && o.CreatedAt <= todayEnd)
                    .ToList();

                var totalRevenue = completedOrdersToday.Sum(o => o.TotalAmount);
                var completedOrdersCount = completedOrdersToday.Count;
                var averageOrderValue = completedOrdersCount > 0 ? totalRevenue / completedOrdersCount : 0;

                return new GetOrdersResponseOffDto
                {
                    Items = _mapper.Map<List<OrderOffDto>>(paginatedOrders),
                    TotalItems = totalItems,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    CompletedOrdersCount = completedOrdersCount,
                    TotalRevenue = totalRevenue,
                    AverageOrderValue = averageOrderValue
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching offline orders");
                throw;
            }
        }


        public async Task<List<TableDto>> GetTablesAsync(Guid? branchId, bool? isOccupied, string? tableNumber)
        {
            var tables = await _orderRepository.GetTablesAsync(branchId, isOccupied, tableNumber);

            var tableDtos = tables.Select(t => new TableDto
            {
                TableId = t.Id,
                TableNumber = t.TableNumber,
                BranchId = t.BranchId,
                BranchName = t.Branch.Name,
                IsOccupied = t.IsOccupied,
                ActiveOrderCount = t.Orders.Count(o => o.Status == 0 || o.Status == 1 || o.Status == 4),
                TotalAmount = t.Orders
                .Where(o => o.Status == 0 || o.Status == 1 || o.Status == 4)
                .Sum(o => o.TotalAmount)
            }).ToList();

            return tableDtos;
        }
        public async Task<bool> AcceptOrderOnlAsync(Guid orderId)
        {
            _logger.LogInformation("Processing AcceptOrderAsync for order {OrderId}", orderId);

            var order = await _orderRepository.GetOrderByIdForManager(orderId);
            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} not found", orderId);
                throw new KeyNotFoundException("Không tìm thấy đơn hàng nào");
            }
            if (order.OrderType != OrderEnum.EOrderType.Online.ToString())
            {
                _logger.LogWarning("Order {OrderId} is not an online order", orderId);
                throw new InvalidOperationException("Chỉ có thể chấp nhận đơn hàng trực tuyến Online");
            }
            if (order.Status == (int)OrderEnum.EStatus.Completed)
                throw new InvalidOperationException("Đơn hàng đã hoàn tất.");
            if (order.Status == (int)OrderEnum.EStatus.Canceled)
                throw new InvalidOperationException("Đơn hàng đã bị hủy.");
            if (order.Status == (int)OrderEnum.EStatus.Processing)
                throw new InvalidOperationException("Đơn hàng đang được xử lý.");
            if (order.Status != (int)OrderEnum.EStatus.Pending)
            {
                throw new InvalidOperationException("Chỉ có thể chấp nhận những đơn hàng đang ở trạng thái 'Chờ xử lý'.");
            }
            //if (order.Status == (int)OrderEnum.EStatus.Processed)
            //    throw new InvalidOperationException("Đơn hàng đã được xử lý và không thể thay đổi.");
            if (order.Status == (int)OrderEnum.EStatus.Delivering)
                throw new InvalidOperationException("Đơn hàng hiện đang được giao và không thể thay đổi.");
            if (order.OrderDetails == null || !order.OrderDetails.Any())
            {
                _logger.LogWarning("Order {OrderId} has no order details", orderId);
                throw new InvalidOperationException($"Đơn hàng không có sản phẩm nào để xử lý.");
            }
            if (order.PaymentMethod == (int)EPaymentMethod.Banking && !order.IsPaid)
            {
                _logger.LogWarning("Order {OrderId} is paid via Banking but has not been marked as paid.", orderId);
                throw new InvalidOperationException("Đơn hàng chưa chuyển khoản để thanh toán bằng hình thức chuyển khoản.");
            }
            // Dictionary để lưu tổng số lượng nguyên liệu cần thiết cho đơn hàng
            var ingredientRequirements = new Dictionary<Guid, decimal>();

            foreach (var orderDetail in order.OrderDetails)
            {
                var product = await _orderRepository.GetProductById(orderDetail.ProductId);
                if (product == null)
                {
                    _logger.LogWarning("Product {ProductId} not found", orderDetail.ProductId);
                    throw new KeyNotFoundException($"Không tìm thấy sản phẩm có ID {orderDetail.ProductId}");
                }
                if (product.ProductIngredients == null || !product.ProductIngredients.Any())
                {
                    _logger.LogWarning("Product {ProductId} has no ingredients defined", orderDetail.ProductId);
                    throw new InvalidOperationException($"Sản phẩm {orderDetail.ProductId} không có thành phần nào được xác định.");
                }
                foreach (var productIngredient in product.ProductIngredients)
                {
                    decimal requiredQuantity = productIngredient.QuantityRequired * orderDetail.Quantity;

                    if (ingredientRequirements.ContainsKey(productIngredient.IngredientId))
                        ingredientRequirements[productIngredient.IngredientId] += requiredQuantity;
                    else
                        ingredientRequirements[productIngredient.IngredientId] = requiredQuantity;
                }
            }

            //  Kiểm tra kho xem có đủ nguyên liệu không
            var insufficientIngredients = new List<string>(); // Danh sách nguyên liệu thiếu

            foreach (var requirement in ingredientRequirements)
            {
                var ingredientId = requirement.Key;
                var totalRequiredQuantity = requirement.Value;

                // Lấy danh sách các lô nguyên liệu theo thứ tự nhập kho gần nhất
                var inventoryBatches = await _orderRepository.GetInventoryBatchesByIngredientAndBranch(ingredientId, order.BranchId);
                if (inventoryBatches == null || !inventoryBatches.Any())
                {
                    var ingredient = await _orderRepository.GetIngredientById(ingredientId);
                    string ingredientName = ingredient != null ? ingredient.Name : $"Unknown Ingredient ({ingredientId})";

                    _logger.LogWarning("No inventory batches found for ingredient {IngredientName} ({IngredientId}) in branch {BranchId}",
                        ingredientName, ingredientId, order.BranchId);
                    throw new InvalidOperationException($"Không có sẵn hàng tồn kho cho thành phần {ingredientName}.");
                }

                decimal availableQuantity = inventoryBatches.Sum(ii => ii.Quantity); // Tổng số lượng có sẵn

                if (availableQuantity < totalRequiredQuantity)
                {
                    decimal missingQuantity = totalRequiredQuantity - availableQuantity;
                    var ingredient = await _orderRepository.GetIngredientById(ingredientId);
                    insufficientIngredients.Add($"Nguyên liệu {ingredient.Name} còn thiếu {missingQuantity} đơn vị.");
                }
            }

            if (insufficientIngredients.Count > 0)
            {
                throw new InvalidOperationException("Không đủ nguyên liệu cho đơn hàng. Các nguyên liệu còn thiếu:" + string.Join(",", insufficientIngredients));
            }

            try
            {
                //  Trừ nguyên liệu trong kho theo lô mới nhất trước
                foreach (var requirement in ingredientRequirements)
                {
                    var ingredientId = requirement.Key;
                    var totalRequiredQuantity = requirement.Value;

                    var inventoryBatches = await _orderRepository.GetInventoryBatchesByIngredientAndBranch(ingredientId, order.BranchId);

                    foreach (var batch in inventoryBatches)
                    {
                        if (totalRequiredQuantity <= 0) break;

                        decimal quantityToDeduct = Math.Min(batch.Quantity, totalRequiredQuantity);
                        batch.Quantity -= quantityToDeduct;
                        totalRequiredQuantity -= quantityToDeduct;
                        _logger.LogInformation("Deducted {DeductedQuantity} units from batch {BatchId} for ingredient {IngredientId}", quantityToDeduct, batch.Id, ingredientId);

                        await _orderRepository.UpdateInventoryItem(batch);
                    }
                }
                order.Status = (int)OrderEnum.EStatus.Processing;
                await _orderRepository.UpdateStatusOrder(order);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while processing order {OrderId}", orderId);
                throw new InvalidOperationException("Database error occurred while processing the order.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing order {OrderId}", orderId);
                throw;
            }


            _logger.LogInformation("Order {OrderId} has been successfully processed.", orderId);
            return true;
        }
        public async Task<bool> AcceptOrderOffAsync(Guid orderId)
        {
            _logger.LogInformation("Processing AcceptOrderAsync for order {OrderId}", orderId);

            var order = await _orderRepository.GetOrderByIdForManager(orderId);
            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} not found", orderId);
                throw new KeyNotFoundException("Không tìm thấy đơn hàng");
            }
            if (order.OrderType != OrderEnum.EOrderType.Offline.ToString())
            {
                _logger.LogWarning("Order {OrderId} is not an offline order", orderId);
                throw new InvalidOperationException("Chỉ có thể chấp nhận đơn hàng tại cửa hàng (offline).");
            }
            if (order.Status != (int)OrderEnum.EStatus.Pending)
            {
                throw new InvalidOperationException("Chỉ có thể chấp nhận những đơn hàng đang ở trạng thái 'Chờ xử lý'.");
            }
            if (order.OrderDetails == null || !order.OrderDetails.Any())
            {
                _logger.LogWarning("Order {OrderId} has no order details", orderId);
                throw new InvalidOperationException($"Đơn hàng không có sản phẩm nào để xử lý.");
            }

            // Dictionary để lưu tổng số lượng nguyên liệu cần thiết cho đơn hàng
            var ingredientRequirements = new Dictionary<Guid, decimal>();

            foreach (var orderDetail in order.OrderDetails)
            {
                var product = await _orderRepository.GetProductById(orderDetail.ProductId);
                if (product == null)
                {
                    _logger.LogWarning("Product {ProductId} not found", orderDetail.ProductId);
                    throw new KeyNotFoundException($"Không tìm thấy sản phẩm có ID {orderDetail.ProductId}");
                }
                if (product.ProductIngredients == null || !product.ProductIngredients.Any())
                {
                    _logger.LogWarning("Product {ProductId} has no ingredients defined", orderDetail.ProductId);
                    throw new InvalidOperationException($"Sản phẩm {orderDetail.ProductId} không có thành phần nào được xác định.");
                }
                foreach (var productIngredient in product.ProductIngredients)
                {
                    decimal requiredQuantity = productIngredient.QuantityRequired * orderDetail.Quantity;

                    if (ingredientRequirements.ContainsKey(productIngredient.IngredientId))
                        ingredientRequirements[productIngredient.IngredientId] += requiredQuantity;
                    else
                        ingredientRequirements[productIngredient.IngredientId] = requiredQuantity;
                }
            }

            //  Kiểm tra kho xem có đủ nguyên liệu không
            var insufficientIngredients = new List<string>(); // Danh sách nguyên liệu thiếu

            foreach (var requirement in ingredientRequirements)
            {
                var ingredientId = requirement.Key;
                var totalRequiredQuantity = requirement.Value;

                // Lấy danh sách các lô nguyên liệu theo thứ tự nhập kho gần nhất
                var inventoryBatches = await _orderRepository.GetInventoryBatchesByIngredientAndBranch(ingredientId, order.BranchId);
                if (inventoryBatches == null || !inventoryBatches.Any())
                {
                    var ingredient = await _orderRepository.GetIngredientById(ingredientId);
                    string ingredientName = ingredient != null ? ingredient.Name : $"Unknown Ingredient ({ingredientId})";

                    _logger.LogWarning("No inventory batches found for ingredient {IngredientName} ({IngredientId}) in branch {BranchId}",
                        ingredientName, ingredientId, order.BranchId);
                    throw new InvalidOperationException($"Không có sẵn hàng tồn kho cho thành phần {ingredientName}.");
                }

                decimal availableQuantity = inventoryBatches.Sum(ii => ii.Quantity); // Tổng số lượng có sẵn

                if (availableQuantity < totalRequiredQuantity)
                {
                    decimal missingQuantity = totalRequiredQuantity - availableQuantity;
                    var ingredient = await _orderRepository.GetIngredientById(ingredientId);
                    insufficientIngredients.Add($"Thành phần {ingredient.Name} bị thiếu {missingQuantity} đơn vị.");
                }
            }

            if (insufficientIngredients.Count > 0)
            {
                throw new InvalidOperationException("Không đủ dùng cho các thành phần sau:\n" + string.Join("\n", insufficientIngredients));
            }

            try
            {
                //  Trừ nguyên liệu trong kho theo lô mới nhất trước
                foreach (var requirement in ingredientRequirements)
                {
                    var ingredientId = requirement.Key;
                    var totalRequiredQuantity = requirement.Value;

                    var inventoryBatches = await _orderRepository.GetInventoryBatchesByIngredientAndBranch(ingredientId, order.BranchId);

                    foreach (var batch in inventoryBatches)
                    {
                        if (totalRequiredQuantity <= 0) break;

                        decimal quantityToDeduct = Math.Min(batch.Quantity, totalRequiredQuantity);
                        batch.Quantity -= quantityToDeduct;
                        totalRequiredQuantity -= quantityToDeduct;
                        _logger.LogInformation("Deducted {DeductedQuantity} units from batch {BatchId} for ingredient {IngredientId}", quantityToDeduct, batch.Id, ingredientId);

                        await _orderRepository.UpdateInventoryItem(batch);
                    }
                }
                order.Status = (int)OrderEnum.EStatus.Processing;
                await _orderRepository.UpdateStatusOrder(order);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while processing order {OrderId}", orderId);
                throw new InvalidOperationException("Database error occurred while processing the order.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing order {OrderId}", orderId);
                throw;
            }


            _logger.LogInformation("Order {OrderId} has been successfully processed.", orderId);
            return true;
        }


        public async Task<bool> CancelOrderByManagerAsync(Guid orderId, Guid? cancelReasonId, string customReason)
        {
            _logger.LogInformation("Processing cancellation for order {OrderId}", orderId);

            var order = await _orderRepository.GetOrderById(orderId);
            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} not found", orderId);
                throw new KeyNotFoundException("Không tìm thấy đơn hàng");
            }

            var orderStatus = (OrderEnum.EStatus)order.Status;
            if (orderStatus == OrderEnum.EStatus.Completed || orderStatus == OrderEnum.EStatus.Processing
                || orderStatus == OrderEnum.EStatus.Canceled || orderStatus == OrderEnum.EStatus.Delivering)
            {
                _logger.LogWarning("Order {OrderId} cannot be canceled because its status is {Status}", orderId, orderStatus);
                return false;
            }

            if (orderStatus == OrderEnum.EStatus.Pending)
            {
                if (order.CustomerId.HasValue && order.voucherId != Guid.Empty)
                {
                    var voucherUsage = await _orderRepository.GetVoucherUsageByUserAndVoucherAsync(order.CustomerId.Value, order.voucherId);
                    if (voucherUsage != null && voucherUsage.UsedAt.Date == order.CreatedAt.Date)
                    {
                        var voucher = voucherUsage.Voucher;
                        if (voucher != null)
                        {
                            voucherUsage.IsDeleted = true;
                            await _orderRepository.UpdateVoucherUsage(voucherUsage);
                            voucher.noOfVoucherUsed = Math.Max(0, voucher.noOfVoucherUsed - 1);
                            await _orderRepository.UpdateVoucherAsync(voucher);
                        }
                    }
                }

                var orderType = order.OrderType;


                var orderCancellation = new OrderCancellation
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderId,
                    CancelReasonId = cancelReasonId,
                    CustomReason = string.IsNullOrEmpty(customReason) ? string.Empty : customReason,
                    CreatedAt = DateTime.Now,
                    CreatedBy = AppConstants.SYSTEM_ROLE_GUID,
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = AppConstants.SYSTEM_ROLE_GUID,
                    CanceledBy = AppConstants.SYSTEM_ROLE_GUID,
                    IsDeleted = false
                };
                await _orderRepository.AddOrderCancellation(orderCancellation);




                order.Status = (int)OrderEnum.EStatus.Canceled;
                order.UpdatedAt = DateTime.Now;
                await _orderRepository.UpdateStatusOrder(order);

                _logger.LogInformation("Order {OrderId} successfully canceled", orderId);
                return true;
            }

            return false;
        }
        public async Task<bool> DeliverOrderAsync(Guid orderId)
        {
            _logger.LogInformation("Processing DeliverOrderAsync for order {OrderId}", orderId);

            var order = await _orderRepository.GetOrderByIdForManager(orderId);
            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} not found", orderId);
                throw new KeyNotFoundException("Không tìm thấy đơn hàng");
            }

            if (order.OrderType != OrderEnum.EOrderType.Online.ToString())
                throw new InvalidOperationException("Chỉ có thể giao hàng cho những đơn hàng trực tuyến.");

            if (order.Status != (int)OrderEnum.EStatus.Processing)
                throw new InvalidOperationException("Chỉ những đơn hàng đang xử lý mới có thể được giao.");

            try
            {
                order.Status = (int)OrderEnum.EStatus.Delivering;
                await _orderRepository.UpdateStatusOrder(order);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while updating order {OrderId} to Delivering", orderId);
                throw new InvalidOperationException("Database error occurred while updating the order status.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating order {OrderId} to Delivering", orderId);
                throw;
            }

            _logger.LogInformation("Order {OrderId} is now being delivered.", orderId);
            return true;
        }
        //public async Task<bool> ProcessOfflineOrderAsync(Guid orderId)
        //{
        //    _logger.LogInformation("Processing offline order {OrderId}", orderId);

        //    if (orderId == Guid.Empty)
        //    {
        //        _logger.LogWarning("Invalid orderId: {OrderId}", orderId);
        //        throw new ArgumentException("Mã đơn hàng không hợp lệ.");
        //    }

        //    var order = await _orderRepository.GetOrderById(orderId);
        //    if (order == null)
        //    {
        //        _logger.LogWarning("Order {OrderId} not found", orderId);
        //        throw new KeyNotFoundException("Không tìm thấy đơn hàng");
        //    }

        //    if (order.OrderType != OrderEnum.EOrderType.Offline.ToString())
        //    {
        //        _logger.LogWarning("Order {OrderId} is not an offline order", orderId);
        //        throw new InvalidOperationException("Chỉ có thể xử lý các đơn hàng ngoại tuyến.");
        //    }
        //    if (order.Status != (int)OrderEnum.EStatus.Processing)
        //    {
        //        _logger.LogWarning("Order {OrderId} is not in Processing status", orderId);
        //        throw new InvalidOperationException("Chỉ những đơn hàng ở trạng thái Đang xử lý mới có thể chuyển sang Đã xử lý.");
        //    }

        //    order.Status = (int)OrderEnum.EStatus.Processed;
        //    order.UpdatedAt = DateTime.Now;

        //    await _orderRepository.UpdateStatusOrder(order);

        //    _logger.LogInformation("Order {OrderId} successfully updated to Processed", orderId);
        //    return true;
        //}

        public async Task<bool> CompleteOfflineOrderAsync(Guid orderId, int paymentMethod)
        {
            _logger.LogInformation("Processing complete offline order {OrderId}", orderId);

            if (orderId == Guid.Empty)
            {
                _logger.LogWarning("OrderId {OrderId} trống !", orderId);
                throw new ArgumentException("Mã đơn hàng không hợp lệ .");
            }
            if (paymentMethod != (int)EPaymentMethod.Banking && paymentMethod != (int)EPaymentMethod.Cash)
            {
                _logger.LogWarning("Invalid payment method: {PaymentMethod}", paymentMethod);
                throw new ArgumentException("Phương thức thanh toán không hợp lệ. Chỉ chấp nhận 1 (chuyển khoản) hoặc 2 (tiền mặt).");
            }
            var order = await _orderRepository.GetOrderById(orderId);
            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} not found", orderId);
                throw new KeyNotFoundException("Không tìm thấy đơn hàng ");
            }

            if (order.OrderType != OrderEnum.EOrderType.Offline.ToString())
            {
                _logger.LogWarning("Order {OrderId} is not an offline order", orderId);
                throw new InvalidOperationException("Chỉ có thể hoàn tất các đơn hàng tại cửa hàng .");
            }
            if (order.Status != (int)OrderEnum.EStatus.Processing)
            {
                _logger.LogWarning("Order {OrderId} is not in Processing status", orderId);
                throw new InvalidOperationException("Chỉ những đơn hàng đang ở trạng thái Đang xử lý mới được hoàn tất.");
            }

            order.PaymentMethod = paymentMethod;
            order.Status = (int)OrderEnum.EStatus.Completed;
            order.IsPaid = true;
            order.UpdatedAt = DateTime.Now;
            await _orderRepository.UpdateStatusOrder(order);
            _logger.LogInformation("Order {OrderId} successfully completed", orderId);
            return true;
        }

        public async Task<BranchTableDetailDto> GetBranchTableDetailAsync(Guid tableId)
        {
            var branchTable = await _orderRepository.GetBranchTableWithOrdersAsync(tableId);

            if (branchTable == null)
            {
                return null;
            }

            var branchTableDetailDto = new BranchTableDetailDto
            {
                TableNumber = branchTable.TableNumber,
                IsOccupied = branchTable.IsOccupied,
                Orders = branchTable.Orders
                    .Where(o => o.Status == 0 || o.Status == 1 || o.Status == 4)
                    .Select(o => new OrderOfTableDto
                    {
                        OrderId = o.Id,
                        OrderCode = o.OrderCode,
                        Status = o.Status,
                        VoucherAmount = o.VoucherAmount,
                        TotalAmount = o.TotalAmount,
                        Products = o.OrderDetails.Select(od => new OrderOffProductDto
                        {
                            ProductName = od.Product.Name,
                            UnitPrice = od.Product.Price,
                            Quantity = od.Quantity
                        }).ToList()
                    })
                    .ToList()
            };

            return branchTableDetailDto;
        }


    }
}

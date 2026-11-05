using Domain.Constanst;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Order.API.Dtos.CancelReason;
using Order.API.Dtos.OrderOfCustomer;
using Order.API.Helpers;
using Order.API.Services.Interfaces;
using static Domain.Enums.OrderEnum;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IHubContext<CreateOrderHub> _orderHub;
        public OrderController(IOrderService orderService, IHubContext<CreateOrderHub> orderHub)
        {
            _orderService = orderService;
            _orderHub = orderHub;
        }
        [Authorize(Roles = $"{AppConstants.USER_ROLE_STRING}")]

        [HttpGet("get-all-order-a-customer")]

        public async Task<ActionResult<List<OrderOfACustomerDto>>> GetAllOrder1Customer(
           [FromQuery] Guid customerId,
           [FromQuery] string? orderCode = null,
           [FromQuery] int? status = null,
           [FromQuery] string? type = null,
            [FromQuery] int page = 1,
   [FromQuery] int pageSize = 10)
        {
            if (!string.IsNullOrEmpty(type) && type != OrderEnum.EOrderType.Online.ToString() && type != OrderEnum.EOrderType.Offline.ToString())
            {
                return BadRequest("Loại phải là 'Online' hoặc 'Offline'.");
            }
            if (page < 1 || pageSize < 1)
            {
                return BadRequest("Trang và Kích thước trang phải lớn hơn 0.");
            }
            try
            {
                var orders = await _orderService.GetCustomerOrdersAsync(customerId, orderCode, status, type, page, pageSize);
                return Ok(orders);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Lỗi cơ sở dữ liệu. Vui lòng thử lại sau.", ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [Authorize(Roles = $"{AppConstants.USER_ROLE_STRING}")]
        [HttpGet("get-order-by-id/{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {

            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);

                return Ok(order);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Lỗi cơ sở dữ liệu. Vui lòng thử lại sau." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi máy chủ nội bộ");
            }

        }
        [Authorize(Roles = $"{AppConstants.USER_ROLE_STRING}")]
        [HttpGet("get-order-codes-by-customer")]
        public async Task<ActionResult<List<string>>> GetOrderCodesByCustomerId([FromQuery] Guid customerId)
        {
            try
            {
                var orderCodes = await _orderService.GetListOrderCodeAsync(customerId);
                return Ok(orderCodes);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        //  [Authorize(Roles = $"{AppConstants.USER_ROLE_STRING}")]
        [HttpGet("get-all-cancel-reason")]
        public async Task<ActionResult<List<string>>> GetAllCancelReason()
        {
            try
            {
                var reasons = await _orderService.GetListCancelReasonAsync();
                return Ok(reasons);
            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [Authorize(Roles = $"{AppConstants.USER_ROLE_STRING}")]
        [HttpPut("cancel-order-online/{id}")]
        public async Task<IActionResult> CancelOrder(Guid id, [FromBody] CancelOrderRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Reason) && !request.CancelReasonId.HasValue)
            {
                return BadRequest(new { message = "Lý do là bắt buộc" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Dữ liệu yêu cầu hủy không hợp lệ." });
            }

            try
            {
                Guid? cancelReasonId = request.CancelReasonId;
                string customReason = !string.IsNullOrEmpty(request.Reason) ? request.Reason : null;

                var result = await _orderService.CancelOrderAsync(id, cancelReasonId, customReason);
                if (!result)
                {
                    return BadRequest(new { message = "Đơn hàng không thể hủy hoặc đã bị hủy." });
                }
                return Ok(new { message = "Hủy đơn hàng thành công" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }
        [Authorize(Roles = $"{AppConstants.USER_ROLE_STRING}")]
        [HttpPut("update-completed-order-onl/{id}")]
        public async Task<IActionResult> UpdateCompletedOrderOnl(Guid id)
        {
            try
            {
                var result = await _orderService.UpdateCompletedOrderOnlAsync(id);
                return Ok(new { message = "Đã hoàn thành đơn hàng " });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Lỗi cơ sở dữ liệu", details = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (TaskCanceledException ex)
            {
                return StatusCode(408, new { message = "Yêu cầu đã bị hủy. Vui lòng thử lại." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }

        [Authorize(Roles = $"{AppConstants.USER_ROLE_STRING}")]
        [HttpPost("create-order-online")]
        public async Task<IActionResult> CreateOrderOnl([FromBody] CreateOrderOnlDto orderDto)
        {

            if (orderDto == null)
            {
                return BadRequest(new { message = "Order DTO là null." });
            }
            if (!Enum.IsDefined(typeof(EPaymentMethod), orderDto.PaymentMethod))
            {
                return BadRequest(new { message = "Phương thức thanh toán phải là 1 (Banking) hoặc 2 (Tiền mặt)." });
            }
            if (!TryValidateModel(orderDto))
            {
                var errors = ModelState
                    .Where(ms => ms.Value.Errors.Count > 0)
                    .ToDictionary(
                        ms => ms.Key,
                        ms => ms.Value.Errors.Select(e => e.ErrorMessage).ToList()
                    );

                return BadRequest(new { errors });
            }

            try
            {
                var orderId = await _orderService.CreateOrderOnl(orderDto);
                var order = await _orderService.GetOrderById(orderId);
                if (order == null)
                {
                    return NotFound(new { message = "Không tìm thấy đơn hàng." });
                }
                try
                {
                    if (_orderHub?.Clients != null)
                    {
                        await _orderHub.Clients.All.SendAsync("NewOrderOnlCreated", order);
                    }
                }
                catch
                {
                }

                return Ok(new { message = "Tạo đơn hàng thành công.", orderId });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }


        //[Authorize(Roles = $"{AppConstants.USER_ROLE_STRING}")]
        [HttpPost("create-order-in-store")]
        public async Task<IActionResult> CreateOrderInStore([FromBody] CreateOrderOffDto orderDto)
        {
            if (orderDto == null)
            {
                return BadRequest("Order DTO là null.");
            }

            try
            {
                var orderId = await _orderService.CreateOrderOff(orderDto);
                var order = await _orderService.GetOrderByIdHub(orderId);
                try
                {
                    if (_orderHub?.Clients != null && order != null)
                    {
                        var tableStatus = new
                        {
                            TableId = order.TableId,
                            BranchId = order.BranchId,
                            IsOccupied = true,
                            TotalAmount = order.TotalAmount
                        };

                        await _orderHub.Clients.All.SendAsync("TableStatusChanged", tableStatus);
                    }
                }
                catch
                {
                }
                return Ok(new { message = "Tạo đơn hàng thành công.", orderId });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return UnprocessableEntity(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }

        //  [Authorize(Roles = $"{AppConstants.USER_ROLE_STRING}")]
        [HttpPut("cancel-order-off/{id}")]
        public async Task<IActionResult> CancelOrderOff(Guid id)
        {
            try
            {

                var result = await _orderService.CancelOrderOffAsync(id);
                if (!result)
                {
                    return BadRequest(new { message = "Đơn hàng không thể hủy hoặc đã bị hủy." });
                }
                return Ok(new { message = "Hủy đơn hàng thành công" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }

    }
}

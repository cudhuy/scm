using Domain.Constanst;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Order.API.Dtos.OrderManagement;
using Order.API.Services.Interfaces;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderManagementController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderManagementController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING},{AppConstants.STAFF_ROLE_STRING}")]
        [HttpGet("get-all-order-online")]

        public async Task<ActionResult<GetOrdersResponseDto>> GetAllOrderOnline(
    [FromQuery] string? orderCode = null,
    [FromQuery] int? status = null,
    [FromQuery] Guid? branchID = null,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10
)
        {
            try
            {
                if (page <= 0 || pageSize <= 0)
                {
                    return BadRequest(new { message = "Trang và Kích thước trang phải lớn hơn 0." });
                }
                if (status.HasValue && (status < 0 || status > 3))
                {
                    return BadRequest(new { message = "Giá trị trạng thái không hợp lệ. Các giá trị cho phép: 0, 1, 2, 3." });
                }
                var orderOnlines = await _orderService.GetOrderOnlsAsync(orderCode, status, branchID, page, pageSize);
                return Ok(orderOnlines);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi cơ sở dữ liệu.", details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING},{AppConstants.STAFF_ROLE_STRING}")]
        [HttpGet("get-order-in-store-completed-cancel")]
        public async Task<ActionResult<GetOrdersResponseOffDto>> GetAllOrderOffCompleted(
     [FromQuery] string? orderCode = null,
     [FromQuery] int? status = null,
     [FromQuery] Guid? branchID = null,
     [FromQuery] Guid? tableId = null,
     [FromQuery] int page = 1,
     [FromQuery] int pageSize = 10)

        {
            try
            {
                if (page <= 0 || pageSize <= 0)
                {
                    return BadRequest(new { message = "Trang và Kích thước trang phải lớn hơn 0." });
                }
                if (status.HasValue && (status != 2 && status != 3))
                {
                    return BadRequest(new { message = "Giá trị trạng thái không hợp lệ. Các giá trị cho phép: 0, 1, 2, 3." });
                }
                if (tableId.HasValue && !branchID.HasValue)
                    return BadRequest(new { message = "Bắt buộc lọc theo cả branch." });
                var orderResponse = await _orderService.GetOrderOff(orderCode, status, branchID, tableId, page, pageSize);
                return Ok(orderResponse);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi cơ sở dữ liệu.", details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING},{AppConstants.STAFF_ROLE_STRING}")]
        [HttpGet("get-table-in-store")]
        public async Task<ActionResult<List<TableDto>>> GetAllTablesInStore(
     [FromQuery] Guid? branchId = null,  
     [FromQuery] bool? isOccupied = null,
     [FromQuery] string? tableNumber = null 
 )
        {
            try
            {
                var tables = await _orderService.GetTablesAsync(branchId, isOccupied, tableNumber);
                if (tables == null || tables.Count == 0)
                {
                    return NotFound(new { message = "Không có bàn nào được tìm thấy !" });
                }
                return Ok(tables);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
            }
        }
        
        [HttpGet("get-detail-table/{tableId}")]
        public async Task<ActionResult<BranchTableDetailDto>> GetBranchTableDetailAsync(Guid tableId)
        {
            var detail = await _orderService.GetBranchTableDetailAsync(tableId);
            if (detail == null)
                return NotFound(new { message = "Không có bàn này !" });

            return Ok(detail);
        }


        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING},{AppConstants.STAFF_ROLE_STRING}")]
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
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi máy chủ nội bộ");
            }
        }
        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING},{AppConstants.STAFF_ROLE_STRING}")]
        [HttpPut("accept-order-onl/{id}")]
        public async Task<IActionResult> AcceptOrderOnl(Guid id)
        {

            try
            {
                var result = await _orderService.AcceptOrderOnlAsync(id);
                return Ok(new { message = "Chấp nhận đơn hàng thành công để xử lý!" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi cơ sở dữ liệu khi xử lý đơn hàng." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }
        
        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING},{AppConstants.STAFF_ROLE_STRING}")]
        [HttpPut("accept-order-off/{id}")]
        public async Task<IActionResult> AcceptOrderOff(Guid id)
        {

            try
            {
                var result = await _orderService.AcceptOrderOffAsync(id);
                return Ok(new { message = "Chấp nhận đơn hàng thành công để xử lý!" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi cơ sở dữ liệu khi xử lý đơn hàng." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }
        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING},{AppConstants.STAFF_ROLE_STRING}")]
        [HttpPut("deliver-order-onl/{id}")]
        public async Task<IActionResult> DeliverOrder(Guid id)
        {
            try
            {
                var result = await _orderService.DeliverOrderAsync(id);
                return Ok(new { message = "Đơn hàng trực tuyến hiện đang được giao!" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi cơ sở dữ liệu khi cập nhật trạng thái đơn hàng." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }
        //[Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING},{AppConstants.STAFF_ROLE_STRING}")]
        //[HttpPut("mark-offline-order-processed/{id}")]
        //public async Task<IActionResult> MarkOfflineOrderProcessed(Guid id)
        //{
        //    try
        //    {
        //        var result = await _orderService.ProcessOfflineOrderAsync(id);

        //        return Ok(new { message = "Đơn hàng đã được đánh dấu là đã xử lý thành công." });
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound(new { message = ex.Message });
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
        //    }
        //}
        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING},{AppConstants.STAFF_ROLE_STRING}")]
        [HttpPut("cancel-order-by-manager/{id}")]
        public async Task<IActionResult> CancelOrderByManager(Guid id, [FromBody] CancelOrderByManagerDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Dữ liệu yêu cầu hủy không hợp lệ." });
            }

            try
            {
                Guid? cancelReasonId = request.CancelReasonId;
                string customReason = !string.IsNullOrEmpty(request.Reason) ? request.Reason : null;

                var result = await _orderService.CancelOrderByManagerAsync(id, cancelReasonId, customReason);
                if (!result)
                {
                    return BadRequest(new { message = "Đơn hàng không thể bị hủy hoặc đã bị hủy." });
                }
                return Ok(new { message = "Đơn hàng đã được hủy thành công" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }
        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING},{AppConstants.STAFF_ROLE_STRING}")]
        [HttpPut("complete-offline-order/{id}")]
        public async Task<IActionResult> CompleteOfflineOrder(Guid id, [FromQuery] int paymentMethod)
        {
            try
            {
                var result = await _orderService.CompleteOfflineOrderAsync(id, paymentMethod);

                return Ok(new { message = "Đơn hàng tại cửa hàng đã được hoàn thành thành công." });
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
    }
}

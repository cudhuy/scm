using Branch.API.Services.Interfaces;
using Domain.Constanst;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Branch.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [Authorize(Roles = AppConstants.ADMIN_ROLE_STRING)]
        [HttpGet("revenue-report")]
        public async Task<IActionResult> GetRevenueReport(
            [FromQuery] Guid? branchId = null,
            [FromQuery] Guid? categoryId = null,
            [FromQuery] int year = 0)
        {
            try
            {
                if (year == 0) return BadRequest("Tham số năm là bắt buộc.");

                var result = await _reportService.GetRevenueReportAsync(branchId, categoryId, year);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi máy chủ nội bộ: {ex.Message}");
            }
        }

        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpGet("order-statistics")]
        public async Task<IActionResult> GetOrderStatistics([FromQuery] Guid? branchId,
            [FromQuery] int week,
            [FromQuery] int month,
            [FromQuery] int year)
        {
            if (week < 1 || week > 4)
            {
                return BadRequest("Tuần không hợp lệ. Sử dụng các giá trị từ 1 đến 4.");
            }

            if (month < 1 || month > 12)
            {
                return BadRequest("Tháng không hợp lệ. Sử dụng các giá trị từ 1 đến 12.");
            }

            if (year < 2000 || year > DateTime.UtcNow.Year)
            {
                return BadRequest("Năm không hợp lệ.");
            }

            var result = await _reportService.GetOrderStatistics(branchId, week, month, year);
            if (result != null) return Ok(result);
            return StatusCode(500, "Lỗi máy chủ nội bộ");
        }
        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpGet("report-count-summary")]
        public async Task<IActionResult> GetReportSummary([FromQuery] Guid? branchId = null)
        {
            try
            {
                int? totalCustomers = await _reportService.GetTotalCustomers();
                decimal? totalAmount = await _reportService.GetTotalAmountForStatus2Orders(branchId);
                int? totalOrders = await _reportService.GetTotalOrdersForStatus2(branchId);
                int? totalProducts = await _reportService.GetTotalProductsCount();

                if (totalCustomers == null && totalAmount == null && totalOrders == null && totalProducts == null)
                {
                    return NotFound(new { message = "Không tìm thấy dữ liệu báo cáo." });
                }

                return Ok(new
                {
                    TotalCustomers = totalCustomers ?? 0,
                    TotalAmount = totalAmount ?? 0,
                    TotalOrders = totalOrders ?? 0,
                    TotalProducts = totalProducts ?? 0
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }
        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpGet("get-recent-orders")]
        public async Task<IActionResult> GetLatestFiveOrders([FromQuery] Guid? branchId)
        {
            try
            {
                var orders = await _reportService.GetLatestFiveOrders(branchId);

                if (orders == null || !orders.Any())
                {
                    return NotFound(new { message = "Không có đơn hàng nào gần đây." });
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }
        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpGet("get-order-by-id/{id}")]

        public async Task<IActionResult> GetOrderById(Guid id)
        {
            try
            {
                var order = await _reportService.GetOrderByIdAsync(id);
                return Ok(order);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ" });
            }
        }
        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpGet("top-selling-products")]
        public async Task<IActionResult> GetTopSellingProducts([FromQuery] Guid? branchId)
        {
            try
            {
                var products = await _reportService.GetTopSellingProductsAsync(branchId);

                if (!products.Any())
                {
                    return NotFound(new { message = "Không tìm thấy sản phẩm bán chạy nhất." });
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }
        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpGet("get-recent-customers")]
        public async Task<IActionResult> GetRecentCustomers([FromQuery] Guid? branchId)
        {
            try
            {
                var customers = await _reportService.GetRecentCustomers(branchId);

                if (customers == null || !customers.Any())
                {
                    return NotFound(new { message = "Không tìm thấy khách hàng gần đây." });
                }

                return Ok(customers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }
        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpGet("get-detail-product/{id}")]
        public async Task<IActionResult> GetDetailProduct(Guid id)
        {
            try
            {
                var response = await _reportService.GetDetailProductsAsync(id);
                if (response == null)
                {
                    return NotFound("Không tìm thấy sản phẩm");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi máy chủ nội bộ");
            }
        }

        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpGet("get-detail-user/{id}")]
        public async Task<IActionResult> GetDetailUser(Guid id)
        {
            try
            {
                var response = await _reportService.GetDetailUserAsync(id);
                if (response == null)
                {
                    return NotFound("Không tìm thấy người dùng");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi máy chủ nội bộ");
            }
        }
    }
}

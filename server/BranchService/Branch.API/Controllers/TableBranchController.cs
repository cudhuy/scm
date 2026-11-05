using Branch.API.Dtos.TableBranch;
using Branch.API.Services.Interfaces;
using Domain.Constanst;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Branch.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableBranchController : ControllerBase
    {
        private readonly ITableBranchService _tableBranchService;

        public TableBranchController(ITableBranchService tableBranchService)
        {
            _tableBranchService = tableBranchService;
        }
        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpPost("create-table")]
        public async Task<IActionResult> CreateTable([FromBody] CreateTableDto createTableDto)
        {
            try
            {
                var result = await _tableBranchService.CreateTable(createTableDto);

                if (!result)
                {
                    return BadRequest(new { message = "Bàn đã tồn tại" });
                }

                return Ok(new { message = "Tạo bàn thành công" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi không mong muốn" });
            }
        }
        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}, {AppConstants.STAFF_ROLE_STRING}")]
        [HttpPut("tables-toggle/{tableId}")]
        public async Task<IActionResult> ToggleTableStatus(Guid tableId)
        {
            try
            {
                var result = await _tableBranchService.ToggleTableStatus(tableId);

                if (!result)
                    return BadRequest(new { message = "Cập nhật bàn thất bại." });

                return Ok(new { message = "Chuyển trạng thái bàn thành công!" });
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
                return StatusCode(500, new { message = "Đã xảy ra lỗi không mong muốn." });
            }
        }
        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpPut("delete-table/{id}")]
        public async Task<IActionResult> DeleteTable(Guid id)
        {
            try
            {
                var result = await _tableBranchService.DeleteBranch(id);

                if (!result)
                {
                    return NotFound(new { message = "Bàn không tồn tại hoặc đã bị vô hiệu hóa." });
                }

                return Ok(new { message = "Xóa bàn thành công!" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi máy chủ nội bộ: {ex.Message}" });
            }
        }
        //   [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}, {AppConstants.STAFF_ROLE_STRING}")]
        //   [HttpGet("get-all-tables")]
        //   public async Task<IActionResult> GetTables(
        //[FromQuery] int page = 0,
        //[FromQuery] int pageSize = 10,
        //[FromQuery] Guid? branchId = null)
        //   {
        //       try
        //       {
        //           var response = await _tableBranchService.GetTablesAsync(page, pageSize, branchId);
        //           return Ok(response);
        //       }
        //       catch (Exception ex)
        //       {
        //           return StatusCode(500, "Internal Server Error");
        //       }
        //   }
        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpGet("get-all-tables")]
        public async Task<IActionResult> GetAllTables(
        [FromQuery] Guid? branchId = null,
        [FromQuery] bool? isOccupied = null,
        [FromQuery] string? tableNumber = null)
        {
            try
            {
                var tables = await _tableBranchService.GetAllTablesAsync(branchId, isOccupied, tableNumber);

                if (tables == null || !tables.Any())
                {
                    return NotFound(new { message = "Không có bàn nào phù hợp." });
                }

                return Ok(tables);
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "Lỗi cơ sở dữ liệu." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }

        [HttpGet("get-all-table-not-deleted")]
        public async Task<IActionResult> GetTablesNotDeleted(
     [FromQuery] int page = 0,
     [FromQuery] int pageSize = 10,
     [FromQuery] Guid? branchId = null)
        {
            try
            {
                var response = await _tableBranchService.GetTablesNotDeleted(page, pageSize, branchId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi máy chủ nội bộ");
            }
        }


        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpPut("update-table/{tableId}")]
        public async Task<IActionResult> UpdateTable(Guid tableId, [FromBody] UpdateTableDto updateTableDto)
        {
            try
            {
                var result = await _tableBranchService.UpdateTable(tableId, updateTableDto);
                if (!result)
                {
                    return BadRequest(new { message = "Bàn đã tồn tại trong chi nhánh" });
                }
                return Ok(new { message = "Cập nhật bàn thành công" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi không mong muốn" });
            }
        }

        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpPut("redeactive-table/{id}")]
        public async Task<IActionResult> ToggleTableDeleted(Guid id)
        {
            try
            {
                var result = await _tableBranchService.ToggleTableDeleted(id);

                if (result == null)
                {
                    return NotFound(new { message = "Không tìm thấy bàn." });
                }

                var message = result.Value
                    ? "Vô hiệu hóa bàn thành công."
                    : "Đã tắt vô hiệu hóa bàn. Bàn đã được kích hoạt lại.";

                return Ok(new { message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi máy chủ nội bộ: {ex.Message}" });
            }
        }


        [HttpGet("get-branchInfo-by-id/{tableId}")]
        public async Task<IActionResult> GetBranchInfoById(Guid tableId)
        {
            try
            {
                var result = await _tableBranchService.GetBranchInfoById(tableId);
                if (result == null)
                {
                    return NotFound(new { message = "Không tìm thấy thông tin chi nhánh" });
                }

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi không mong muốn" });
            }
        }
    }
}

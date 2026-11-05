using Branch.API.Dtos.Branch;
using Branch.API.Services.Interfaces;
using Domain.Constanst;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Branch.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly IBranchService _branchService;

        public BranchController(IBranchService branchService)
        {
            _branchService = branchService;
        }
        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}, {AppConstants.STAFF_ROLE_STRING}")]
        [HttpGet("get-all-branch")]
        public async Task<IActionResult> GetAllBranch()
        {
            try
            {
                var branchs = await _branchService.GetAllBranch();

                if (branchs == null || !branchs.Any())
                {
                    return NotFound(new { message = "Không có chi nhánh nào có sẵn." });
                }

                return Ok(branchs);
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi cơ sở dữ liệu." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }

        [Authorize(Roles = AppConstants.ADMIN_ROLE_STRING)]
        [HttpPost("create-branch-sub")]
        public async Task<IActionResult> CreateBranch([FromBody] CreateBranchDto branchDto)
        {
            if (branchDto == null)
            {
                return BadRequest(new { message = "Dữ liệu chi nhánh không hợp lệ." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Dữ liệu chi nhánh không hợp lệ.",
                    details = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }


            try
            {

                var result = await _branchService.CreateBranch(branchDto);

                if (result)
                {
                    return CreatedAtAction(nameof(CreateBranch), new { message = "Tạo chi nhánh thành công." });
                }
                else
                {
                    return BadRequest(new { message = "Tạo chi nhánh thất bại." });
                }
            }
            catch (InvalidOperationException invOpEx)
            {
                return BadRequest(new { message = "Lỗi xác thực nghiệp vụ", details = invOpEx.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = "Lỗi tham số", details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }
        [Authorize(Roles = AppConstants.ADMIN_ROLE_STRING)]
        [HttpPut("update-branch/{id}")]
        public async Task<IActionResult> UpdateBranch(Guid id, [FromBody] UpdateBranchDto updateBranchDto)
        {
            try
            {
                if (updateBranchDto == null)
                {
                    return BadRequest("Dữ liệu chi nhánh không hợp lệ.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updated = await _branchService.UpdateBranch(id, updateBranchDto);

                if (!updated)
                {
                    return NotFound(new { message = "Không tìm thấy chi nhánh hoặc cập nhật thất bại." });
                }

                return Ok(new { message = "Cập nhật chi nhánh thành công!" });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, new { message = $"Lỗi hoạt động: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi máy chủ nội bộ: {ex.Message}" });
            }
        }

        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}, {AppConstants.STAFF_ROLE_STRING}")]
        [HttpGet("get-detail-branch/{id}")]
        public async Task<IActionResult> GetBranchById(Guid id)
        {
            try
            {
                var branch = await _branchService.GetBranchById(id);

                if (branch == null)
                {
                    return NotFound(new { message = "Không tìm thấy chi nhánh." });
                }

                return Ok(branch);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi máy chủ nội bộ: {ex.Message}" });
            }
        }

        [Authorize(Roles = AppConstants.ADMIN_ROLE_STRING)]
        [HttpPut("delete-branch/{id}")]
        public async Task<IActionResult> DeleteBranch(Guid id)
        {
            try
            {
                var result = await _branchService.DeleteBranch(id);

                if (!result)
                {
                    return NotFound(new { message = "Không tìm thấy chi nhánh hoặc chi nhánh đã không hoạt động." });
                }

                return Ok(new { message = "Xóa chi nhánh thành công!" });
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

        [HttpGet("get-all-branch-base")]
        public async Task<IActionResult> GetAllBranchBase()
        {
            try
            {
                var branchs = await _branchService.GetAllBranchBase();

                if (branchs == null || !branchs.Any())
                {
                    return NotFound(new { message = "Không có chi nhánh nào." });
                }

                return Ok(branchs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }


    }
}

using Domain.Constanst;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.API.Dtos.Profile;
using UserManagement.API.Dtos.Staff;
using UserManagement.API.Services.Interfaces;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffInformationsController : ControllerBase
    {
        private readonly IStaffInformationService _staffInformationService;
        private readonly ILogger<VouchersController> _logger;

        public StaffInformationsController(IStaffInformationService staffInformationService, ILogger<VouchersController> logger)
        {
            _staffInformationService = staffInformationService;
            _logger = logger;
        }

        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpGet("get-all-staff")]
        public async Task<IActionResult> GetAllStaff(
            [FromQuery] int page = 1,
            [FromQuery] int? pageSize = null,
            [FromQuery] string? search = null,
            [FromQuery] Guid? branchId = null)
        {
            try
            {
                var reponse = await _staffInformationService.GetAllStaffs(page, pageSize, search, branchId);
                return Ok(reponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ nội bộ: {ex.Message}");
            }
        }

        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpPut("reactive-staff/{id}")]
        public async Task<IActionResult> ReactiveStaff(Guid id)
        {
            try
            {
                var result = await _staffInformationService.DeleteStaff(id);

                if (!result)
                {
                    return NotFound(new { message = "Nhân viên không tồn tại hoặc đã bị vô hiệu hóa." });
                }

                return Ok(new { message = "Xóa nhân viên thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ nội bộ: {ex.Message}");
            }
        }

        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpPut("update-staff")]
        public async Task<IActionResult> UpdateStaff([FromBody] UpdateStaffDto updateStaffDto)
        {
            if (!ModelState.IsValid) return ValidationErrorResponse();

            try
            {
                var result = await _staffInformationService.UpdateStaff(updateStaffDto);

                if (!result)
                {
                    return NotFound(new { message = "Nhân viên không tồn tại hoặc cập nhật thất bại." });
                }

                return Ok(new { message = "Cập nhật nhân viên thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ nội bộ: {ex.Message}");
            }
        }

        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpPost("create-staff")]
        public async Task<IActionResult> CreateStaff([FromBody] CreateStaffDto createStaffDto)
        {
            if (!ModelState.IsValid) return ValidationErrorResponse();
            try
            {
                bool result = await _staffInformationService.CreateStaff(createStaffDto);
                if (!result) return Conflict(new { message = "Nhân viên với email đã tồn tại." });

                return Ok(new { message = "Tạo nhân viên thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ nội bộ: {ex.Message}");
            }
        }

        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpGet("get-detail-staff{id}")]
        public async Task<IActionResult> GetDetailStaff(Guid id)
        {
            try
            {
                var response = await _staffInformationService.GetDetailStaffAsync(id);
                if (response == null)
                {
                    return NotFound("Nhân viên này không tồn tại.");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi máy chủ nội bộ");
            }
        }

        [Authorize(Roles = $"{AppConstants.STAFF_ROLE_STRING},{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING},{AppConstants.USER_ROLE_STRING}")]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileDto updateProfileDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Dữ liệu không hợp lệ",
                    details = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            try
            {
                var result = await _staffInformationService.UpdateProfile(updateProfileDto);
                if (result == null)
                {
                    return NotFound(new { message = "Người dùng không tồn tại hoặc cập nhật thất bại." });
                }

                return Ok(new { message = "Cập nhật người dùng thành công!", result });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError("Tải lên Cloudinary thất bại: {Message}", ex.Message);
                return StatusCode(502, new { message = "Tải lên hình ảnh thất bại", details = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi trong UpdateStaff: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }

        [Authorize(Roles = $"{AppConstants.STAFF_ROLE_STRING},{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpGet("profile-staff/{userId}")]
        public async Task<IActionResult> GetStaffProfile(Guid userId)
        {
            try
            {
                var staffProfile = await _staffInformationService.GetStaffProfileAsync(userId);
                if (staffProfile == null)
                {
                    return NotFound(new { message = "Nhân viên không tồn tại." });
                }
                return Ok(staffProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }

        private IActionResult ValidationErrorResponse() => BadRequest(new
        {
            message = "Dữ liệu không hợp lệ",
            errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
        });
    }
}

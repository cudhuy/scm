using Domain.Constanst;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.API.Dtos;
using UserManagement.API.Services.Interfaces;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRolesController : ControllerBase
    {
        private readonly IUserRolesService _userRolesService;
        private readonly ILogger<UserRolesController> _logger;

        public UserRolesController(ILogger<UserRolesController> logger, IUserRolesService userRolesService)
        {
            _logger = logger;
            _userRolesService = userRolesService;
        }

        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpGet("get-all-user-roles")]
        public async Task<IActionResult> GetAllUserRoles()
        {
            try
            {
                var userRoles = await _userRolesService.GetAllUserRoles();
                if (userRoles == null || !userRoles.Any())
                {
                    return NotFound(new { message = "Không có vai trò người dùng nào." });
                }
                return Ok(userRoles);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in {MethodName}: {ErrorMessage}", nameof(GetAllUserRoles), ex.Message);
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }

        }

        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpPost("create-user-role")]
        public async Task<IActionResult> CreateUserRole([FromBody] CreateRoleDto userRoleDto)
        {
            if (userRoleDto == null || string.IsNullOrEmpty(userRoleDto.RoleName))
            {
                return BadRequest(new { message = "Dữ liệu vai trò người dùng không hợp lệ." });
            }

            bool isCreated = await _userRolesService.CreateUserRole(userRoleDto);

            if (isCreated)
            {
                return Ok(new { message = "Tạo vai trò người dùng thành công." });
            }
            else
            {
                return StatusCode(500, new { message = "Không thể tạo vai trò người dùng." });
            }
        }

        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpPut("delete-role-user/{id}")]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            try
            {
                var result = await _userRolesService.DeleteUserRole(id);

                if (!result)
                {
                    return NotFound(new { message = "Vai trò không tồn tại hoặc đã bị vô hiệu hóa!" });
                }

                return Ok(new { message = "Xóa vai trò thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ nội bộ: {ex.Message}");
            }
        }

        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpPut("update-role/{id}")]
        public async Task<IActionResult> UpdateRoleUser(Guid id, [FromBody] UpdateRoleDto updateRoleDto)
        {
            try
            {

                if (updateRoleDto == null)
                {
                    return BadRequest("Dữ liệu vai trò là null.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedRole = await _userRolesService.UpdateUserRole(id, updateRoleDto);

                if (updatedRole == null)
                {
                    return NotFound(new { message = "Vai trò không tồn tại hoặc cập nhật thất bại." });
                }

                return Ok(new { message = "Cập nhật vai trò thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ nội bộ: {ex.Message}");
            }
        }

        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpGet("get-role-user-count")]
        public async Task<IActionResult> GetRoleUserCount()
        {
            try
            {
                var roleUserCounts = await _userRolesService.GetRoleUserCount();
                if (roleUserCounts == null || roleUserCounts.Count == 0)
                {
                    return NotFound(new { message = "Không tìm thấy dữ liệu vai trò." });
                }

                return Ok(roleUserCounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting role user count.");
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }

    }
}

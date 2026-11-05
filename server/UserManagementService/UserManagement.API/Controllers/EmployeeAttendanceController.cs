using Domain.Constanst;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserManagement.API.Dtos.WorkShift;
using UserManagement.API.Services.Interfaces;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeAttendanceController : ControllerBase
    {
        private readonly IEmployeeAttendanceService _employeeAttendanceService;

        public EmployeeAttendanceController(IEmployeeAttendanceService employeeAttendanceService)
        {
            _employeeAttendanceService = employeeAttendanceService;
        }

        [Authorize(Roles = $"{AppConstants.STAFF_ROLE_STRING}")]
        [HttpPut("checkin")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInParameterDto input)
        {
            var userId = await _employeeAttendanceService.GetEmployeeIdByEmployeeShift(input.EmployeeShiftId);
            var currentUserId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return BadRequest("Ca làm việc không hợp lệ hoặc không tìm thấy nhân viên!");
            if (currentUserId == Guid.Empty || userId != currentUserId)
                return Forbid();

            var result = await _employeeAttendanceService.CheckIn(input, HttpContext);
            if (!result.Result) return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        [Authorize(Roles = $"{AppConstants.STAFF_ROLE_STRING}")]
        [HttpPut("checkout")]
        public async Task<IActionResult> CheckOut([FromBody] CheckOutParameterDto input)
        {
            var userId = await _employeeAttendanceService.GetEmployeeIdByEmployeeShift(input.EmployeeShiftId);
            var currentUserId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return BadRequest("Ca làm việc không hợp lệ hoặc không tìm thấy nhân viên!");
            if (currentUserId == Guid.Empty || userId != currentUserId)
                return Forbid();

            var result = await _employeeAttendanceService.CheckOut(input, HttpContext);
            if (!result.Result) return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        [Authorize(Roles = $"{AppConstants.STAFF_ROLE_STRING}")]
        [HttpGet("get-individual-attendance/{employeeId}")]
        public async Task<IActionResult> GetIndividualAttendance(Guid employeeId)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty || employeeId != currentUserId)
                return Forbid();
            var result = await _employeeAttendanceService.GetIndividualAttendance(employeeId);
            return Ok(result);
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userIdClaim != null ? Guid.Parse(userIdClaim) : Guid.Empty;
        }
    }
}

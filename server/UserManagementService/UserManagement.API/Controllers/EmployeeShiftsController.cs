using Domain.Constanst;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.API.Dtos.WorkShift;
using UserManagement.API.Services.Interfaces;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeShiftsController : ControllerBase
    {
        private readonly IEmployeeShiftsService _employeeShiftsService;

        public EmployeeShiftsController(IEmployeeShiftsService employeeShiftsService)
        {
            _employeeShiftsService = employeeShiftsService;
        }

        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpPost("add-emmployee-shift")]
        public async Task<IActionResult> AddEmployeeShift([FromBody] CreateEmployeeShiftDto input)
        {
            var result = await _employeeShiftsService.AddEmployeeShift(input);
            if (!result) return BadRequest("Thêm ca làm việc thất bại");
            return Ok("Thêm ca làm việc thành công");
        }

        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpGet("staffs/{branchId}")]
        public async Task<IActionResult> GetStaffs([FromRoute] Guid branchId)
        {
            var result = await _employeeShiftsService.GetStaffs(branchId);
            return Ok(result);
        }

        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpGet("staff-schedules-last-month/{branchId}")]
        public async Task<IActionResult> GetStaffSchedulesLastMonth(Guid branchId)
        {
            var result = await _employeeShiftsService.GetStaffSchedulesLastMonth(branchId);
            return Ok(result);
        }

        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpPost("staff-schedules-by-date")]
        public async Task<IActionResult> GetStaffScheduleByDate([FromBody] StaffScheduleParameter input)
        {
            var result = await _employeeShiftsService.GetStaffScheduleByDate(input);
            return Ok(result);
        }

        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpPut("delete-employeeshift/{id}")]
        public async Task<IActionResult> DeleteEmployeeShift(Guid id)
        {
            var result = await _employeeShiftsService.DeleteEmployeeShift(id);
            if (result) return Ok("Xóa thành công!");
            return BadRequest("Xóa thất bại!");
        }
    }
}

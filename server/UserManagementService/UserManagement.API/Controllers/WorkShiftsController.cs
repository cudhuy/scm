using Domain.Constanst;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.API.Dtos.WorkShift;
using UserManagement.API.Services.Interfaces;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkShiftsController : ControllerBase
    {
        private readonly IWorkShiftsService _workShiftsService;

        public WorkShiftsController(IWorkShiftsService workShiftsService)
        {
            _workShiftsService = workShiftsService;
        }

        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpPost("update-shift")]
        public async Task<IActionResult> UpdateShift([FromBody] UpdateWorkShiftDto workShift)
        {
            var result = await _workShiftsService.UpdateShift(workShift);
            if (!result) return BadRequest("Cập nhật ca làm việc thất bại.");
            return Ok("Cập nhật ca làm việc thành công!");
        }

        //[Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING},{AppConstants.STAFF_ROLE_STRING}")]
        [HttpGet("get-shifts")]
        public async Task<IActionResult> GetShifts()
        {
            var result = await _workShiftsService.GetShifts();
            return Ok(result);
        }

    }
}

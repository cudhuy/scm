using Domain.Constanst;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.API.Dtos.WorkShift;
using UserManagement.API.Services.Interfaces;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalaryController : ControllerBase
    {
        private readonly ISalaryService _salaryService;

        public SalaryController(ISalaryService salaryService)
        {
            _salaryService = salaryService;
        }

        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpPost("get-salary-list")]
        public async Task<IActionResult> GetSalaryList([FromBody] GetSalaryDto salaryDto)
        {
            var result = await _salaryService.GetSalaryList(salaryDto);
            return Ok(result);
        }

        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpPost("confirm-monthly-salary")]
        public async Task<IActionResult> ConfirmMonthlySalary([FromBody] List<CreateMonthlySalaryDto> request)
        {
            var result = await _salaryService.ConfirmMonthlySalary(request);
            if (result) return Ok("Xác nhận thành công");
            return BadRequest("Xác nhận thất bại");
        }

        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpPost("send-salary-slip")]
        public async Task<IActionResult> SendSalarySlip([FromBody] SendSalarySlipDto request)
        {
            var result = await _salaryService.SendSalarySlip(request);
            return Ok(result);
        }
    }

}


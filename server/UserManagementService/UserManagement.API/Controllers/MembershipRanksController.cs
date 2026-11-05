using Domain.Constanst;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using UserManagement.API.Dtos;
using UserManagement.API.Dtos.MembershipRank;
using UserManagement.API.Services.Interfaces;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembershipRanksController : ControllerBase
    {
        private readonly IMembershipRanksService _membershipRanksService;

        public MembershipRanksController(IMembershipRanksService membershipRanksService)
        {
            _membershipRanksService = membershipRanksService;
        }

        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}, {AppConstants.MANAGER_ROLE_STRING}, {AppConstants.STAFF_ROLE_STRING}")]
        [HttpGet]
        public async Task<IActionResult> GetMembershipRanks()
        {
            List<MemberShipRankDto> membershipRanks = await _membershipRanksService.GetMembershipRanks();
            return Ok(membershipRanks);
        }

        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpPost]
        public async Task<IActionResult> CreateMembershipRank([FromBody] SaveMembershipRankDto memberShipRankDto)
        {
            if (!ModelState.IsValid) return ValidationErrorResponse();

            bool result = await _membershipRanksService.CreateMembershipRank(memberShipRankDto);
            if (!result) return BadRequest("Tạo hạng thành viên thất bại");
            return Ok("Tạo thành công!");

        }

        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpDelete("soft-delete/{id}")]
        public async Task<IActionResult> SoftDeleteMembershipRank([FromRoute] Guid id)
        {
            bool result = await _membershipRanksService.DeleteMembershipRank(id);
            if (!result) return BadRequest("Xóa hạng thành viên thất bại");
            return Ok("Xóa thành công!");
        }

        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateMembershipRank([FromRoute] Guid id, [FromBody] SaveMembershipRankDto memberShipRankDto)
        {
            if (!ModelState.IsValid) return ValidationErrorResponse();
            bool result = await _membershipRanksService.UpdateMembershipRank(id, memberShipRankDto);
            if (!result) return BadRequest("Cập nhật hạng thành viên thất bại");
            return Ok("Cập nhật thành công!");
        }

        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpGet("get-report")]
        public async Task<IActionResult> GetReport()
        {
            MemberShipRankReportDto report = await _membershipRanksService.GetReport();
            return Ok(report);
        }

        private IActionResult ValidationErrorResponse() => BadRequest(new
        {
            message = "Dữ liệu không hợp lệ",
            errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
        });

    }
}

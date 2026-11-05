using Domain.Constanst;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.API.Dtos;
using UserManagement.API.Services.Interfaces;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VouchersController : ControllerBase
    {
        private readonly IVoucherService _voucherService;
        private readonly ILogger<VouchersController> _logger;
        public VouchersController(IVoucherService voucherService, ILogger<VouchersController> logger)
        {
            _voucherService = voucherService;
            _logger = logger;
        }
        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}, {AppConstants.STAFF_ROLE_STRING}")]
        [HttpGet("get-all-vouchers")]
        public async Task<IActionResult> GetAllVouchers()
        {

            try
            {
                var vouchers = await _voucherService.GetAllVouchers();

                if (vouchers == null || !vouchers.Any())
                {
                    return NotFound(new { message = "Không có voucher nào." });
                }

                return Ok(vouchers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong {MethodName}: {ErrorMessage}", nameof(GetAllVouchers), ex.Message);
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }
        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}, {AppConstants.STAFF_ROLE_STRING}")]
        [HttpPost("create-voucher")]
        public async Task<IActionResult> CreateVoucher([FromBody] CreateNewVoucherDto createNewVoucherDto)
        {
            try
            {
                if (createNewVoucherDto == null)
                {
                    return BadRequest("Dữ liệu voucher là null.");
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (createNewVoucherDto.MembershipRankIds == null || createNewVoucherDto.MembershipRankIds.Count == 0)
                {
                    return BadRequest("MembershipRankIds không thể null hoặc rỗng.");
                }
                if (createNewVoucherDto.StartDate == default || createNewVoucherDto.ExpireDate == default)
                {
                    return BadRequest("StartDate và ExpireDate phải hợp lệ.");
                }
                if (!Enum.IsDefined(typeof(VoucherEnum.EDiscountType), createNewVoucherDto.DiscountType))
                {
                    return BadRequest(new
                    {
                        message = "Giá trị DiscountType không hợp lệ. Các giá trị được phép: 1 (Phần trăm), 2 (Số tiền cố định)"
                    });
                }

                var isVoucherCreated = await _voucherService.CreateVoucher(createNewVoucherDto);

                if (!isVoucherCreated)
                {
                    return Conflict(new { message = $"Voucher với mã {createNewVoucherDto.Code} đã tồn tại." });
                }

                return Ok(new { message = "Thêm voucher thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi xảy ra khi tạo voucher: {ex.Message}");
                return StatusCode(500, $"Lỗi máy chủ nội bộ: {ex.Message}");
            }
        }
        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}, {AppConstants.STAFF_ROLE_STRING}")]
        [HttpPut("delete-voucher/{id}")]
        public async Task<IActionResult> DeleteVoucher(Guid id)
        {
            try
            {
                var result = await _voucherService.DeleteVoucher(id);

                if (!result)
                {
                    return NotFound(new { message = "Voucher không tìm thấy hoặc đã không còn hoạt động." });
                }

                return Ok(new { message = "Xóa voucher thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ nội bộ: {ex.Message}");
            }
        }
        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}, {AppConstants.STAFF_ROLE_STRING}")]
        [HttpPut("update-voucher/{id}")]
        public async Task<IActionResult> UpdateVoucher(Guid id, [FromBody] UpdateVoucherDto updateVoucherDto)
        {
            try
            {

                if (updateVoucherDto == null)
                {
                    return BadRequest("Dữ liệu voucher là null.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (updateVoucherDto.MembershipRankIds == null || updateVoucherDto.MembershipRankIds.Count == 0)
                {
                    return BadRequest("MembershipRankIds không thể null hoặc rỗng.");
                }

                if (updateVoucherDto.StartDate == default || updateVoucherDto.ExpireDate == default)
                {
                    return BadRequest("StartDate và ExpireDate phải hợp lệ.");
                }

                if (!Enum.IsDefined(typeof(VoucherEnum.EDiscountType), updateVoucherDto.DiscountType))
                {

                    return BadRequest(new
                    {
                        message = "Giá trị DiscountType không hợp lệ. Các giá trị được phép: 1 (Phần trăm), 2 (Số tiền cố định)"
                    });
                }

                var updatedVoucher = await _voucherService.UpdateVoucher(id, updateVoucherDto);

                if (updatedVoucher == null)
                {
                    return NotFound(new { message = "Voucher không tìm thấy hoặc cập nhật thất bại." });
                }

                return Ok(new { message = "Cập nhật thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ nội bộ: {ex.Message}");
            }
        }
        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}, {AppConstants.STAFF_ROLE_STRING}")]
        [HttpGet("get-all-voucherUsed")]
        public async Task<IActionResult> GetAllVoucherUseds()
        {
            try
            {
                var voucherUseds = await _voucherService.GetAllVoucherUseds();
                return Ok(voucherUseds);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ nội bộ: {ex.Message}");
            }
        }


        [Authorize(Roles = $"{AppConstants.USER_ROLE_STRING}")]
        [HttpGet("get-vouchers-by-user/{userId}")]
        public async Task<IActionResult> GetUserVouchers(Guid userId)
        {
            try
            {
                // Get vouchers used by the user
                var userVouchers = await _voucherService.GetUserVouchers(userId);

                if (userVouchers == null || !userVouchers.Any())
                {
                    return NotFound(new { message = "Không tìm thấy voucher nào cho người dùng này." });
                }

                return Ok(userVouchers);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong {MethodName}: {ErrorMessage}", nameof(GetUserVouchers), ex.Message);
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }
        //[Authorize(Roles = $"{AppConstants.USER_ROLE_STRING}")]
        //[HttpGet("get-all-vouchers-base")]
        //public async Task<IActionResult> GetAllVoucherBases()
        //{

        //    try
        //    {
        //        var vouchers = await _voucherService.GetAllVouchers();

        //        if (vouchers == null || !vouchers.Any())
        //        {
        //            return NotFound(new { message = "No voucher available." });
        //        }

        //        return Ok(vouchers);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred in {MethodName}: {ErrorMessage}", nameof(GetAllVouchers), ex.Message);
        //        return StatusCode(500, new { message = "Internal server error", details = ex.Message });
        //    }
        //}



    }
}

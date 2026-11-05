using AutoMapper;
using Domain.Constanst;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.API.Dtos.Customer;
using UserManagement.API.Services.Interfaces;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;

        public CustomersController(ICustomerService customerService, IMapper mapper)
        {
            _customerService = customerService;
            _mapper = mapper;
        }


        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}, {AppConstants.STAFF_ROLE_STRING}")]
        [HttpGet("get-all-customers")]
        public async Task<IActionResult> GetAllCustomers(
            [FromQuery] int page = 1,
            [FromQuery] int? pageSize = null,
            [FromQuery] string? search = null,
            [FromQuery] Guid? rankId = null)
        {
            try
            {
                var response = await _customerService.GetAllCustomers(page, pageSize, search, rankId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ nội bộ: {ex.Message}");
            }
        }

        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}, {AppConstants.STAFF_ROLE_STRING}")]
        [HttpGet("get-customer/{id}")]
        public async Task<IActionResult> GetCustomerById(Guid id)
        {
            try
            {
                var customer = await _customerService.GetCustomerById(id);
                if (customer == null)
                {
                    return NotFound();
                }
                CustomerDto customerDto = _mapper.Map<CustomerDto>(customer);
                customerDto.CreatedAt = customer.CreatedAt.ToString("dd-MM-yyyy");
                customerDto.Dob = customer.Dob?.ToString("dd-MM-yyyy");
                return Ok(customerDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ nội bộ: {ex.Message}");
            }
        }

        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpPut("update-customer")]
        public async Task<IActionResult> UpdateCustomer(CustomerUpdateDto customerDto)
        {
            if (!ModelState.IsValid) return ValidationErrorResponse();
            try
            {
                bool result = await _customerService.UpdateCustomer(customerDto);
                if (!result)
                {
                    return BadRequest(new { message = "Cập nhật thất bại." });
                }
                return Ok((new { message = "Cập nhật thành công." }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ nội bộ: {ex.Message}");
            }
        }

        [HttpGet("get-profile-customer/{id}")]
        public async Task<IActionResult> GetProfileCustomerById(Guid id)
        {
            try
            {
                var customer = await _customerService.GetCustomerById(id);
                if (customer == null)
                {
                    return NotFound();
                }
                CustomerDto customerDto = _mapper.Map<CustomerDto>(customer);
                customerDto.CreatedAt = customer.CreatedAt.ToString("dd-MM-yyyy");
                customerDto.Dob = customer.Dob?.ToString("dd-MM-yyyy");
                return Ok(customerDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ nội bộ: {ex.Message}");
            }
        }
        [Authorize(Roles = $"{AppConstants.USER_ROLE_STRING}")]
        [HttpGet("profile-new-user/{userId}")]
        public async Task<IActionResult> GetUserProfile(Guid userId)
        {
            try
            {
                var userProfile = await _customerService.GetUserProfileAsync(userId);
                if (userProfile == null)
                {
                    return NotFound(new { message = "Không tìm thấy người dùng." });
                }
                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }
        private IActionResult ValidationErrorResponse() => BadRequest(new
        {
            message = "Dữ liệu đầu vào không hợp lệ",
            errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
        });

    }
}

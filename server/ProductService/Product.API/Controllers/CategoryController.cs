using Domain.Constanst;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.API.Dtos.Category;
using Product.API.Services.Interfaces;

namespace Product.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        
        
        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING},{AppConstants.MANAGER_ROLE_STRING},{AppConstants.STAFF_ROLE_STRING}")]
        [HttpGet("get-all-categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategories();

                if (categories == null || !categories.Any())
                {
                    return NotFound(new { message = "Không có danh mục nào." });
                }

                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }
        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING},{AppConstants.MANAGER_ROLE_STRING},{AppConstants.STAFF_ROLE_STRING}")]
        [HttpGet("get-all-categories-admin")]
        public async Task<IActionResult> GetAllCategoryAdmin()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoryUser();

                if (categories == null || !categories.Any())
                {
                    return NotFound(new { message = "Không có danh mục nào." });
                }

                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }


        [HttpGet("get-all-categories-user")]
        public async Task<IActionResult> GetAllCategoryUser()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoryUser();

                if (categories == null || !categories.Any())
                {
                    return NotFound(new { message = "Không có danh mục nào." });
                }

                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }

        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpPost("create-a-category")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            if (createCategoryDto == null)
            {
                return BadRequest(new { message = "Dữ liệu danh mục không được để trống." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool isCreated = await _categoryService.CreateCategory(createCategoryDto);

            if (isCreated)
            {
                return Ok(new { message = "Tạo danh mục thành công." });
            }
            else
            {
                return StatusCode(500, new { message = "Tạo danh mục thất bại." });
            }
        }

        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpPut("update-a-category/{id}")]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryDto updateCategoryDto)
        {
            try
            {

                if (updateCategoryDto == null)
                {
                    return BadRequest("Dữ liệu danh mục không được để trống.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedCategory = await _categoryService.UpdateCategory(id, updateCategoryDto);

                if (updatedCategory == null)
                {
                    return NotFound(new { message = "Không tìm thấy danh mục hoặc cập nhật thất bại." });
                }

                return Ok(new { message = "Cập nhật danh mục thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ nội bộ: {ex.Message}");
            }
        }

        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpPut("delete-category/{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            try
            {
                var result = await _categoryService.DeleteCategory(id);

                if (!result)
                {
                    return NotFound(new { message = "Không tìm thấy danh mục hoặc đã bị xóa." });
                }

                return Ok(new { message = "Xóa danh mục thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ nội bộ: {ex.Message}");
            }
        }

        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpPut("redeactive-category/{id}")]
        public async Task<IActionResult> ToggleCategoryStatus(Guid id)
        {
            try
            {
                var result = await _categoryService.ToggleCategoryStatus(id);

                if (result == null)
                {
                    return NotFound(new { message = "Không tìm thấy danh mục." });
                }

                var message = result.Value
       ? "Vô hiệu hóa danh mục sản phẩm thành công. Tất cả sản phẩm thuộc danh mục này cũng đã bị vô hiệu hóa."
       : "Kích hoạt lại danh mục sản phẩm thành công.";


                return Ok(new { message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ nội bộ: {ex.Message}");
            }
        }

    }
}

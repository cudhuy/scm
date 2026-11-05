using Domain.Constanst;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.API.Dtos.Ingredient;
using Product.API.Dtos.InventoryItem;
using Product.API.Services.Interfaces;
using System.Security.Claims;

namespace Product.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}, {AppConstants.MANAGER_ROLE_STRING}, {AppConstants.STAFF_ROLE_STRING}")]
        [HttpPost("restock-inventory")]
        public async Task<IActionResult> RestockInventory([FromBody] RestockInventoryDto restockInventoryDto)
        {
            if (!ModelState.IsValid) return ValidationErrorResponse();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out Guid userId)) return Unauthorized(new { message = "Không có quyền truy cập" });

            bool isRestocked = await _inventoryService.RestockInventory(restockInventoryDto, userId);
            if (!isRestocked) return BadRequest(new { message = "Nguyên liệu đã tồn tại." });

            return Ok(new { message = "Nhập kho thành công." });

        }


        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}, {AppConstants.MANAGER_ROLE_STRING}, {AppConstants.STAFF_ROLE_STRING}")]
        [HttpGet("get-all-inventory-items")]
        public async Task<IActionResult> GetAllIngredients(
            [FromQuery] int page = 1,
            [FromQuery] int? pageSize = 5,
            [FromQuery] string? search = null,
            [FromQuery] Guid? branchId = null)
        {
            try
            {
                branchId ??= AppConstants.BRANCH_MAIN_GUID;
                var response = await _inventoryService.GetAllIngredients(page, pageSize, search, branchId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }


        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}, {AppConstants.MANAGER_ROLE_STRING}, {AppConstants.STAFF_ROLE_STRING}")]
        [HttpGet("get-ingredient-detail/{branchId}/{ingredientId}")]
        public async Task<IActionResult> GetIngredientDetail(Guid branchId, Guid ingredientId)
        {
            IngredientDetailDto ingredient = await _inventoryService.GetIngredientDetail(branchId, ingredientId);
            if (ingredient == null) return StatusCode(404, new { message = "Kho không có nguyên liệu." });
            return Ok(ingredient);
        }


        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}, {AppConstants.MANAGER_ROLE_STRING}, {AppConstants.STAFF_ROLE_STRING}")]
        [HttpGet("get-low-stock-ingredients")]
        public async Task<IActionResult> GetLowStockIngredients(
            [FromQuery] int page = 1,
            [FromQuery] int? pageSize = 5,
            [FromQuery] string? search = null,
            [FromQuery] Guid? branchId = null)
        {
            try
            {
                branchId ??= AppConstants.BRANCH_MAIN_GUID;
                var response = await _inventoryService.GetLowStockIngredients(page, pageSize, search, branchId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }


        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}, {AppConstants.MANAGER_ROLE_STRING}, {AppConstants.STAFF_ROLE_STRING}")]
        [HttpGet("get-inventory-statistics")]
        public async Task<IActionResult> GetInventoryStatistics()
        {
            try
            {
                InventoryStatisticsDto ingredient = await _inventoryService.GetInventoryStatistics();
                if (ingredient == null) return StatusCode(404, new { message = "Kho không có thống kê." });
                return Ok(ingredient);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }

        }

        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}, {AppConstants.MANAGER_ROLE_STRING}, {AppConstants.STAFF_ROLE_STRING}")]
        [HttpGet("get-all-inventory-receipt")]
        public async Task<IActionResult> GetAllInventoryReceipts(
            [FromQuery] int page = 1,
            [FromQuery] int? pageSize = 5,
            [FromQuery] string? search = null,
            [FromQuery] Guid? branchId = null)
        {
            try
            {
                branchId ??= AppConstants.BRANCH_MAIN_GUID;
                var response = await _inventoryService.GetAllInventoryReceipts(page, pageSize, search, branchId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        private IActionResult ValidationErrorResponse() => BadRequest(new
        {
            message = "Dữ liệu không hợp lệ",
            errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
        });

    }
}

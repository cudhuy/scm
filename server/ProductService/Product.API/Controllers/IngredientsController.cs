using Microsoft.AspNetCore.Mvc;
using Product.API.Dtos.Ingredient;
using Product.API.Services.Interfaces;

namespace Product.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientsService _ingredientsService;

        public IngredientsController(IIngredientsService ingredientsService)
        {
            _ingredientsService = ingredientsService;
        }

        [HttpPost("create-a-ingredient")]
        public async Task<IActionResult> CreateIngredient([FromBody] CreateIngredientDto createIngredientDto)
        {
            if (createIngredientDto == null)
            {
                return BadRequest(new { message = "Dữ liệu nguyên liệu không được để trống." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool isCreated = await _ingredientsService.CreateIngredient(createIngredientDto);

            if (isCreated)
            {
                return Ok(new { message = "Tạo nguyên liệu thành công." });
            }
            else
            {
                return StatusCode(500, new { message = "Tạo nguyên liệu thất bại." });
            }
        }

        [HttpGet("get-all-ingredient")]
        public async Task<IActionResult> GetAllIngredient()
        {
            try
            {
                var ingredients = await _ingredientsService.GetAllIngredient();

                if (ingredients == null || !ingredients.Any())
                {
                    return NotFound(new { message = "Không có nguyên liệu nào." });
                }

                return Ok(ingredients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }
    }
}

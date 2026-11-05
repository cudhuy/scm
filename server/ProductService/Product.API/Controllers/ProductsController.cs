using Domain.Constanst;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Product.API.Dtos.Product;
using Product.API.Services.Interfaces;

namespace Product.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }
        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING},{AppConstants.STAFF_ROLE_STRING}")]
        [HttpGet("get-all-product")]
        public async Task<IActionResult> GetProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] Guid? category = null)
        {
            try
            {
                var response = await _productService.GetProductsAsync(page, pageSize, search, category);
                if (response == null || !response.Items.Any())
                {
                    return NotFound(new { message = "Không tìm thấy sản phẩm nào." });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING},{AppConstants.MANAGER_ROLE_STRING},{AppConstants.STAFF_ROLE_STRING}")]
        [HttpGet("get-detail-product/{id}")]
        public async Task<IActionResult> GetDetailProduct(Guid id)
        {
            try
            {
                var response = await _productService.GetDetailProductsAsync(id);
                if (response == null)
                {
                    return NotFound("Không tìm thấy sản phẩm");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi máy chủ nội bộ");
            }
        }
        [HttpGet("get-product-ingredients/{id}")]
        public async Task<IActionResult> GetProductIngredients(Guid id)
        {
            try
            {
                var ingredients = await _productService.GetProductIngredientsAsync(id);
                if (ingredients == null || !ingredients.Any())
                {
                    return NotFound("Không tìm thấy nguyên liệu nào cho sản phẩm này.");
                }
                return Ok(ingredients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi máy chủ nội bộ");
            }
        }

        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpPut("delete-product/{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            try
            {
                var result = await _productService.DeleteProduct(id);

                if (!result)
                {
                    return NotFound(new { message = "Không tìm thấy sản phẩm" });
                }

                return Ok(new { message = "Xóa sản phẩm thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ nội bộ: {ex.Message}");
            }
        }
        
        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpPut("redeactive-product/{id}")]
        public async Task<IActionResult> ToggleProductStatus(Guid id)
        {
            try
            {
                var result = await _productService.ToggleProductStatus(id);

                if (result == null)
                {
                    return NotFound(new { message = "Không tìm thấy sản phẩm" });
                }
                var message = result.Value
           ? "Vô hiệu hóa sản phẩm thành công."
           : "Đã tắt vô hiệu hóa sản phẩm. Sản phẩm đã được kích hoạt lại.";
                return Ok(new { message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ nội bộ: {ex.Message}");
            }
        }


        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpPost("create-product")]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto productDto)
        {
            if (productDto == null)
            {
                return BadRequest(new { message = "Dữ liệu sản phẩm không hợp lệ." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Dữ liệu sản phẩm không hợp lệ.",
                    details = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            List<ProductIngredientDTO> ingredients;
            try
            {
                ingredients = string.IsNullOrWhiteSpace(productDto.Ingredients)
                    ? new List<ProductIngredientDTO>()
                    : JsonConvert.DeserializeObject<List<ProductIngredientDTO>>(productDto.Ingredients);

                if (ingredients == null || ingredients.Count == 0)
                {
                    return BadRequest(new { message = "Danh sách nguyên liệu là bắt buộc và không được để trống." });
                }
            }
            catch (JsonException)
            {
                return BadRequest(new { message = "Định dạng JSON không hợp lệ trong trường nguyên liệu." });
            }

            try
            {
                productDto.Ingredients = JsonConvert.SerializeObject(ingredients); // Convert lại để dùng sau

                var result = await _productService.CreateProduct(productDto, ingredients);
                if (!result)
                {
                    return BadRequest(new { message = "Tạo sản phẩm thất bại." });
                }

                return CreatedAtAction(nameof(GetProducts), new { message = "Tạo sản phẩm thành công." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = "Lỗi thành phần", details = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }


        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpPut("update-product/{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromForm] UpdateProductDto productDto)
        {
            if (productDto == null)
            {
                return BadRequest(new { message = "Dữ liệu sản phẩm không hợp lệ." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Dữ liệu sản phẩm không hợp lệ.",
                    details = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            List<ProductIngredientDTO> ingredients;
            try
            {
                ingredients = string.IsNullOrWhiteSpace(productDto.Ingredients)
                    ? new List<ProductIngredientDTO>()
                    : JsonConvert.DeserializeObject<List<ProductIngredientDTO>>(productDto.Ingredients);

                if (ingredients == null || ingredients.Count == 0)
                {
                    return BadRequest(new { message = "Danh sách nguyên liệu là bắt buộc và không được để trống." });
                }
            }
            catch (JsonException)
            {
                return BadRequest(new { message = "Định dạng JSON không hợp lệ trong trường Thành phần." });
            }

            try
            {
                productDto.Ingredients = JsonConvert.SerializeObject(ingredients);

                var result = await _productService.UpdateProduct(id, productDto, ingredients);

                if (result)
                {
                    return Ok(new { message = "Cập nhật sản phẩm thành công." });
                }
                else
                {
                    return BadRequest(new { message = "Cập nhật sản phẩm thất bại." });
                }
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = "Lỗi thành phần", details = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }



        [HttpGet("get-list-product-base")]
        public async Task<IActionResult> GetListProductBase(
    [FromQuery] int page = 1,
    [FromQuery] int? pageSize = null,
    [FromQuery] string? search = null,
    [FromQuery] int sortOrder = 0,
     [FromQuery] Guid? userId = null)
        {
            try
            {
                if (sortOrder != 0 && sortOrder != 1 && sortOrder != 2)
                {
                    return BadRequest(new { message = "Thứ tự sắp xếp không hợp lệ. Các giá trị chấp nhận: 0 (mặc định), 1 (giá tăng dần), 2 (giá giảm dần)." });
                }
                var response = await _productService.GetListProductBase(page, pageSize, search, userId, sortOrder);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }

        [HttpGet("get-product-by-id")]
        public async Task<IActionResult> GetProductById(Guid productID)
        {
            try
            {
                var product = await _productService.GetProductById(productID);
                if (product == null)
                {
                    return NotFound(new { message = "Không tìm thấy sản phẩm." });
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }


        [HttpGet("get-similar-products/{productID}")]
        public async Task<IActionResult> GetSimilarProducts(Guid productID)
        {
            try
            {
                var similarProducts = await _productService.GetSimilarProducts(productID);
                if (similarProducts == null || !similarProducts.Any())
                {
                    return NotFound(new { message = "Không tìm thấy sản phẩm tương tự." });
                }
                return Ok(similarProducts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }

        [HttpGet("get-products-by-category-base/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategoryId(Guid categoryId)
        {
            try
            {
                var products = await _productService.GetProductsByCategoryId(categoryId);
                if (products == null || !products.Any())
                {
                    return NotFound(new { message = "Không tìm thấy sản phẩm nào cho danh mục này." });
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }
        [HttpGet("get-new-products")]
        public async Task<IActionResult> GetLatestProducts()
        {
            try
            {
                var products = await _productService.GetLatestProducts();
                if (products == null || !products.Any())
                {
                    return NotFound(new { message = "Không tìm thấy sản phẩm mới nào." });
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }

        [HttpGet("get-top-selling-products")]
        public async Task<IActionResult> GetTopSellingProducts()
        {
            try
            {
                var products = await _productService.GetTopSellingProducts();
                if (products == null || !products.Any())
                {
                    return NotFound(new { message = "Không tìm thấy sản phẩm bán chạy nào." });
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }
        [Authorize(Roles = $"{AppConstants.USER_ROLE_STRING}")]

        [HttpPost("add-to-wishlist")]
        public async Task<IActionResult> AddToWishlist([FromBody] AddWishListDto request)
        {
            try
            {
                var result = await _productService.AddToWishlist(request.UserId, request.ProductId);
                if (!result)
                {
                    return BadRequest(new { message = "Sản phẩm đã có trong danh sách yêu thích." });
                }
                return Ok(new { message = "Thêm sản phẩm vào danh sách yêu thích thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }
        [Authorize(Roles = $"{AppConstants.USER_ROLE_STRING}")]
        [HttpGet("get-wishlist-by-user/{userId}")]
        public async Task<IActionResult> GetWishlistByCustomer(Guid userId, [FromQuery] int page = 1, [FromQuery] int? pageSize = null, [FromQuery] string? search = null)
        {
            try
            {
                var response = await _productService.GetWishlistByUserIdAsync(userId, page, pageSize, search);
                if (response.TotalItems == 0)
                    return NotFound(new { message = "Không tìm thấy mục nào trong danh sách mong muốn" });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }

        [Authorize(Roles = $"{AppConstants.USER_ROLE_STRING}")]
        [HttpDelete("delete-product-in-wishlist/{userId}/{productId}")]
        public async Task<IActionResult> DeleteWishlistItem(Guid userId, Guid productId)
        {
            try
            {
                var result = await _productService.DeleteWishlistItemAsync(userId, productId);
                if (!result)
                {
                    return NotFound("Không tìm thấy sản phẩm trong danh sách yêu thích hoặc đã bị ẩn.");
                }

                return Ok(new { message = "Xóa sản phẩm thành công khỏi danh sách yêu thích." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi máy chủ nội bộ.");
            }
        }
    }
}

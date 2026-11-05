using Microsoft.AspNetCore.Mvc;
using Product.API.Dtos.Cloud;
using Product.API.Helpers;

namespace Product.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadFileController : ControllerBase
    {
        private readonly CloudinaryHelper _cloudinaryHelper;

        public UploadFileController(CloudinaryHelper cloudinaryHelper)
        {
            _cloudinaryHelper = cloudinaryHelper;
        }


        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(List<IFormFile> files)
        {
            if (files == null || files.Count == 0) return BadRequest("No file uploaded.");

            try
            {
                CloudResponseDto actionResult = await _cloudinaryHelper.UploadImage(files);
                if (!actionResult.IsSuccess)
                {
                    return BadRequest(actionResult);
                }
                return Ok(actionResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading image: {ex.Message}");
            }
        }
    }
}

using AutoMapper;
using Branch.API.Dtos.Layout;
using Branch.API.Helpers;
using Branch.API.Services.Interfaces;
using Domain.Constanst;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Branch.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LayoutController : ControllerBase
    {
        private readonly ILayoutService _layoutService;
        private readonly CloudinaryHelper _cloudinaryHelper;
        private readonly IMapper _mapper;

        public LayoutController(ILayoutService layoutService, CloudinaryHelper cloudinaryHelper, IMapper mapper)
        {
            _layoutService = layoutService;
            _cloudinaryHelper = cloudinaryHelper;
            _mapper = mapper;
        }


        [HttpGet("banners")]
        public async Task<IActionResult> GetBanners()
        {
            List<PromotionalBanner> banners = await _layoutService.GetBannersAsync();
            if (banners == null)
            {
                return NotFound("Không tìm thấy banner nào.");
            }
            var bannerDto = _mapper.Map<List<BannerDto>>(banners);
            return Ok(bannerDto);
        }

        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpPut("update-banners")]
        public async Task<IActionResult> UpdateBanner([FromForm] UpdateBannerFormDto bannerDto)
        {
            string imageUrl = null;
            if (bannerDto.ImageUrl != null)
            {
                var uploadResult = await _cloudinaryHelper.UploadImage(new List<IFormFile> { bannerDto.ImageUrl });
                if (!uploadResult.IsSuccess)
                {
                    return BadRequest(uploadResult);
                }
                imageUrl = uploadResult.Files[0].Url;
            }

            bool result = await _layoutService.UpdateBannerAsync(imageUrl, bannerDto);
            if (result) return Ok(new { message = "Cập nhật thành công" });
            else return NotFound("Không tìm thấy banner nào.");
        }
    }
}

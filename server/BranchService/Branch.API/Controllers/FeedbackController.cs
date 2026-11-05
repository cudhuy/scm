using Branch.API.Dtos.Feedback;
using Branch.API.Services.Interfaces;
using Domain.Constanst;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Branch.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }
        [Authorize(Roles = $"{AppConstants.USER_ROLE_STRING}")]
        [HttpPost("create-feedback-onl")]
        public async Task<IActionResult> CreateFeedback([FromBody] CreateFeedbackDto feedbackDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (feedbackDto.UserId == null || feedbackDto.OrderId == null || feedbackDto.Rating == null || feedbackDto.Content == null)
            {
                return BadRequest(new { message = "UserId, OrderId, Rating hoặc Content là bắt buộc." });
            }

            var result = await _feedbackService.CreateFeedbackAsync(feedbackDto);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = "Tạo phản hồi thành công!" });
        }
        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpGet("get-all-feedback")]
        public async Task<IActionResult> GetAllFeedbacks(
     [FromQuery] int page = 1,
     [FromQuery] int pageSize = 10,
     [FromQuery] Guid? branchId = null)
        {
            try
            {
                var response = await _feedbackService.GetAllFeedbacksAsync(page, pageSize, branchId);
                if (response == null || !response.Items.Any())
                {
                    return NotFound(new { message = "Không tìm thấy phản hồi nào." });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}


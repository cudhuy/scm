using Domain.Constanst;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using UserManagement.API.Dtos.Notification;
using UserManagement.API.Helpers;
using UserManagement.API.Services.Interfaces;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationsService _notificationsService;
        private readonly ILogger<NotificationsController> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationsController(ILogger<NotificationsController> logger, INotificationsService notificationsService, IHubContext<NotificationHub> hubContext)
        {
            _logger = logger;
            _notificationsService = notificationsService;
            _hubContext = hubContext;

        }
        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}, {AppConstants.STAFF_ROLE_STRING}")]
        [HttpGet("get-notification-ranks")]
        public async Task<IActionResult> GetAllNotificationRanksByPage(
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? rankIds = null)
        {
            try
            {
                List<Guid> rankIdList = string.IsNullOrWhiteSpace(rankIds)
                    ? new List<Guid>()
                    : rankIds.Split(',').Select(Guid.Parse).ToList();

                var response = await _notificationsService.GetAllNotificationRanksForPage(page, pageSize, search, rankIdList);

                if (response == null || !response.Items.Any())
                {
                    return NotFound(new { message = "Không có thông báo nào." });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi trong {nameof(GetAllNotificationRanksByPage)}: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }

        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}, {AppConstants.STAFF_ROLE_STRING}")]
        [HttpPut("delete-notification-system/{id}")]
        public async Task<IActionResult> DeleteNotificationSystem(Guid id)
        {
            try
            {
                var result = await _notificationsService.DeleteNotificationSystem(id);

                if (!result)
                {
                    return NotFound(new { message = "Thông báo không tồn tại hoặc đã bị vô hiệu hóa." });
                }

                return Ok(new { message = "Xóa hệ thống thông báo thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ nội bộ: {ex.Message}");
            }
        }

        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}, {AppConstants.STAFF_ROLE_STRING}")]
        [HttpPut("update-notification-system/{id}")]
        public async Task<IActionResult> UpdateNotificationSystem(Guid id, [FromBody] UpdateNotificationSystemDto updateNotificationSystemDto)
        {
            try
            {
                if (updateNotificationSystemDto == null)
                {
                    return BadRequest("Dữ liệu thông báo không hợp lệ.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedNotification = await _notificationsService.UpdateNotificationSystem(id, updateNotificationSystemDto);

                if (!updatedNotification)
                {
                    return NotFound(new { message = "Thông báo không tồn tại hoặc cập nhật thất bại." });
                }

                return Ok(new { message = "Cập nhật nội dung thông báo thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ nội bộ: {ex.Message}");
            }
        }

        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}, {AppConstants.STAFF_ROLE_STRING}")]
        [HttpPost("create-notification-system")]
        public async Task<IActionResult> CreateNotificationSystem([FromBody] CreateNotificationSystemDto createNotificationSystemDto)
        {
            if (createNotificationSystemDto == null || string.IsNullOrEmpty(createNotificationSystemDto.Title))
            {
                return BadRequest(new { message = "Dữ liệu thông báo không hợp lệ." });
            }
            if (createNotificationSystemDto.Priority < 0 || createNotificationSystemDto.Priority > 2)
            {
                return BadRequest(new { message = "Độ ưu tiên phải là 0, 1 hoặc 2." });
            }

            bool isCreated = await _notificationsService.CreateNotificationSystem(createNotificationSystemDto);

            if (isCreated)
            {
                try
                {
                    await _hubContext.Clients.All.SendAsync("ReceiveNotification", $"Thông báo mới: {createNotificationSystemDto.Title} - {createNotificationSystemDto.Content}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Lỗi khi gửi thông báo qua SignalR: {ex.Message}");
                    return StatusCode(500, new { message = "Gửi thông báo qua SignalR thất bại." });
                }
                return Ok(new { message = "Thêm thông báo thành công." });
            }
            else
            {
                return StatusCode(500, new { message = "Tạo thông báo thất bại." });
            }
        }

        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}, {AppConstants.STAFF_ROLE_STRING}")]
        [HttpPost("create-notification-for-ranks")]
        public async Task<IActionResult> CreateNotificationRanks([FromBody] CreateNotificationRanksDtos createNotificationRanksDtos)
        {
            if (createNotificationRanksDtos == null || string.IsNullOrEmpty(createNotificationRanksDtos.Title) || string.IsNullOrEmpty(createNotificationRanksDtos.Content))
            {
                return BadRequest(new { message = "Dữ liệu thông báo không hợp lệ." });
            }

            bool isCreated = await _notificationsService.CreateNotificationForRanks(createNotificationRanksDtos);

            if (isCreated)
            {
                return Ok(new { message = "Thêm thông báo người dùng thành công và gửi email cho người dùng có hạng này!" });
            }
            else
            {
                return StatusCode(500, new { message = "Tạo thông báo người dùng thất bại, hạng không tồn tại." });
            }
        }

        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}, {AppConstants.STAFF_ROLE_STRING}")]
        [HttpGet("get-notifications-system")]
        public async Task<IActionResult> GetAllNotificationSystemByPage(
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            try
            {
                var response = await _notificationsService.GetAllNotificationSystemForPage(page, pageSize, search);

                if (response.Items.Count == 0)
                {
                    return NotFound(new { message = "Không tìm thấy thông báo nào." });
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi trong {nameof(GetAllNotificationSystemByPage)}: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }

        [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}, {AppConstants.STAFF_ROLE_STRING}")]
        [HttpGet("count-all-notifications")]
        public async Task<IActionResult> GetNotificationCounts()
        {
            try
            {
                var response = await _notificationsService.GetNotificationCounts();

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi trong {nameof(GetNotificationCounts)}: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }

        [HttpGet("get-notifications-system-base")]
        public async Task<IActionResult> GetNotificationSystemBase()
        {
            try
            {
                var response = await _notificationsService.GetNotificationSystemBase();

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi trong {nameof(GetNotificationCounts)}: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", details = ex.Message });
            }
        }
    }
}

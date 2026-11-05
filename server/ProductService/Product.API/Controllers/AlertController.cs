using Domain.Constanst;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.API.Services.Interfaces;

namespace Product.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlertController : ControllerBase
    {
        private readonly IAlertService _alertService;

        public AlertController(IAlertService alertService)
        {
            _alertService = alertService;
        }

        [Authorize(Roles = $"{AppConstants.ADMIN_ROLE_STRING}")]
        [HttpPost("send-alerts")]
        public async Task<IActionResult> SendAlerts()
        {
            await _alertService.CheckAndSendAlerts();
            return Ok(new { message = "📩 Email sent successfully to {managerEmail}." });
        }
    }
}

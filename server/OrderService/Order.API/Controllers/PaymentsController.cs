using Domain.Constanst;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order.API.Dtos.Sepay;
using Order.API.Services.Interfaces;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentsService _paymentsService;
    private readonly IConfiguration _configuration;

    public PaymentsController(IPaymentsService paymentsService, IConfiguration configuration)
    {
        _paymentsService = paymentsService;
        _configuration = configuration;
    }

    [Authorize(Roles = $"{AppConstants.MANAGER_ROLE_STRING},{AppConstants.ADMIN_ROLE_STRING}, {AppConstants.STAFF_ROLE_STRING},{AppConstants.USER_ROLE_STRING}")]
    [HttpGet("generate-qr/{orderId}")]
    public async Task<IActionResult> GenerateQR(Guid orderId)
    {
        var result = await _paymentsService.GenerateQR(orderId);
        if (result != null)
        {
            return File(result, "image/png");
        }
        return BadRequest("Không thể tạo mã QR");
    }

    [HttpPost("webhook/sepay")]
    public async Task<IActionResult> ReceiveSepayWebhook([FromBody] SepayWebhookRequestDto request)
    {
        if (!Request.Headers.TryGetValue("Authorization", out var apiKey) || string.IsNullOrEmpty(apiKey))
        {
            return Unauthorized("Thiếu API Key");
        }

        string extractedApiKey = apiKey.ToString().Replace("Apikey ", "", StringComparison.OrdinalIgnoreCase);

        if (_configuration["SepayConfig:ApiKey"] != extractedApiKey)
        {
            return Unauthorized("API Key không hợp lệ");
        }

        await _paymentsService.CheckPayment(request);
        return Ok(new { success = true });
    }

}

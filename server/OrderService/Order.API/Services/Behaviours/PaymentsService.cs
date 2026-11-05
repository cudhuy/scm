using AutoMapper;
using Order.API.Repositories.Interfaces;
using Order.API.Services.Interfaces;
using System.Text.Json;
using System.Text;
using Order.API.Dtos.Sepay;
using Domain.Enums;
using Microsoft.AspNetCore.SignalR;
using Order.API.Helpers;
using static System.Net.Mime.MediaTypeNames;
using static Domain.Enums.OrderEnum;
using System.Text.RegularExpressions;

namespace Order.API.Services.Behaviours
{
    public class PaymentsService : IPaymentsService
    {
        private readonly IPaymentsRepository _paymentsRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentsService> _logger;
        //private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IOrderRepository _orderRepository;
        private readonly IHubContext<OrderHub> _hubContext;
        private readonly IHttpClientFactory _httpClientFactory;

        public PaymentsService(IPaymentsRepository paymentsRepository,
            IMapper mapper, ILogger<PaymentsService> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IOrderRepository orderRepository,
            IHubContext<OrderHub> hubContext)
        {
            _paymentsRepository = paymentsRepository;
            _mapper = mapper;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _orderRepository = orderRepository;
            _hubContext = hubContext;
        }

        public async Task CheckPayment(SepayWebhookRequestDto request)
        {
            _logger.LogInformation("Entered method CheckPayment PaymentsService");
            var match = Regex.Match(request.Content ?? "", @"ORD\d+");
            string code = match.Success ? match.Value : "";

            _logger.LogInformation($"Order code: {code}");
            //int start = request.Content.IndexOf("ORD");
            //string code = request.Content.Substring(start);
            var order = await _orderRepository.GetOrderByCode(code);
            if (order != null && order.TotalAmount == request.TransferAmount)
            {
                switch (order.OrderType)
                {
                    case "Online":
                        //order.Status = (int)OrderEnum.EStatus.Processing;
                        order.IsPaid = true;
                        order.PaymentMethod = (int)EPaymentMethod.Banking;
                        _logger.LogInformation("Order type is Online. Updating status to Processing.");
                        break;
                    case "Offline":
                        order.IsPaid = true;
                        order.Status = (int)OrderEnum.EStatus.Completed;
                        order.PaymentMethod = (int)EPaymentMethod.Banking;
                        _logger.LogInformation("Order type is Offline. Updating status to Completed.");
                        break;
                    default:
                        _logger.LogWarning("Unknown order type: {OrderType}", order.OrderType);
                        break;
                }
                await _orderRepository.UpdateStatusOrder(order);
                _logger.LogInformation("Order status updated in database. New Status: {Status}", order.Status);

                await _hubContext.Clients.Group($"{order.Id}").SendAsync("OrderUpdated", new
                {
                    order.Id,
                    order.OrderCode,
                    order.BranchId,
                    order.Status
                });
                _logger.LogInformation("Notification sent to SignalR group: {Group}", order.Id);
            }

            _logger.LogInformation("Leaving method CheckPayment PaymentsService");
        }

        public async Task<byte[]> GenerateQR(Guid orderId)
        {
            try
            {
                _logger.LogInformation("Entered method GenerateQR PaymentsService");
                var order = await _orderRepository.GetOrderById(orderId);
                if (order != null)
                {
                    var _httpClient = _httpClientFactory.CreateClient();
                    var baseUrl = _configuration["VietQR:BaseUrl"];
                    var clientId = _configuration["VietQR:ClientId"];
                    var apiKey = _configuration["VietQR:ApiKey"];

                    var requestBody = new
                    {
                        accountNo = _configuration["VietQR:AccountNo"],
                        accountName = _configuration["VietQR:AccountName"],
                        acqId = _configuration["VietQR:AcqId"],
                        addInfo = $"{_configuration["VietQR:PrefixTransContent"]} _ {order.OrderCode}",
                        amount = Math.Round(order.TotalAmount).ToString("0"),
                        template = _configuration["VietQR:Template"]
                    };

                    var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                    _httpClient.DefaultRequestHeaders.Clear();
                    _httpClient.DefaultRequestHeaders.Add("x-client-id", clientId);
                    _httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);

                    var response = await _httpClient.PostAsync(baseUrl, jsonContent);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    var responseJson = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    if (responseJson.TryGetProperty("data", out JsonElement data) && data.TryGetProperty("qrDataURL", out JsonElement qrDataUrl))
                    {
                        var base64String = qrDataUrl.GetString().Replace("data:image/png;base64,", "");
                        return Convert.FromBase64String(base64String);
                    }
                }
                _logger.LogInformation("Leaving method GenerateQR PaymentsService");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while GenerateQR: {ex}");
                return null;

            }
        }
    }
}

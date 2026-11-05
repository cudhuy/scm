using Product.API.Dtos.InventoryItem;
using Product.API.Helpers;
using Product.API.Repositories.Interfaces;
using Product.API.Services.Interfaces;

namespace Product.API.Services.Behaviours
{
    public class AlertService : IAlertService
    {
        private readonly IInventoryService _inventoryService;
        private readonly EmailHelper _emailHelper;
        private readonly ILogger<AlertService> _logger;
        private readonly IAlertRepository _alertRepository;

        public AlertService(IInventoryService inventoryService, EmailHelper emailHelper, ILogger<AlertService> logger, IAlertRepository alertRepository)
        {
            _inventoryService = inventoryService;
            _emailHelper = emailHelper;
            _logger = logger;
            _alertRepository = alertRepository;
        }

        public async Task CheckAndSendAlerts()
        {
            _logger.LogInformation("Checking for expired ingredients...");

            List<LowStockInventoryItemDto> expiredItems = await _inventoryService.GetAllExpiredIngredients();
            if (!expiredItems.Any())
            {
                _logger.LogInformation("✅ No expired ingredients found.");
                return;
            }

            // Nhóm nguyên liệu theo chi nhánh
            var branchGroups = expiredItems.GroupBy(i => i.InventoryName);

            // Gửi mail cho từng chi nhánh
            foreach (var branchGroup in branchGroups)
            {
                string inventoryName = branchGroup.Key;

                // Tìm email của quản lý chi nhánh
                string? managerEmail = await GetBranchManagerEmail(inventoryName);


                if (string.IsNullOrEmpty(managerEmail))
                {
                    _logger.LogWarning($"⚠️ No manager email found for branch '{inventoryName}'.");
                    continue;
                }

                // Tạo nội dung email
                string emailBody = $@"
                <html>
                <body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;'>
                    <table align='center' width='100%' cellpadding='0' cellspacing='0' style='max-width: 600px; background-color: #ffffff; border-radius: 8px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);'>
                        <tr>
                            <td style='padding: 20px; text-align: center;'>
                                <h2 style='color: #333; margin-bottom: 10px;'>Cảnh báo nguyên liệu hết hạn</h2>
                                <p style='font-size: 16px; color: #555; margin: 0 0 20px;'>Danh sách nguyên liệu hết hạn tại chi nhánh <strong>{inventoryName}</strong>:</p>
                                <table width='100%' cellpadding='0' cellspacing='0' style='border-collapse: collapse;'>
                                    <thead>
                                        <tr style='background-color: #007bff; color: white;'>
                                            <th style='padding: 10px; text-align: left; font-size: 14px; width: 10%;'>Tên</th>
                                            <th style='padding: 10px; text-align: left; font-size: 14px; width: 20%;'>Ngày nhập</th>
                                            <th style='padding: 10px; text-align: left; font-size: 14px; width: 20%;'>Ngày hết hạn</th>
                                            <th style='padding: 10px; text-align: left; font-size: 14px; width: 50%;'>Nội dung</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {string.Join("", branchGroup.Select(item => $@"
                                        <tr>
                                            <td style='padding: 10px; text-align: left; border-bottom: 1px solid #ddd; font-size: 14px; color: #555; width: 10%;'><strong>{item.IngredientName}</strong></td>
                                            <td style='padding: 10px; text-align: left; border-bottom: 1px solid #ddd; font-size: 14px; color: #555; width: 20%;'>{item.LastRestock}</td>
                                            <td style='padding: 10px; text-align: left; border-bottom: 1px solid #ddd; font-size: 14px; color: #555; width: 20%;'>{item.ExpireDate}</td>
                                            <td style='padding: 10px; text-align: left; border-bottom: 1px solid #ddd; font-size: 14px; color: #555; width: 50%; word-break: break-word;'>{item.AlertMessage}</td>
                                        </tr>"))}
                                    </tbody>
                                </table>
                                <p style='font-size: 14px; color: #777; margin-top: 20px;'>Vui lòng kiểm tra và xử lý kịp thời.</p>
                                <hr style='border: none; height: 1px; background-color: #ddd; margin: 20px 0;'>
                                <p style='font-size: 14px; color: #999; margin: 0;'>Trân trọng,</p>
                                <p style='font-size: 14px; color: #555; font-weight: bold; margin: 5px 0 0;'>Đội ngũ quản lý kho</p>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>";

                // Gửi email cảnh báo
                bool isSent = await _emailHelper.SendEmail(managerEmail, "Cảnh báo nguyên liệu hết hạn", emailBody);

                if (isSent) _logger.LogInformation($"📩 Email sent successfully to {managerEmail}.");
                else _logger.LogError($"❌ Failed to send email to {managerEmail}.");
            }

        }

        private async Task<string?> GetBranchManagerEmail(string inventoryName)
        {
            return await _alertRepository.GetBranchManagerEmail(inventoryName);
        }
    }
}

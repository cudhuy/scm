using System.Net;
using System.Net.Mail;

namespace Auth.API.Helpers
{
    public class EmailHelper
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailHelper> _logger;

        public EmailHelper(IConfiguration configuration, ILogger<EmailHelper> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmail(string to, string subject, string otp, string name)
        {
            _logger.LogInformation("Entered method SendEmail");
            try
            {
                using var smtp = new SmtpClient
                {
                    Host = _configuration["Email:Smtp:Host"],
                    Port = _configuration.GetValue<int>("Email:Smtp:Port"),
                    Credentials = new NetworkCredential(
                        _configuration["Email:Smtp:User"],
                        _configuration["Email:Smtp:Password"]
                    ),
                    EnableSsl = true
                };

                string body = $@"
                <html>
                <body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;'>
                    <table align='center' width='100%' cellpadding='0' cellspacing='0' style='max-width: 600px; background-color: #ffffff; border-radius: 8px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);'>
                        <tr>
                            <td style='padding: 20px; text-align: center;'>
                                <h2 style='color: #333;'>Xin chào {name},</h2>
                                <p style='font-size: 16px; color: #555;'>Bạn đã yêu cầu mã xác minh từ <strong>Xóm</strong>.</p>
                                <p style='font-size: 16px; color: #555;'>Mã xác minh của bạn là:</p>
                                <div style='display: inline-block; padding: 10px 20px; font-size: 24px; font-weight: bold; color: white; background-color: #007bff; border-radius: 5px;'>
                                    {otp}
                                </div>
                                <p style='font-size: 14px; color: #777; margin-top: 10px;'>Mã xác minh của bạn sẽ hết hạn trong <strong>30 phút</strong>.</p>
                                <p style='font-size: 14px; color: #777;'>Nếu bạn không yêu cầu điều này, vui lòng bỏ qua email này.</p>
                                <hr style='border: none; height: 1px; background-color: #ddd; margin: 20px 0;'>
                                <p style='font-size: 14px; color: #999;'>Trân trọng,</p>
                                <p style='font-size: 14px; color: #555; font-weight: bold;'>Đội ngũ Xóm</p>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>";


                using var message = new MailMessage
                {
                    From = new MailAddress(_configuration["Email:From"]),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                message.To.Add(to);
                await smtp.SendMailAsync(message);
                _logger.LogInformation("Leaving method SendEmail");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error sending email: {ex.Message}");
                return false;
            }
        }
    }
}
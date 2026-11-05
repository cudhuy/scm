using System.Net;
using System.Net.Mail;

namespace Product.API.Helpers
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

        public async Task<bool> SendEmail(string to, string subject, string body)
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

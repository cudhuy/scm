using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;
using UserManagement.API.Messaging.KafkaMessage;

namespace UserManagement.API.Helpers
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

        public async Task SendEmailWithAttachment(string message)
        {
            _logger.LogInformation("Entered method SendEmailWithAttachment");
            try
            {
                SalarySlipMessage salarySlip = JsonConvert.DeserializeObject<SalarySlipMessage>(message) ?? new SalarySlipMessage();

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

                var emailBody = await GetEmailBodyAsync(salarySlip);

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(_configuration["Email:From"]),
                    Subject = $"Xóm Cafe - Lương Tháng {salarySlip.Month}/{salarySlip.Year} - {salarySlip.StaffName}",
                    Body = emailBody,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(salarySlip.Email);
                mailMessage.Attachments.Add(new Attachment(new MemoryStream(salarySlip.Data), salarySlip.FileName, "application/pdf"));
                await smtp.SendMailAsync(mailMessage);

                _logger.LogInformation("Leaving method SendEmailWithAttachment");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while SendEmailWithAttachment: {ex.Message}");
            }
        }


        private async Task<string> GetEmailBodyAsync(SalarySlipMessage salarySlip)
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "Emails", "salary_slip.html");

            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException("Email template file not found.");
            }

            var body = await File.ReadAllTextAsync(templatePath);

            return body
                .Replace("{{StaffName}}", salarySlip.StaffName)
                .Replace("{{Month}}", salarySlip.Month.ToString())
                .Replace("{{Year}}", salarySlip.Year.ToString())
                .Replace("{{CurrentYear}}", DateTime.Now.Year.ToString());
        }

    }
}

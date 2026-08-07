using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using HamperStore.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HamperStore.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var host = _config["SmtpSettings:Host"];
                if (string.IsNullOrEmpty(host))
                {
                    _logger.LogWarning("SMTP Host is not configured. Email was not sent.");
                    return false;
                }

                var portVal = _config["SmtpSettings:Port"] ?? "25";
                int.TryParse(portVal, out var port);
                var username = _config["SmtpSettings:Username"];
                var password = _config["SmtpSettings:Password"];
                var fromEmail = _config["SmtpSettings:FromEmail"] ?? "noreply@hamperstore.com";

                using var client = new SmtpClient(host, port);
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    client.Credentials = new NetworkCredential(username, password);
                    client.EnableSsl = true;
                }

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, "HamperStore Curations"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"Successfully sent email to {toEmail}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {toEmail}");
                return false;
            }
        }
    }
}

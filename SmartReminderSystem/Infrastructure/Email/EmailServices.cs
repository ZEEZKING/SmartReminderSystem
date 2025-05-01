using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace SmartReminderSystem.Infrastructure.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(
                _configuration["Smtp:FromName"],
                _configuration["Smtp:FromEmail"]
            ));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };
            email.Body = builder.ToMessageBody();

            try
            {
                using var smtp = new SmtpClient();

                // Connect using STARTTLS (TLS)
                await smtp.ConnectAsync(
                    _configuration["Smtp:Host"],
                    int.Parse(_configuration["Smtp:Port"]),
                    MailKit.Security.SecureSocketOptions.StartTls
                );

                // Authenticate
                await smtp.AuthenticateAsync(
                    _configuration["Smtp:Username"],
                    _configuration["Smtp:Password"]
                );

                // Send Email
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                Console.WriteLine("✅ Email sent successfully!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Email Error: {ex.Message}");
                return false;
            }
        }
    }
}

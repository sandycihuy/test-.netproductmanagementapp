using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using ProductManagementApp.Models;

namespace ProductManagementApp.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.From),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            using var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
            {
                Port = _emailSettings.Port,
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                EnableSsl = true,
            };

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}

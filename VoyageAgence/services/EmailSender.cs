using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace VoyageAgence.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient(_emailSettings.Host, _emailSettings.Port)
            {
                EnableSsl = _emailSettings.EnableSsl,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password)
            };

            var message = new MailMessage
            {
                From = new MailAddress(_emailSettings.Email, _emailSettings.SenderName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            message.To.Add(email);

            await client.SendMailAsync(message);
        }
    }

    public class EmailSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string SenderName { get; set; }
        public bool EnableSsl { get; set; }
    }
}
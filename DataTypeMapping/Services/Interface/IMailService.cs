using MailKit.Net.Smtp;
using MimeKit;

namespace DataTypeMapping.Services.Interface
{
    public interface IMailService
    {
        public Task<SmtpClient> CreateSmtpClientAsync(string toEmail, string password);
        public Task<MimeMessage> CreateMimeMessagesAsync(string toEmail, string subject , string body, string ccEmail = "" );
        public Task SendEmailAsync(string email, string subject, string message);
    }
}

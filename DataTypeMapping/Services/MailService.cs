using DataTypeMapping.Services.Interface;
using DataTypeMapping.Utilities.AppSettingsDO;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace DataTypeMapping.Services
{
    public class MailService : IMailService
    {
        // MailSettings mailSettings = new MailSettings(); // if we create new object then it will be null.
        private readonly MailSettings mailSettings;

        public MailService(IOptions<MailSettings> options)
        {
            this.mailSettings = options.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var mimeMessage = await CreateMimeMessagesAsync("sazid4294@gmail.com", subject, body);
                var smtpClient = await CreateSmtpClientAsync(mailSettings.Username, mailSettings.Password);
                await smtpClient.SendAsync(mimeMessage);
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log them)
                throw;
            }
        }

        public async Task<SmtpClient> CreateSmtpClientAsync(String username, string password) 
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Authentication failed while creating SmtpClient, invalid Username/password");
            }
                var smtpClient = new SmtpClient();
                try
                {
                    await smtpClient.ConnectAsync(mailSettings.Host, mailSettings.Port, SecureSocketOptions.StartTls);
                    await smtpClient.AuthenticateAsync(username, password);
                }
                catch
                {
                    smtpClient.Dispose();
                    throw;
                }
                return smtpClient; // caller disposes
            
        }

        public async Task<MimeMessage> CreateMimeMessagesAsync(string toEmail, string subject, string body, string ccEmail = "")
        {
            var Mimemessage = new MimeMessage();
            Mimemessage.From.Add(new MailboxAddress("MyApp", "yourEmail@gmail.com"));
            Mimemessage.To.Add(new MailboxAddress("", toEmail));
            Mimemessage.Subject = subject;
            Mimemessage.Body = new TextPart("html") { Text = body };
            return Mimemessage;
        }
    }
}

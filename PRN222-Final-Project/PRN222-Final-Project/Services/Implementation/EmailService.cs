using PRN222_Final_Project.ModelDto;
using PRN222_Final_Project.Services.Interface;
using System.Net.Mail;
using System.Net;

namespace PRN222_Final_Project.Services.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(EmailMessage emailMessage)
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            var senderName = _configuration["EmailSettings:SenderName"];
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var username = _configuration["EmailSettings:Username"];
            var password = _configuration["EmailSettings:Password"];

            var mail = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = emailMessage.Subject,
                Body = emailMessage.Body,
                IsBodyHtml = emailMessage.IsHtml
            };
            mail.To.Add(emailMessage.To);

            using var smtpClient = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };

            await smtpClient.SendMailAsync(mail);
        }
    }
}

using System.Net.Mail;
using PRN222_Final_Project.ModelDto;

namespace PRN222_Final_Project.Services.Interface
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessage emailMessage);
    }
}

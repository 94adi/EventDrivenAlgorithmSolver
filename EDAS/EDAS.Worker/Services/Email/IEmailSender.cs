namespace EDAS.Worker.Services.Email;

public interface IEmailSender
{
    Task SendEmailAsync(string toEmail, string subject,string body);
}

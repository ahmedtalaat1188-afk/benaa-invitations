namespace BenaaInvitations.API.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendPasswordResetCodeAsync(string to, string userName, string code);
    }
}

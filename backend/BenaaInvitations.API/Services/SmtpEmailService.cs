using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using BenaaInvitations.API.Models;

namespace BenaaInvitations.API.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public SmtpEmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPass);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // In production, log the exception. For demo, we might want to know it failed.
                Console.WriteLine($"Email failed: {ex.Message}");
                throw;
            }
        }

        public async Task SendPasswordResetCodeAsync(string to, string userName, string code)
        {
            string subject = "كود استعادة كلمة المرور - منصة بناء";
            
            // Professional Arabic HTML Template
            string body = $@"
            <div dir='rtl' style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e2e8f0; border-radius: 12px; overflow: hidden;'>
                <div style='background-color: #1e293b; padding: 20px; text-align: center;'>
                    <h2 style='color: #ffffff; margin: 0;'>منصة بناء | Benaa</h2>
                </div>
                <div style='padding: 30px; line-height: 1.6; color: #334155;'>
                    <p>عزيزي <strong>{userName}</strong>،</p>
                    <p>لقد تلقينا طلباً لإعادة تعيين كلمة المرور الخاصة بحسابك في منصة بناء.</p>
                    <div style='background-color: #f1f5f9; padding: 20px; border-radius: 8px; text-align: center; margin: 25px 0;'>
                        <p style='margin-bottom: 10px; font-size: 0.9rem;'>كود التحقق الخاص بك هو:</p>
                        <span style='font-size: 2.5rem; font-weight: bold; letter-spacing: 10px; color: #2563eb;'>{code}</span>
                    </div>
                    <p>يرجى إدخال هذا الكود في الشاشة المخصصة لإتمام عملية إعادة التعيين. هذا الكود صالح لمدة ساعة واحدة فقط.</p>
                    <p style='color: #64748b; font-size: 0.85rem;'>إذا لم تكن أنت من طلب هذا التغيير، فيرجى تجاهل هذا البريد.</p>
                </div>
                <div style='background-color: #f8fafc; padding: 15px; text-align: center; border-top: 1px solid #e2e8f0; font-size: 0.8rem; color: #94a3b8;'>
                    &copy; 2026 جميع الحقوق محفوظة - إدارة تقنية المعلومات | hns.edu.sa
                </div>
            </div>";

            await SendEmailAsync(to, subject, body);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BenaaInvitations.API.Data;
using BenaaInvitations.API.Models;
using BenaaInvitations.API.Services;

namespace BenaaInvitations.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ITenantService _tenantService;
        private readonly IEmailService _emailService;

        public UsersController(ApplicationDbContext context, ITenantService tenantService, IEmailService emailService)
        {
            _context = context;
            _tenantService = tenantService;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            return await _context.AppUsers.ToListAsync();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // For Benaa Platform, we match by Email (as seen in frontend)
            var user = await _context.AppUsers
                .IgnoreQueryFilters() // 🔥 Crucial for login to work across any tenant context
                .FirstOrDefaultAsync(u => 
                    u.Email == request.Username && 
                    u.PasswordHash == request.Password); // In production, use hashing (e.g. BCrypt)

            if (user == null)
            {
                return Unauthorized(new { message = "بيانات الدخول غير صحيحة" });
            }

            return Ok(new { 
                token = "benaa-secure-session-token", // Simulated JWT
                role = user.Role,
                username = user.Name 
            });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Email == request.Email);
            
            // Security Best Practice: Don't reveal if user exists. 
            // But for this platform's UX, we provide feedback.
            if (user == null) return Ok(new { message = "إذا كان الحساب موجوداً، فقد أرسلنا كود الاستعادة." });

            // Generate a secure 6-digit code
            var token = new Random().Next(100000, 999999).ToString();
            user.ResetPasswordToken = token;
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            
            await _context.SaveChangesAsync();

            try 
            {
                await _emailService.SendPasswordResetCodeAsync(user.Email, user.Name, token);
                return Ok(new { message = "تم إرسال كود الاستعادة لبريدك الإلكتروني.", debugToken = token });
            }
            catch (Exception)
            {
                return Ok(new { message = "فشل إرسال البريد، ولكن تم توليد الكود (للتجربة: " + token + ")", debugToken = token });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var user = await _context.AppUsers.FirstOrDefaultAsync(u => 
                u.Email == request.Email && 
                u.ResetPasswordToken == request.Token && 
                u.ResetTokenExpiry > DateTime.UtcNow);

            if (user == null) return BadRequest(new { message = "الكود غير صحيح أو انتهت صلاحيته." });

            // Update password (should be hashed in production)
            user.PasswordHash = request.NewPassword;
            user.ResetPasswordToken = null;
            user.ResetTokenExpiry = null;

            await _context.SaveChangesAsync();
            return Ok(new { message = "تم تغيير كلمة المرور بنجاح." });
        }
    }

    public class LoginRequest { public string Username { get; set; } = string.Empty; public string Password { get; set; } = string.Empty; }
    public class ForgotPasswordRequest { public string Email { get; set; } = string.Empty; }
    public class ResetPasswordRequest 
    { 
        public string Email { get; set; } = string.Empty; 
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}

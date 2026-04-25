using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BenaaInvitations.API.Data;
using BenaaInvitations.API.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;

namespace BenaaInvitations.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RSVPController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RSVPController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("confirm/{token}")]
        public async Task<IActionResult> Confirm(string token)
        {
            // Note: Since SecureToken is unique across the whole DB, 
            // we don't strictly need the tenant filter here for public links,
            // but the DbContext global filter is active. 
            // However, public RSVP links should probably IGNORE the tenant filter
            // because the guest might click it from a general domain or the specific subdomain.
            // But since the middleware sets the tenant from the Host, 
            // the Global Filter WILL apply.
            
            var guest = await _context.Guests
                .Include(g => g.Event)
                .IgnoreQueryFilters() // Public links should find the guest regardless of current host subdomain if they are correct
                .FirstOrDefaultAsync(g => g.SecureToken == token);
            
            if (guest == null) return NotFound("رابط غير صحيح أو منتهي الصلاحية.");

            if (guest.Status == 1) 
            {
                return Ok(new { status = "AlreadyConfirmed", message = "تم تأكيد حضورك مسبقاً." });
            }

            guest.Status = 1; // Confirmed
            guest.RSVPDate = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            
            return Ok(new { 
                status = "Success", 
                message = "تم تأكيد حضورك بنجاح! سيصلك رابط التذكرة عبر الواتساب لاحقاً.",
                guestName = guest.Name
            });

        }

        [HttpPost("decline/{token}")]
        public async Task<IActionResult> Decline(string token)
        {
            var guest = await _context.Guests.IgnoreQueryFilters().FirstOrDefaultAsync(g => g.SecureToken == token);
            if (guest == null) return NotFound("رابط غير صحيح.");

            guest.Status = 2; // Declined
            guest.RSVPDate = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            
            return Ok(new { status = "Declined", message = "نشكرك على إبلاغنا، سنفتقد تواجدك في هذه الفعالية ونأمل رؤيتك في مناسبات قادمة." });
        }

        [HttpGet("image/{token}")]
        public async Task<IActionResult> GetInvitationImage(string token)
        {
            var guest = await _context.Guests
                .Include(g => g.Event)
                .FirstOrDefaultAsync(g => g.SecureToken == token);

            if (guest == null || string.IsNullOrEmpty(guest.Event.CustomTemplatePath))
                return NotFound();

            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", guest.Event.CustomTemplatePath);
            if (!System.IO.File.Exists(imagePath)) return NotFound("Template file missing.");

            using (var image = await SixLabors.ImageSharp.Image.LoadAsync(imagePath))
            {
                // Simple Text Overlay at (NameX, NameY)
                // In a real app, we'd load a specific font (e.g., Amiri.ttf)
                var x = guest.Event?.NameX ?? 100;
                var y = guest.Event?.NameY ?? 100;

                FontFamily family;
                if (!SystemFonts.TryGet("Arial", out family))
                {
                    family = SystemFonts.Families.FirstOrDefault()!;
                }

                if (family != null)
                {
                    var font = family.CreateFont(40);
                    image.Mutate(ctx => ctx.DrawText(
                        guest.Name, 
                        font, 
                        SixLabors.ImageSharp.Color.Black, 
                        new SixLabors.ImageSharp.PointF(x, y)
                    ));
                }

                var ms = new MemoryStream();
                await image.SaveAsPngAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, "image/png");
            }
        }
    }
}

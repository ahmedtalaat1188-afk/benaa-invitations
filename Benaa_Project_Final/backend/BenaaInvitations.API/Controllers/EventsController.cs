using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BenaaInvitations.API.Data;
using BenaaInvitations.API.Models;
using BenaaInvitations.API.Services;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;

namespace BenaaInvitations.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ITenantService _tenantService;
        private readonly IStudentsService _studentsService;
        private readonly IAIDesignService _aiDesignService;

        public EventsController(ApplicationDbContext context, 
                               ITenantService tenantService, 
                               IStudentsService studentsService,
                               IAIDesignService aiDesignService)
        {
            _context = context;
            _tenantService = tenantService;
            _studentsService = studentsService;
            _aiDesignService = aiDesignService;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            // Global filter in DbContext handles SchoolId
            return await _context.Events.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            var @event = await _context.Events.Include(e => e.Guests).FirstOrDefaultAsync(e => e.Id == id);
            
            if (@event == null) return NotFound();
            
            return @event;
        }

        [HttpPost]
        public async Task<ActionResult<Event>> CreateEvent(Event @event)
        {
            if (_tenantService.SchoolId == null) return BadRequest("School context missing.");

            @event.SchoolId = _tenantService.SchoolId.Value;
            _context.Events.Add(@event);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetEvent), new { id = @event.Id }, @event);
        }

        [HttpGet("{id}/stats")]
        public async Task<IActionResult> GetStats(int id)
        {
            var guests = await _context.Guests.Where(g => g.EventId == id).ToListAsync();
            var stats = new {
                Total = guests.Count,
                Pending = guests.Count(g => g.Status == 0),
                Confirmed = guests.Count(g => g.Status == 1),
                Declined = guests.Count(g => g.Status == 2),
                Attended = guests.Count(g => g.Status == 3)
            };
            return Ok(stats);
        }

        [HttpGet("{id}/qr/{guestToken}")]
        public async Task<IActionResult> GetQRCode(int id, string guestToken)
        {
            var guest = await _context.Guests.FirstOrDefaultAsync(g => g.EventId == id && g.SecureToken == guestToken);
            
            if (guest == null) return NotFound();

            // QR contains the confirmation token
            string qrData = guest.SecureToken; 
            
            using QRCodeGenerator qrGenerator = new QRCodeGenerator();
            using QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
            using PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeImage = qrCode.GetGraphic(20);
            
            return File(qrCodeImage, "image/png");
        }

        [HttpPost("{id}/add-guests")]
        public async Task<IActionResult> AddGuests(int id, List<Guest> guests)
        {
            if (_tenantService.SchoolId == null) return BadRequest("School context missing.");
            
            var @event = await _context.Events.FindAsync(id);
            if (@event == null) return NotFound();

            var errors = new List<string>();
            var validGuests = new List<Guest>();

            foreach (var guest in guests)
            {
                // Simple Data Validation
                if (string.IsNullOrWhiteSpace(guest.Name)) errors.Add($"Guest name missing.");
                if (string.IsNullOrWhiteSpace(guest.Phone) || !System.Text.RegularExpressions.Regex.IsMatch(guest.Phone, @"^\+?[0-9]{10,15}$"))
                    errors.Add($"Invalid or missing phone for {guest.Name}.");

                if (errors.Count == 0)
                {
                    guest.EventId = id;
                    guest.SchoolId = _tenantService.SchoolId.Value;
                    guest.SecureToken = Guid.NewGuid().ToString("N");
                    validGuests.Add(guest);
                }
            }

            if (errors.Any()) return BadRequest(new { message = "Validation errors found.", errors });

            _context.Guests.AddRange(validGuests);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"{validGuests.Count} guests added successfully." });
        }

        [HttpGet("guest/{guestId}/whatsapp")]
        public async Task<IActionResult> GetWhatsAppMessage(int guestId)
        {
            var guest = await _context.Guests.Include(g => g.Event).FirstOrDefaultAsync(g => g.Id == guestId);
            if (guest == null) return NotFound();

            var school = await _context.Schools.FindAsync(guest.SchoolId);
            string schoolName = school?.Name ?? "مدارس الأحساء النموذجية الأهلية";

            // Dynamic Subdomain link for confirmation
            string confirmUrl = $"https://{_tenantService.Subdomain}.benaa.sa/rsvp/confirm/{guest.SecureToken}";
            
            string message = $"مرحباً {guest.Name}، 🌹\n\n" +
                             $"نتشرف بدعوتكم لحضور {guest.Event?.Title} في {schoolName}.\n" +
                             $"تجدون أعلاه تصميم الدعوة الرسمي الخاص بكم 🎨.\n\n" +
                             $"الرجاء تأكيد الحضور من خلال الرابط:\n" +
                             $"{confirmUrl}\n\n";


            if (!string.IsNullOrEmpty(guest.Event?.LocationUrl))
            {
                message += $"📍 موقع الفعالية (Google Maps):\n{guest.Event.LocationUrl}\n\n";
            }

            message += "نسعد بوجودكم!";

            string encodedMessage = WebUtility.UrlEncode(message);
            string waLink = $"https://wa.me/{guest.Phone}?text={encodedMessage}";

            return Ok(new { whatsappLink = waLink, messageText = message });
        }
        [HttpGet("{id}/export-report")]
        public async Task<IActionResult> GetAttendanceReport(int id)
        {
            var guests = await _context.Guests.Where(g => g.EventId == id).ToListAsync();
            
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Name,Phone,Grade,Section,Status,RSVPDate");
            
            foreach (var g in guests)
            {
                var status = g.Status == 1 ? "Confirmed" : (g.Status == 2 ? "Declined" : "Pending");
                csv.AppendLine($"{g.Name},{g.Phone},{g.Grade},{g.Section},{status},{g.RSVPDate}");
            }
            
            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", $"attendance_report_event_{id}.csv");
        }
        [HttpGet("verify-scan/{token}")]
        public async Task<IActionResult> VerifyScan(string token)
        {
            var guest = await _context.Guests
                .IgnoreQueryFilters() // Scanning happens at the gate, might bypass tenant filters if needed
                .FirstOrDefaultAsync(g => g.SecureToken == token);

            if (guest == null) return NotFound(new { message = "كود غير صالح أو غير موجود في النظام." });

            if (guest.Status == 3) return BadRequest(new { message = "تم تسجيل حضور هذا الضيف مسبقاً!" });
            if (guest.Status == 2) return BadRequest(new { message = "هذا الضيف سجل اعتذاره عن الحضور مسبقاً." });

            guest.Status = 3; // Attended
            guest.RSVPDate = DateTime.Now; // Used here as Arrival Date
            
            await _context.SaveChangesAsync();

            return Ok(new { 
                status = "Success", 
                message = "تم تسجيل الحضور بنجاح! نرحب بكم.", 
                guestName = guest.Name 
            });
        }

        [HttpPost("{id}/generate-design")]
        public async Task<IActionResult> GenerateDesign(int id, [FromBody] DesignRequest request)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event == null) return NotFound();

            try
            {
                var imageUrl = await _aiDesignService.GenerateInvitationBackgroundAsync(request.Prompt);
                @event.AttachmentUrl = imageUrl; 
                await _context.SaveChangesAsync();
                
                return Ok(new { imageUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("guest/{guestId}/ticket-whatsapp")]
        public async Task<IActionResult> GetTicketWhatsAppMessage(int guestId)
        {
            var guest = await _context.Guests.Include(g => g.Event).FirstOrDefaultAsync(g => g.Id == guestId);
            if (guest == null) return NotFound();

            string ticketUrl = $"https://{_tenantService.Subdomain}.benaa.sa/t/{guest.SecureToken}";
            
            string message = $"مرحباً {guest.Name}، 🌹\n\n" +
                             $"يسعدنا تأكيد حضوركم لـ {guest.Event?.Title}.\n" +
                             $"إليكم تذكرة الدخول الرسمية (QR Code):\n" +
                             $"{ticketUrl}\n\n" +
                             $"ننتظركم بكل حماس!";

            string encodedMessage = WebUtility.UrlEncode(message);
            string waLink = $"https://wa.me/{guest.Phone}?text={encodedMessage}";

            return Ok(new { whatsappLink = waLink, messageText = message });
        }
    }

    public class DesignRequest { public string Prompt { get; set; } = string.Empty; }
}


using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using BenaaInvitationPlatform.Models;

namespace BenaaInvitationPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        // Mock DB Context for logic demonstration
        private static List<Event> _events = new List<Event> {
            new Event { Id = 1, Name = "حفل تخرج الثانوي 2026", EventDate = DateTime.Now.AddDays(30), Location = "القاعة الكبرى" }
        };

        private static List<Guest> _guests = new List<Guest> {
            new Guest { Id = 1, EventId = 1, FullName = "سعيد القحطاني", PhoneNumber = "966563048351", Status = "Pending", TicketCode = "TKT-001" },
            new Guest { Id = 2, EventId = 1, FullName = "ريم العتيبي", PhoneNumber = "966563048351", Status = "Pending", TicketCode = "TKT-002" }
        };

        [HttpGet]
        public IActionResult GetEvents() => Ok(_events);

        [HttpGet("{id}/guests")]
        public IActionResult GetGuests(int id) => Ok(_guests.Where(g => g.EventId == id));

        [HttpPost("{id}/guests")]
        public IActionResult AddGuest(int id, [FromBody] Guest guest)
        {
            guest.Id = _guests.Max(g => g.Id) + 1;
            guest.EventId = id;
            guest.TicketCode = "TKT-" + Guid.NewGuid().ToString().Substring(0, 8);
            _guests.Add(guest);
            return Ok(guest);
        }

        [HttpPatch("guests/{guestId}/status")]
        public IActionResult UpdateStatus(int guestId, [FromBody] StatusUpdate update)
        {
            var guest = _guests.FirstOrDefault(g => g.Id == guestId);
            if (guest == null) return NotFound();
            guest.Status = update.Status;
            guest.LastUpdated = DateTime.Now;
            return Ok(guest);
        }
    }

    public class StatusUpdate { public string Status { get; set; } }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BenaaInvitationPlatform.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } // Admin, Scanner
    }

    public class Event
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; }
        public int CreatedByUserId { get; set; }
        public bool IsActive { get; set; } = true;
        
        public ICollection<Guest> Guests { get; set; }
        public ICollection<DesignTemplate> Templates { get; set; }
    }

    public class Guest
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Declined, Attended
        public string TicketCode { get; set; } // Unique GUID or short code
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }

    public class DesignTemplate
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string PromptText { get; set; }
        public string ImagePath { get; set; }
        public bool IsActive { get; set; }
    }
}

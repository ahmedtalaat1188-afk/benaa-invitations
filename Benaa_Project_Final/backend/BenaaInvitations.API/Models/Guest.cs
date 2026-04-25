using System.ComponentModel.DataAnnotations;

namespace BenaaInvitations.API.Models
{
    public class Guest
    {
        public int Id { get; set; }
        public int SchoolId { get; set; } // Required for multi-tenancy
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Phone { get; set; } = string.Empty;
        
        public string? Grade { get; set; }
        public string? Section { get; set; }
        
        public int EventId { get; set; }
        public Event? Event { get; set; }
        
        // Status: 0 = Pending, 1 = Confirmed, 2 = Declined
        public int Status { get; set; } = 0;
        
        public string SecureToken { get; set; } = Guid.NewGuid().ToString("N");
        
        public DateTime? RSVPDate { get; set; }
        
        public bool IsNotified { get; set; } = false;
    }
}

using System.ComponentModel.DataAnnotations;

namespace BenaaInvitations.API.Models
{
    public class Event
    {
        public int Id { get; set; }
        public int SchoolId { get; set; } 
        public School? School { get; set; }
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        public string? Date { get; set; } // Matches frontend 'date'
        public string? Time { get; set; } // Matches frontend 'time'
        public string? LocationLabel { get; set; } // Matches frontend 'locationLabel'
        
        public DateTime EventDate { get; set; } // Internal use
        public string? Location { get; set; }
        public string? LocationUrl { get; set; } 
        
        public string? Description { get; set; }
        public string? AttachmentUrl { get; set; } 
        
        public ICollection<Guest> Guests { get; set; } = new List<Guest>();
    }
}

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
        
        public string? Date { get; set; } 
        public string? Time { get; set; } 
        public string? LocationLabel { get; set; } 
        
        public string? Subtitle { get; set; }
        public DateTime EventDate { get; set; } 
        public string? Location { get; set; }
        public string? LocationUrl { get; set; } 
        
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        public string? ThemeColor { get; set; }
        public string? AttachmentUrl { get; set; } 
        
        public string? CustomTemplatePath { get; set; }
        public int? NameX { get; set; }
        public int? NameY { get; set; }

        public ICollection<Guest> Guests { get; set; } = new List<Guest>();
    }
}

using System.ComponentModel.DataAnnotations;

namespace BenaaInvitations.API.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        public string Role { get; set; } = "Viewer"; // Viewer, Admin, Scanner
        
        public int SchoolId { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public bool IsActive { get; set; } = true;

        // Password Reset Fields
        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }
    }
}

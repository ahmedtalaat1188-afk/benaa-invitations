namespace BenaaInvitations.API.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public int SchoolId { get; set; } // Required for multi-tenancy
        public int GuestId { get; set; }
        public Guest? Guest { get; set; }
        public int EventId { get; set; }
        public Event? Event { get; set; }
        public DateTime ScanTime { get; set; } = DateTime.UtcNow;
        public string? ScannedBy { get; set; }
    }
}

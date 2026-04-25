namespace BenaaInvitations.API.Models
{
    public class School
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Subdomain { get; set; } = string.Empty;
        public string PrimaryColor { get; set; } = "#2563eb";
        
        public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}

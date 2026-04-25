namespace BenaaInvitations.API.Services
{
    public interface ITenantService
    {
        int? SchoolId { get; set; }
        string? Subdomain { get; set; }
    }

    public class TenantService : ITenantService
    {
        public int? SchoolId { get; set; }
        public string? Subdomain { get; set; }
    }
}

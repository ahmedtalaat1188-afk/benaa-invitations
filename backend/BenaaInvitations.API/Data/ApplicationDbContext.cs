using Microsoft.EntityFrameworkCore;
using BenaaInvitations.API.Models;
using BenaaInvitations.API.Services;

namespace BenaaInvitations.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ITenantService _tenantService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantService tenantService)
            : base(options)
        {
            _tenantService = tenantService;
        }

        public DbSet<School> Schools { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Note: School should NOT have a query filter by itself if we lookup by Subdomain/Host
            modelBuilder.Entity<Event>().HasQueryFilter(e => e.SchoolId == _tenantService.SchoolId);
            modelBuilder.Entity<Student>().HasQueryFilter(s => s.SchoolId == _tenantService.SchoolId);
            modelBuilder.Entity<Guest>().HasQueryFilter(g => g.SchoolId == _tenantService.SchoolId);
            modelBuilder.Entity<Attendance>().HasQueryFilter(a => a.SchoolId == _tenantService.SchoolId);
            modelBuilder.Entity<AppUser>().HasQueryFilter(u => u.SchoolId == _tenantService.SchoolId);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.Guests)
                .WithOne(g => g.Event)
                .HasForeignKey(g => g.EventId);

            modelBuilder.Entity<Guest>()
                .Property(g => g.SecureToken)
                .IsRequired();
            
            // Seed initial school
            modelBuilder.Entity<School>().HasData(new School
            {
                Id = 1,
                Name = "مدارس الأحساء النموذجية الأهلية",
                Subdomain = "ahsa-model",
                PrimaryColor = "#1e293b"
            });

            // Seed initial Admin users
            modelBuilder.Entity<AppUser>().HasData(
                new AppUser
                {
                    Id = 1,
                    SchoolId = 1,
                    Name = "أحمد طلعت (مدير)",
                    Email = "admin@hns.edu.sa",
                    PasswordHash = "admin123",
                    Role = "Admin",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 4, 9, 0, 0, 0, DateTimeKind.Utc)
                },
                new AppUser
                {
                    Id = 2,
                    SchoolId = 1,
                    Name = "أحمد طلعت",
                    Email = "talaat@hns.edu.sa",
                    PasswordHash = "admin123",
                    Role = "Admin",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 4, 9, 0, 0, 0, DateTimeKind.Utc)
                },
                new AppUser
                {
                    Id = 3,
                    SchoolId = 1,
                    Name = "ماسح التذاكر",
                    Email = "scanner@hns.edu.sa",
                    PasswordHash = "scanner123",
                    Role = "Scanner",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 4, 9, 0, 0, 0, DateTimeKind.Utc)
                }
            );

        }
    }
}

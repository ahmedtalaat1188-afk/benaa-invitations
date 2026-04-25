using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using BenaaInvitations.API.Data;
using BenaaInvitations.API.Services;

namespace BenaaInvitations.API.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("BenaaConnection");

            // Use SQL Server as it's the target for the User
            builder.UseSqlServer(connectionString ?? "Server=(localdb)\\mssqllocaldb;Database=BenaaDB;Trusted_Connection=True;MultipleActiveResultSets=true");

            // Provide a mock/dummy implementation of ITenantService for design-time
            return new ApplicationDbContext(builder.Options, new DesignTimeTenantService());
        }
    }

    public class DesignTimeTenantService : ITenantService
    {
        public int? SchoolId { get; set; } = 1;
        public string? Subdomain { get; set; } = "design";
    }
}

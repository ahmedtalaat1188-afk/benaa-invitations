using BenaaInvitations.API.Data;
using BenaaInvitations.API.Services;
using Microsoft.EntityFrameworkCore;

namespace BenaaInvitations.API.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITenantService tenantService, ApplicationDbContext dbContext)
        {
            var host = context.Request.Host.Host;
            
            // Local development helper
            if (host == "localhost" || host == "127.0.0.1")
            {
                tenantService.SchoolId = 1;
                tenantService.Subdomain = "ahsa-model";
            }
            else
            {
                var subdomain = GetSubdomain(host);
                if (!string.IsNullOrEmpty(subdomain))
                {
                    var school = await dbContext.Schools
                        .IgnoreQueryFilters()
                        .AsNoTracking()
                        .FirstOrDefaultAsync(s => s.Subdomain == subdomain);

                    if (school != null)
                    {
                        tenantService.SchoolId = school.Id;
                        tenantService.Subdomain = subdomain;
                    }
                }
            }

            await _next(context);
        }

        private string? GetSubdomain(string host)
        {
            // Simple logic: school1.benaa.sa -> school1
            var parts = host.Split('.');
            if (parts.Length >= 3)
            {
                return parts[0];
            }
            return null;
        }
    }
}

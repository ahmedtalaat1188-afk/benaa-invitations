using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BenaaInvitations.API.Data;
using BenaaInvitations.API.Models;
using BenaaInvitations.API.Services;

namespace BenaaInvitations.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ITenantService _tenantService;

        public BrandingController(ApplicationDbContext context, ITenantService tenantService)
        {
            _context = context;
            _tenantService = tenantService;
        }

        [HttpGet("config")]
        public async Task<IActionResult> GetBrandingConfig()
        {
            // Note: TenantMiddleware should have set SchoolId or Subdomain
            var subdomain = _tenantService.Subdomain;
            
            if (string.IsNullOrEmpty(subdomain)) 
            {
                // Fallback or default for base domain
                return Ok(new { 
                    name = "مدارس الأحساء النموذجية الأهلية", 
                    primaryColor = "#1e293b", 
                    secondaryColor = "#6366f1",
                    logoUrl = "assets/images/default-logo.png"
                });
            }

            var school = await _context.Schools
                .IgnoreQueryFilters() // Branding lookup needs to find the school even if no tenant context is set yet
                .FirstOrDefaultAsync(s => s.Subdomain == subdomain);

            if (school == null) return NotFound("School not found.");

            return Ok(new {
                name = school.Name,
                primaryColor = school.PrimaryColor,
                secondaryColor = school.SecondaryColor,
                logoUrl = school.LogoUrl ?? "assets/images/default-logo.png"
            });
        }
    }
}

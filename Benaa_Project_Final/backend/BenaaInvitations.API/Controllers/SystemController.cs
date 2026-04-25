using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BenaaInvitations.API.Data;

namespace BenaaInvitations.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SystemController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SystemController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetStatus()
        {
            var status = new List<object>();
            
            try 
            {
                bool canConnect = await _context.Database.CanConnectAsync();
                status.Add(new { component = "Database Connection", status = canConnect ? "Connected" : "Failed" });
                
                if (canConnect)
                {
                    status.Add(new { table = "Schools", count = await _context.Schools.IgnoreQueryFilters().CountAsync() });
                    status.Add(new { table = "AppUsers", count = await _context.AppUsers.IgnoreQueryFilters().CountAsync() });
                    status.Add(new { table = "Events", count = await _context.Events.IgnoreQueryFilters().CountAsync() });
                    status.Add(new { table = "Guests", count = await _context.Guests.IgnoreQueryFilters().CountAsync() });
                    status.Add(new { table = "Students", count = await _context.Students.IgnoreQueryFilters().CountAsync() });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { overallStatus = "Error", error = ex.Message, details = status });
            }

            return Ok(new { overallStatus = "Healthy", details = status });
        }
    }
}

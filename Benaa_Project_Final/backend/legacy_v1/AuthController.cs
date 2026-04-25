using Microsoft.AspNetCore.Mvc;
using System.Linq;
using BenaaInvitationPlatform.Models;
using System.Collections.Generic;

namespace BenaaInvitationPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // For demo purposes, we use a mock repository. 
        // In real app, inject ApplicationDbContext.
        private static List<User> _mockUsers = new List<User> {
            new User { Id = 1, Username = "admin", PasswordHash = "admin123", Role = "Admin" }
        };

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _mockUsers.FirstOrDefault(u => u.Username == request.Username && u.PasswordHash == request.Password);
            
            if (user == null) return Unauthorized(new { message = "Invalid credentials" });

            return Ok(new { 
                token = "mock-jwt-token", 
                role = user.Role,
                username = user.Username 
            });
        }
    }

    public class LoginRequest {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}

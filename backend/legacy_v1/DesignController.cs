using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using BenaaInvitationPlatform.Models;

namespace BenaaInvitationPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DesignController : ControllerBase
    {
        private static List<DesignTemplate> _templates = new List<DesignTemplate>();

        [HttpPost("generate")]
        public IActionResult GenerateDesign([FromBody] DesignRequest request)
        {
            // In a real scenario, call OpenAI/DALL-E here.
            // For now, we simulate saving the path.
            var newDesign = new DesignTemplate {
                Id = _templates.Count + 1,
                EventId = request.EventId,
                PromptText = request.Prompt,
                ImagePath = "assets/ai_generated_placeholder.png", // Mocked path
                IsActive = false
            };
            _templates.Add(newDesign);
            return Ok(newDesign);
        }

        [HttpPost("save-active")]
        public IActionResult SetActive([FromBody] int templateId)
        {
            var template = _templates.FirstOrDefault(t => t.Id == templateId);
            if (template == null) return NotFound();
            
            // Deactivate others for this event
            _templates.Where(t => t.EventId == template.EventId).ToList().ForEach(t => t.IsActive = false);
            template.IsActive = true;
            
            return Ok(new { message = "Template set as active for invitations" });
        }
    }

    public class DesignRequest {
        public int EventId { get; set; }
        public string Prompt { get; set; }
    }
}

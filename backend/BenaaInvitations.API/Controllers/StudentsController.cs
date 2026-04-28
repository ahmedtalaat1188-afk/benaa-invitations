using Microsoft.AspNetCore.Mvc;
using BenaaInvitations.API.Services;
using BenaaInvitations.API.Models;

namespace BenaaInvitations.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentsService _studentsService;

        public StudentsController(IStudentsService studentsService)
        {
            _studentsService = studentsService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetAllStudents([FromQuery] string? grade, [FromQuery] string? section)
        {
            var students = await _studentsService.GetStudentsAsync(grade, section);
            return Ok(students);
        }

        [HttpGet("grades")]
        public async Task<ActionResult<IEnumerable<string>>> GetGrades()
        {
            var grades = await _studentsService.GetGradesAsync();
            return Ok(grades);
        }

        [HttpGet("sections")]
        public async Task<ActionResult<IEnumerable<string>>> GetSections()
        {
            var sections = await _studentsService.GetSectionsAsync();
            return Ok(sections);
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> CreateStudents(List<Student> students)
        {
            await _studentsService.AddStudentsAsync(students);
            return Ok(new { message = $"{students.Count} records added successfully." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(string id, Student student)
        {
            await _studentsService.UpdateStudentAsync(id, student);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(string id)
        {
            await _studentsService.DeleteStudentAsync(id);
            return NoContent();
        }
    }
}

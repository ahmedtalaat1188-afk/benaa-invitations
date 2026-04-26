using Microsoft.EntityFrameworkCore;
using BenaaInvitations.API.Data;
using BenaaInvitations.API.Models;

namespace BenaaInvitations.API.Services
{
    public interface IStudentsService
    {
        Task<List<Student>> GetStudentsAsync(string? grade, string? section);
        Task<List<string>> GetGradesAsync();
        Task<List<string>> GetSectionsAsync();
        Task AddStudentsAsync(List<Student> students);
    }

    public class StudentsService : IStudentsService
    {
        private readonly ApplicationDbContext _context;

        public StudentsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Student>> GetStudentsAsync(string? grade, string? section)
        {
            var query = _context.Students.AsQueryable();

            if (!string.IsNullOrEmpty(grade))
                query = query.Where(s => s.Grade == grade);
            
            if (!string.IsNullOrEmpty(section))
                query = query.Where(s => s.Section == section);

            return await query.ToListAsync();
        }

        public async Task<List<string>> GetGradesAsync()
        {
             return await _context.Students
                .Select(s => s.Grade)
                .Where(g => g != null)
                .Distinct()
                .Cast<string>()
                .ToListAsync();
        }

        public async Task<List<string>> GetSectionsAsync()
        {
             return await _context.Students
                .Select(s => s.Section)
                .Where(s => s != null)
                .Distinct()
                .Cast<string>()
                .ToListAsync();
        }

        public async Task AddStudentsAsync(List<Student> students)
        {
            // Set default schoolId if not present (simplified for this QC pass)
            foreach(var s in students) {
                if(s.SchoolId == 0) s.SchoolId = 1; 
                if(string.IsNullOrEmpty(s.NameAr) && !string.IsNullOrEmpty(s.Name)) s.NameAr = s.Name;
                if(string.IsNullOrEmpty(s.NationalId)) s.NationalId = Guid.NewGuid().ToString("N");
            }
            _context.Students.AddRange(students);
            await _context.SaveChangesAsync();
        }
    }
}

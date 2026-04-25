using System.ComponentModel.DataAnnotations;

namespace BenaaInvitations.API.Models
{
    public class Student
    {
        [Key]
        public string NationalId { get; set; } = string.Empty; // هوية الطالب/الإقامة
        
        [Required]
        public string NameAr { get; set; } = string.Empty; // اسم الطالب الرباعي بالعربية
        
        public string NameEn { get; set; } = string.Empty; // اسم الطالب الرباعي بالإنجليزية
        
        public string? Email { get; set; } // بريد الإلكتروني
        
        public string? Phone { get; set; } // رقم تواص الطالب
        
        public string? ParentName { get; set; } // اسم ولي الأمر
        
        public string? Grade { get; set; } // الصف الدراسي
        
        public string? EducationLevel { get; set; } // المرحلة الدراسية
        
        public string? Section { get; set; } // القسم التعليمي
        
        public string? AcademicYear { get; set; } // سنة الالتحاق
        
        // Multi-tenancy
        [Required]
        public int SchoolId { get; set; }
        public virtual School? School { get; set; }
    }
}

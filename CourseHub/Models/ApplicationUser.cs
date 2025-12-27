using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CourseHub.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    public string DisplayName { get; set; } = null!;

    public ICollection<Course> CoursesTaught { get; set; } = new HashSet<Course>();
    public ICollection<Enrollment> Enrollments { get; set; } = new HashSet<Enrollment>();
}

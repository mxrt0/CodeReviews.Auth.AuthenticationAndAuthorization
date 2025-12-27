using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseHub.Models;

public class Course
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    [Required]
    public string Description { get; set; } = null!;

    [ForeignKey(nameof(Instructor))]
    public string? InstructorId { get; set; }
    public ApplicationUser? Instructor { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; } = new HashSet<Enrollment>();
}

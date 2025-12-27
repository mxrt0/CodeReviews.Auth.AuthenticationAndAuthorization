namespace CourseHub.Models;

public class Enrollment
{
    public string StudentId { get; set; } = null!;
    public ApplicationUser Student { get; set; } = null!;

    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public DateTime EnrolledOn { get; set; } = DateTime.UtcNow;
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
}

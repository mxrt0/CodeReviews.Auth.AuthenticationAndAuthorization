using CourseHub.Models;

namespace CourseHub.ViewModels;

public class AllCoursesViewModel
{
    public List<Course> Courses { get; set; } = new();
    public List<ApplicationUser> Instructors { get; set; } = new();

    public string? SearchQuery { get; set; }
    public string? SelectedInstructorId { get; set; }
    public bool ShowOnlyMyCreated { get; set; }
    public bool ShowOnlyMyEnrolled { get; set; }
    public string? Sort { get; set; }
    public bool ShowInProgressOnly { get; set; }

}

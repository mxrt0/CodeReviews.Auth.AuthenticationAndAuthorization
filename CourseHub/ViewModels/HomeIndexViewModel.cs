using CourseHub.Models;

namespace CourseHub.ViewModels;

public class HomeIndexViewModel
{
    public List<Course> LatestCourses { get; set; } = new List<Course>();

    public List<Course> EnrolledCourses { get; set; } = new List<Course>();
}


using CourseHub.Models;

namespace CourseHub.Services.Contracts;

public interface ICourseService
{
    Task<List<Course>> GetAllCoursesAsync(bool includeEnrollments = false);
    Task<Course?> GetCourseByIdAsync(int id);
    Task<bool> EnrollStudentAsync(string studentId, int courseId);
    Task<List<Course>> GetCoursesByStudentAsync(string studentId);
    Task AddCourseAsync(string name, string description, string instructorId);
    Task DeleteCourseAsync(int id);
    Task EditCourseAsync(int id, string newTitle, string newDesc);
    Task<bool> IsUserEnrolledAsync(string userId, int courseId);
    Task<bool> UnenrollStudentAsync(string studentId, int courseId);
    Task<bool> MarkCourseCompletedAsync(string userId, int courseId);
}

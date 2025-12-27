using CourseHub.Data;
using CourseHub.Models;
using CourseHub.Services.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Services;

public class CourseService : ICourseService
{
    private readonly ApplicationDbContext _context;
    private readonly IDbLogger _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    public CourseService(ApplicationDbContext context, IDbLogger logger,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _logger = logger;
        _userManager = userManager;
    }

    public async Task AddCourseAsync(string name, string description, string instructorId)
    {
        try
        {
            var entry = new Course
            {
                Title = name,
                Description = description,
                CreatedAt = DateTime.UtcNow,
                InstructorId = instructorId
            };
            _context.Courses.Add(entry);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            await _logger.LogAsync(ex.Message, LogLevel.Error);
            throw;
        }
    }

    public async Task DeleteCourseAsync(int id)
    {
        var courseToRemove = await _context.Courses.FindAsync(id);
        if (courseToRemove is not null)
        {
            var enrollmentsToDelete = _context.Enrollments.Where(e => e.CourseId == id);
            _context.Courses.Remove(courseToRemove);
            await _context.SaveChangesAsync();

        }
    }

    public async Task EditCourseAsync(int id, string newTitle, string newDesc)
    {
        var course = await this.GetCourseByIdAsync(id);
        course!.Title = newTitle;
        course.Description = newDesc;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsUserEnrolledAsync(string userId, int courseId)
    {
        return await _context.Enrollments
        .AnyAsync(e => e.StudentId == userId && e.CourseId == courseId);
    }

    public async Task<bool> EnrollStudentAsync(string studentId, int courseId)
    {
        if (await IsUserEnrolledAsync(studentId, courseId)) return false;
        try
        {
            var entry = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId
            };
            await _context.Enrollments.AddAsync(entry);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            await _logger.LogAsync(ex.Message, LogLevel.Error);
            throw;
        }
    }

    public async Task<List<Course>> GetAllCoursesAsync(bool includeEnrollments = false)
    {
        try
        {
            return includeEnrollments ? await _context.Courses
                .Include(c => c.Enrollments)
                .ThenInclude(e => e.Student)
                .Include(c => c.Instructor)
                .ToListAsync()
                                  : await _context.Courses.Include(c => c.Instructor).ToListAsync();
        }
        catch (Exception ex)
        {
            await _logger.LogAsync(ex.Message, LogLevel.Error);
            throw;
        }
    }

    public async Task<Course?> GetCourseByIdAsync(int id)
    {
        return await _context.Courses.Include(c => c.Instructor).FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Course>> GetCoursesByStudentAsync(string studentId)
    {
        try
        {
            return await _context.Enrollments.
            Where(e => e.StudentId == studentId)
            .Include(e => e.Course)
            .Select(e => e.Course)
            .ToListAsync();
        }
        catch (Exception ex)
        {
            await _logger.LogAsync(ex.Message, LogLevel.Error);
            throw;
        }
    }

    public async Task<bool> UnenrollStudentAsync(string studentId, int courseId)
    {
        var enrollmentToRemove = await _context.Enrollments.FindAsync(studentId, courseId);
        if (enrollmentToRemove is null) return false;
        _context.Enrollments.Remove(enrollmentToRemove);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkCourseCompletedAsync(string userId, int courseId)
    {
        var enrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e =>
                e.StudentId == userId &&
                e.CourseId == courseId);

        if (enrollment == null || enrollment.IsCompleted)
            return false;

        enrollment.IsCompleted = true;
        enrollment.CompletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

}

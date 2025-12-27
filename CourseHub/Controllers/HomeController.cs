using CourseHub.Models;
using CourseHub.Services.Contracts;
using CourseHub.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CourseHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IUserService _userService;

        public HomeController(ICourseService courseService, IUserService userService)
        {
            _courseService = courseService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            List<Course>? enrolledCourses = null;
            string? userId = null;
            if (User.Identity?.IsAuthenticated ?? false)
            {
                userId = _userService.GetUserIdByClaimsPrincipal(User)!;
                enrolledCourses = await _courseService.GetCoursesByStudentAsync(userId);
            }

            var vm = new HomeIndexViewModel
            {
                LatestCourses = courses
                .Where(c => c.InstructorId != userId)
                .OrderByDescending(c => c.CreatedAt)
                .Take(5).ToList(),

                EnrolledCourses = enrolledCourses ?? new List<Course>()
            };
            return View(vm);
        }

    }
}

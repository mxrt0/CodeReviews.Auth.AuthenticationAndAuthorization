using CourseHub.Models;
using CourseHub.Services.Contracts;
using CourseHub.Utils;
using CourseHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourseHub.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IUserService _userService;
        public CoursesController(ICourseService courseService, IUserService userService)
        {
            _courseService = courseService;
            _userService = userService;
        }

        public async Task<IActionResult> AllCourses(string search, string instructorName, bool onlyEnrolled, string sort = "newest", bool myCreated = false, bool myEnrolled = false, bool showInProgressOnly = false)
        {
            try
            {
                IEnumerable<Course> courses = await _courseService.GetAllCoursesAsync(includeEnrollments: true);
                string? userId = _userService.GetUserIdByClaimsPrincipal(User);
                var instructors = await _userService.GetUsersInRoleAsync("Instructor");
                if (!myCreated && !myEnrolled)
                {
                    if (await _userService.IsInRoleAsync(userId, "Instructor"))
                    {
                        courses = courses.Where(c => c.InstructorId != userId);
                        instructors = instructors.Where(i => i.Id != userId);
                    }
                }
                if (!string.IsNullOrWhiteSpace(search))
                {
                    courses = courses.Where(c => c.Description.Contains(search, StringComparison.OrdinalIgnoreCase)
                                                || c.Title.Contains(search, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrEmpty(sort))
                {
                    courses = sort switch
                    {
                        "newest" => courses.OrderByDescending(c => c.CreatedAt).ToList(),
                        "oldest" => courses.OrderBy(c => c.CreatedAt).ToList(),
                        "recent" => courses = courses.OrderBy(c =>
                        {
                            var val = c.Enrollments.Select(e => e.CompletedAt).FirstOrDefault();
                            return val == null;
                        })
                        .ThenByDescending(c => c.Enrollments.Select(e => e.CompletedAt).FirstOrDefault()),
                        _ => courses
                    };
                }

                if (myCreated && userId is not null)
                {
                    courses = courses.Where(c => c.InstructorId == userId);
                }

                if (myEnrolled && userId is not null)
                {
                    var enrolled = await _courseService.GetCoursesByStudentAsync(userId);
                    courses = courses.Intersect(enrolled);
                    if (showInProgressOnly)
                        courses = courses.Where(c => c.Enrollments.Any(e => e.CompletedAt == null && e.StudentId == User.FindFirstValue(ClaimTypes.NameIdentifier)));
                }
                if (!string.IsNullOrWhiteSpace(instructorName) && !myCreated)
                {
                    courses = courses.Where(c => c.Instructor is not null && c.Instructor.DisplayName == instructorName);
                }

                var model = new AllCoursesViewModel
                {
                    Courses = onlyEnrolled ? courses.Where(c => c.Enrollments.Any(e => e.StudentId == _userService.GetUserIdByClaimsPrincipal(User))).ToList()
                                            : courses.ToList(),
                    Instructors = instructors.ToList(),
                    SearchQuery = search,
                    SelectedInstructorId = (await _userService.GetUsersInRoleAsync("Instructor"))?
                                            .FirstOrDefault(u => u.DisplayName == instructorName)?
                                            .Id ?? "N/A",
                    ShowOnlyMyCreated = myCreated,
                    ShowOnlyMyEnrolled = myEnrolled,
                    Sort = sort,
                    ShowInProgressOnly = showInProgressOnly,
                };
                return View(model);
            }
            catch (Exception ex)
            {
                var vm = new ErrorViewModel
                {
                    Title = "Retrieving Courses Failed",
                    Message = ex.Message
                };
                return View("~/Views/Shared/Error.cshtml", vm);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Instructor")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> Create(CreateCourseViewModel model)
        {
            try
            {
                if (!ModelState.IsValid) return View(model);

                var instructorId = _userService.GetUserIdByClaimsPrincipal(User);

                await _courseService.AddCourseAsync(model.Title, model.Description, instructorId!);

                TempData["SuccessMessage"] = Messages.SuccessfullyCreatedCourseMessage;
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                var vm = new ErrorViewModel
                {
                    Title = "Creation Failed",
                    Message = ex.Message
                };
                return View("~/Views/Shared/Error.cshtml", vm);
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Enroll(int id)
        {
            try
            {
                var course = await _courseService.GetCourseByIdAsync(id);
                if (course == null)
                    return NotFound();

                string? userId = _userService.GetUserIdByClaimsPrincipal(User);
                if (userId is null)
                {
                    return RedirectToPage("/Identity/Account/Login",
                            new { returnUrl = Url.Action("Enroll", "Courses", new { id }) });
                }

                if (await _courseService.IsUserEnrolledAsync(userId, course.Id))
                {
                    TempData["InfoMessage"] = Messages.AlreadyEnrolledMessage;
                    return RedirectToAction(nameof(AllCourses));
                }

                return View("RequestEnroll", course);
            }
            catch (Exception ex)
            {
                var vm = new ErrorViewModel
                {
                    Title = "/GET Enroll Failed",
                    Message = ex.Message
                };
                return View("~/Views/Shared/Error.cshtml", vm);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EnrollConfirmed(int id)
        {
            try
            {
                var course = await _courseService.GetCourseByIdAsync(id);
                string? userId = _userService.GetUserIdByClaimsPrincipal(User)!;
                if (userId is null)
                {
                    string returnUrl = Url.Action("Enroll", "Courses", new { id })!;
                    return RedirectToPage("/Identity/Account/Login", new { returnUrl });
                }
                if (course is null)
                {
                    return NotFound();
                }
                else if (course.InstructorId == userId)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    if (await _courseService.IsUserEnrolledAsync(userId, course.Id))
                    {
                        TempData["InfoMessage"] = Messages.AlreadyEnrolledMessage;
                        return RedirectToAction(nameof(AllCourses));
                    }
                    else
                    {
                        await _courseService.EnrollStudentAsync(userId, course.Id);
                    }
                }
                return RedirectToAction(nameof(Enrolled), new { id = course.Id });
            }
            catch (Exception ex)
            {
                var vm = new ErrorViewModel
                {
                    Title = "Enrollment Failed",
                    Message = ex.Message
                };
                return View("~/Views/Shared/Error.cshtml", vm);
            }
        }
        public async Task<IActionResult> Enrolled(int id)
        {
            try
            {
                var course = await _courseService.GetCourseByIdAsync(id);
                if (course is null)
                {
                    return NotFound();
                }
                return View(course);
            }
            catch (Exception ex)
            {
                var vm = new ErrorViewModel
                {
                    Title = "Displaying Enrolled Page Failed",
                    Message = ex.Message
                };
                return View("~/Views/Shared/Error.cshtml", vm);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var course = await _courseService.GetCourseByIdAsync(id);
                if (course is null)
                {
                    return NotFound();
                }
                if (course.InstructorId != _userService.GetUserIdByClaimsPrincipal(User))
                {
                    return Unauthorized();
                }
                return View(course);
            }
            catch (Exception ex)
            {
                var vm = new ErrorViewModel
                {
                    Title = "Edit Failed",
                    Message = ex.Message
                };
                return View("~/Views/Shared/Error.cshtml", vm);
            }
        }
        [HttpPost]
        [Authorize(Roles = "Instructor")]
        [ActionName("Edit")]
        public async Task<IActionResult> EditConfirmed(int id, Course newCourse)
        {
            try
            {
                var course = await _courseService.GetCourseByIdAsync(id);
                if (!ModelState.IsValid)
                {
                    return View(course);
                }
                if (course is null)
                {
                    return NotFound();
                }
                if (course.InstructorId != _userService.GetUserIdByClaimsPrincipal(User))
                {
                    return Unauthorized();
                }
                await _courseService.EditCourseAsync(id, newCourse.Title, newCourse.Description);
                TempData["InfoMessage"] = Messages.SuccessfullyEditedCourseMessage;
                return RedirectToAction(nameof(AllCourses), new { myCreated = true });
            }
            catch (Exception ex)
            {
                var vm = new ErrorViewModel
                {
                    Title = "Edit Failed",
                    Message = ex.Message
                };
                return View("~/Views/Shared/Error.cshtml", vm);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Unenroll(int id)
        {
            try
            {
                var userId = _userService.GetUserIdByClaimsPrincipal(User)!;

                if (!await _courseService.IsUserEnrolledAsync(userId, id))
                {
                    TempData["InfoMessage"] = Messages.AlreadyUnenrolledMessage;
                    return RedirectToAction(nameof(AllCourses), new { myEnrolled = true });
                }

                await _courseService.UnenrollStudentAsync(userId, id);
                TempData["InfoMessage"] = Messages.SuccessfullyUnenrolledMessage;
                return RedirectToAction(nameof(AllCourses), new { myEnrolled = true });
            }
            catch (Exception ex)
            {
                var vm = new ErrorViewModel
                {
                    Title = "Unenrollment Failed",
                    Message = ex.Message
                };
                return View("~/Views/Shared/Error.cshtml", vm);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _courseService.DeleteCourseAsync(id);
                TempData["InfoMessage"] = Messages.SuccessfullyDeletedCourseMessage;
                return RedirectToAction(nameof(AllCourses), new { myCreated = true });
            }
            catch (Exception ex)
            {
                var vm = new ErrorViewModel
                {
                    Title = "Deletion Failed",
                    Message = ex.Message
                };
                return View("~/Views/Shared/Error.cshtml", vm);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> MarkCompleted(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

                var success = await _courseService.MarkCourseCompletedAsync(userId, id);

                if (!success)
                    return BadRequest();

                TempData["InfoMessage"] = Messages.CourseCompletedMessage;
                return RedirectToAction(nameof(AllCourses), new { myEnrolled = true });
            }
            catch (Exception ex)
            {
                var vm = new ErrorViewModel
                {
                    Title = "Marking as Completed Failed",
                    Message = ex.Message
                };
                return View("~/Views/Shared/Error.cshtml", vm);
            }
        }

    }
}

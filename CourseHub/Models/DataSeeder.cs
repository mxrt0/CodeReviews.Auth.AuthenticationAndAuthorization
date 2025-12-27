using CourseHub.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Models
{
    public static class DataSeeder
    {
        private const string instructorRole = "Instructor";
        private static readonly (string Email, string Password, string Name)[] InstructorData = new[]
        {
            ("john.smith@academy.com", "Test123!", "John Smith"),
            ("emma.jones@academy.com", "Test123!", "Emma Jones"),
            ("luis.martin@academy.com", "Test123!", "Luis Martin")
        };

        private static readonly (string Title, string Description, int DaysAgo)[] CourseData = new[]
        {
            ("Introduction to C#", "Learn the basics of C# programming from scratch.", 10),
            ("ASP.NET Core for Beginners", "Build your first web apps using ASP.NET Core MVC and Razor Pages.", 8),
            ("Entity Framework Core Deep Dive", "Master EF Core with hands-on examples and database modeling techniques.", 5),
            ("Frontend Basics: HTML & CSS", "Understand HTML structure, CSS styling, and responsive design principles.", 3),
            ("JavaScript Essentials", "Learn modern JavaScript features and DOM manipulation.", 1)
        };
        public static async Task SeedAsync(ApplicationDbContext db,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {

            var instructors = new List<ApplicationUser>();
            foreach (var (email, password, name) in InstructorData)
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        DisplayName = email.Substring(0, email.IndexOf('@')),
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(user, password);
                    await userManager.AddToRoleAsync(user, instructorRole);
                }
                instructors.Add(user);
            }


            if (!await db.Courses.AnyAsync())
            {
                var courses = new List<Course>();
                int instructorIndex = 0;

                foreach (var (title, description, daysAgo) in CourseData)
                {
                    var course = new Course
                    {
                        Title = title,
                        Description = description,
                        CreatedAt = DateTime.UtcNow.AddDays(-daysAgo),
                        InstructorId = instructors[instructorIndex].Id
                    };

                    courses.Add(course);
                    instructorIndex = (instructorIndex + 1) % instructors.Count;
                }

                await db.Courses.AddRangeAsync(courses);
                await db.SaveChangesAsync();
            }
        }
    }
}

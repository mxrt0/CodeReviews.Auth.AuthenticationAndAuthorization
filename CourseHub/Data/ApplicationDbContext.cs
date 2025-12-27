using CourseHub.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace CourseHub.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Log> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Enrollment>()
                .HasKey(e => new { e.StudentId, e.CourseId });

            builder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId);

            builder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(u => u.CoursesTaught)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Course>().HasData(
                new Course
                {
                    Id = 1,
                    Title = "Introduction to C#",
                    Description = "Learn the basics of C# programming from scratch.",
                    InstructorId = null,
                    CreatedAt = DateTime.UtcNow.AddDays(-10)
                },
                new Course
                {
                    Id = 2,
                    Title = "ASP.NET Core for Beginners",
                    Description = "Build your first web apps using ASP.NET Core MVC and Razor Pages.",
                    InstructorId = null,
                    CreatedAt = DateTime.UtcNow.AddDays(-8)
                },
                new Course
                {
                    Id = 3,
                    Title = "Entity Framework Core Deep Dive",
                    Description = "Master EF Core with hands-on examples and database modeling techniques.",
                    InstructorId = null,
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                },
                new Course
                {
                    Id = 4,
                    Title = "Frontend Basics: HTML & CSS",
                    Description = "Understand HTML structure, CSS styling, and responsive design principles.",
                    InstructorId = null,
                    CreatedAt = DateTime.UtcNow.AddDays(-3)
                },
                new Course
                {
                    Id = 5,
                    Title = "JavaScript Essentials",
                    Description = "Learn modern JavaScript features and DOM manipulation.",
                    InstructorId = null,
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                }
    );
        }
    }
}


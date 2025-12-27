using Microsoft.AspNetCore.Identity;

namespace CourseHub.Models;

public class RoleSeeder
{
    public static readonly string[] roles = { "Instructor", "Student" };
    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

    }
}

using CourseHub.Models;
using System.Security.Claims;

namespace CourseHub.Services.Contracts;

public interface IUserService
{
    string? GetUserIdByClaimsPrincipal(ClaimsPrincipal principal);
    Task<IEnumerable<ApplicationUser>> GetUsersInRoleAsync(string role);
    Task<bool> IsInRoleAsync(string? userId, string role);
}

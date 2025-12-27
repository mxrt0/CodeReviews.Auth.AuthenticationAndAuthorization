using CourseHub.Data;
using CourseHub.Models;
using CourseHub.Services.Contracts;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
namespace CourseHub.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _context;
    private readonly IDbLogger _logger;

    public UserService(UserManager<ApplicationUser> userManager,
                       RoleManager<IdentityRole> roleManager,
                       SignInManager<ApplicationUser> signInManager,
                       ApplicationDbContext context,
                       IDbLogger logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<ApplicationUser>> GetUsersInRoleAsync(string role)
    {
        try
        {
            return (await _userManager.GetUsersInRoleAsync(role)).ToList();
        }
        catch (Exception ex)
        {
            await _logger.LogAsync(ex.Message, LogLevel.Error);
            throw;
        }
    }

    public string? GetUserIdByClaimsPrincipal(ClaimsPrincipal principal)
    {
        return _userManager.GetUserId(principal);

    }

    public async Task<bool> IsInRoleAsync(string? userId, string role)
    {
        ApplicationUser? user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return false;
        }
        return await _userManager.IsInRoleAsync(user, role);
    }
}

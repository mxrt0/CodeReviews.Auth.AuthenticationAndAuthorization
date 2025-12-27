using CourseHub.Data;
using CourseHub.Models;
using CourseHub.Services.Contracts;

namespace CourseHub.Services;

public class DbLogger : IDbLogger
{
    private readonly ApplicationDbContext _context;
    public DbLogger(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task LogAsync(string message = "", LogLevel severity = LogLevel.Information)
    {
        var logEntry = new Log
        {
            CreatedAt = DateTime.UtcNow,
            Message = message,
            Severity = severity
        };

        await _context.Logs.AddAsync(logEntry);
        await _context.SaveChangesAsync();
    }
}

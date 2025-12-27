using System.ComponentModel.DataAnnotations;

namespace CourseHub.Models;

public class Log
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }

    [Required]
    public string Message { get; set; } = null!;
    public LogLevel Severity { get; set; }
}

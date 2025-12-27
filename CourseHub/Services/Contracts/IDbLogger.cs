namespace CourseHub.Services.Contracts;

public interface IDbLogger
{
    public Task LogAsync(string message, LogLevel severity);
}

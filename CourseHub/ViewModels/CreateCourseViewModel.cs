using System.ComponentModel.DataAnnotations;

namespace CourseHub.ViewModels;

public class CreateCourseViewModel
{
    [Required]
    [StringLength(100, MinimumLength = 5)]
    public string Title { get; set; } = null!;

    [Required]
    [StringLength(500, MinimumLength = 70)]
    public string Description { get; set; } = null!;
}

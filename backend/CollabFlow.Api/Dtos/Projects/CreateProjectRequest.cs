using System.ComponentModel.DataAnnotations;

namespace CollabFlow.Api.Dtos.Projects;

public class CreateProjectRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime? Deadline { get; set; }
}

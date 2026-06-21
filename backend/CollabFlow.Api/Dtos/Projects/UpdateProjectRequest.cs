using CollabFlow.Api.Models;

namespace CollabFlow.Api.Dtos.Projects;

public class UpdateProjectRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public ProjectStatus? Status { get; set; }
    public DateTime? Deadline { get; set; }
}

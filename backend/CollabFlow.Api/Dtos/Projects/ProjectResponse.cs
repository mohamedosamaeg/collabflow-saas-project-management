using CollabFlow.Api.Models;

namespace CollabFlow.Api.Dtos.Projects;

public class ProjectResponse
{
    public Guid Id { get; set; }
    public Guid WorkspaceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ProjectStatus Status { get; set; }
    public Guid CreatedById { get; set; }
    public DateTime? Deadline { get; set; }
    public DateTime CreatedAt { get; set; }
    public int TaskCount { get; set; }
}

using CollabFlow.Api.Models;

namespace CollabFlow.Api.Dtos.Workspaces;

public class WorkspaceResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public WorkspaceRole CurrentUserRole { get; set; }
    public List<WorkspaceMemberResponse> Members { get; set; } = new();
}

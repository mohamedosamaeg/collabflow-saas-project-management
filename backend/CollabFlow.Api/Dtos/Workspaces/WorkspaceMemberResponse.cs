using CollabFlow.Api.Models;

namespace CollabFlow.Api.Dtos.Workspaces;

public class WorkspaceMemberResponse
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public WorkspaceRole Role { get; set; }
    public DateTime JoinedAt { get; set; }
}

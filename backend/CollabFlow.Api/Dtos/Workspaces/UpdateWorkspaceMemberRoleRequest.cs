using CollabFlow.Api.Models;

namespace CollabFlow.Api.Dtos.Workspaces;

public class UpdateWorkspaceMemberRoleRequest
{
    public WorkspaceRole Role { get; set; } = WorkspaceRole.Member;
}

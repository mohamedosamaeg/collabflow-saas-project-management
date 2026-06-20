namespace CollabFlow.Api.Models;

public class WorkspaceMember
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid WorkspaceId { get; set; }
    public Guid UserId { get; set; }
    public WorkspaceRole Role { get; set; } = WorkspaceRole.Member;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public Workspace Workspace { get; set; } = null!;
    public User User { get; set; } = null!;
}

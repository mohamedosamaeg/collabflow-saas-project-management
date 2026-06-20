namespace CollabFlow.Api.Models;

public class ActivityLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid WorkspaceId { get; set; }
    public Guid UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public ActivityEntityType EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public string MetadataJson { get; set; } = "{}";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Workspace Workspace { get; set; } = null!;
    public User User { get; set; } = null!;
}

namespace CollabFlow.Api.Models;

public class Comment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TaskItemId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public TaskItem TaskItem { get; set; } = null!;
    public User User { get; set; } = null!;
}

namespace CollabFlow.Api.Models;

public enum WorkspaceRole
{
    Admin,
    Manager,
    Member
}

public enum ProjectStatus
{
    Active,
    Completed,
    Archived
}

public enum TaskItemStatus
{
    Todo,
    InProgress,
    Review,
    Done
}

public enum TaskPriority
{
    Low,
    Medium,
    High,
    Urgent
}

public enum NotificationType
{
    TaskAssigned,
    TaskStatusChanged,
    CommentAdded,
    FileUploaded,
    MemberAdded
}

public enum ActivityEntityType
{
    Workspace,
    Project,
    Task,
    Comment,
    File,
    Member
}

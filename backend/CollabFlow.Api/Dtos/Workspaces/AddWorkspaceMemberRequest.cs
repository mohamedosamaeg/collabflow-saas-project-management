using System.ComponentModel.DataAnnotations;
using CollabFlow.Api.Models;

namespace CollabFlow.Api.Dtos.Workspaces;

public class AddWorkspaceMemberRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public WorkspaceRole Role { get; set; } = WorkspaceRole.Member;
}

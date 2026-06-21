using System.ComponentModel.DataAnnotations;

namespace CollabFlow.Api.Dtos.Workspaces;

public class CreateWorkspaceRequest
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;
}

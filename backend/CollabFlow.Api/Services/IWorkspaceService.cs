using CollabFlow.Api.Dtos.Workspaces;

namespace CollabFlow.Api.Services;

public interface IWorkspaceService
{
    Task<WorkspaceResponse> CreateAsync(Guid userId, CreateWorkspaceRequest request);
    Task<List<WorkspaceResponse>> GetMyWorkspacesAsync(Guid userId);
    Task<WorkspaceResponse> GetByIdAsync(Guid userId, Guid workspaceId);
    Task<WorkspaceResponse> AddMemberAsync(Guid adminUserId, Guid workspaceId, AddWorkspaceMemberRequest request);
    Task<WorkspaceResponse> UpdateMemberRoleAsync(Guid adminUserId, Guid workspaceId, Guid memberUserId, UpdateWorkspaceMemberRoleRequest request);
    Task RemoveMemberAsync(Guid adminUserId, Guid workspaceId, Guid memberUserId);
}

using CollabFlow.Api.Dtos.Projects;

namespace CollabFlow.Api.Services;

public interface IProjectService
{
    Task<ProjectResponse> CreateAsync(Guid userId, Guid workspaceId, CreateProjectRequest request);
    Task<List<ProjectResponse>> GetWorkspaceProjectsAsync(Guid userId, Guid workspaceId);
    Task<ProjectResponse> GetByIdAsync(Guid userId, Guid projectId);
    Task<ProjectResponse> UpdateAsync(Guid userId, Guid projectId, UpdateProjectRequest request);
    Task ArchiveAsync(Guid userId, Guid projectId);
}

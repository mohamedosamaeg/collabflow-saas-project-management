using CollabFlow.Api.Data;
using CollabFlow.Api.Dtos.Projects;
using CollabFlow.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CollabFlow.Api.Services;

public class ProjectService(AppDbContext context) : IProjectService
{
    public async Task<ProjectResponse> CreateAsync(Guid userId, Guid workspaceId, CreateProjectRequest request)
    {
        await EnsureCanManageProjectsAsync(userId, workspaceId);

        var project = new Project
        {
            WorkspaceId = workspaceId,
            Name = request.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            Deadline = NormalizeUtc(request.Deadline),
            CreatedById = userId
        };

        context.Projects.Add(project);
        await context.SaveChangesAsync();

        return await GetByIdAsync(userId, project.Id);
    }

    public async Task<List<ProjectResponse>> GetWorkspaceProjectsAsync(Guid userId, Guid workspaceId)
    {
        await EnsureWorkspaceAccessAsync(userId, workspaceId);

        var projects = await context.Projects
            .Include(x => x.Tasks)
            .Where(x => x.WorkspaceId == workspaceId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return projects.Select(MapProject).ToList();
    }

    public async Task<ProjectResponse> GetByIdAsync(Guid userId, Guid projectId)
    {
        var project = await context.Projects
            .Include(x => x.Tasks)
            .FirstOrDefaultAsync(x => x.Id == projectId);

        if (project is null)
        {
            throw new KeyNotFoundException("Project not found.");
        }

        await EnsureWorkspaceAccessAsync(userId, project.WorkspaceId);

        return MapProject(project);
    }

    public async Task<ProjectResponse> UpdateAsync(Guid userId, Guid projectId, UpdateProjectRequest request)
    {
        var project = await context.Projects.FirstOrDefaultAsync(x => x.Id == projectId);

        if (project is null)
        {
            throw new KeyNotFoundException("Project not found.");
        }

        await EnsureCanManageProjectsAsync(userId, project.WorkspaceId);

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            project.Name = request.Name.Trim();
        }

        if (request.Description is not null)
        {
            project.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        }

        if (request.Status is not null)
        {
            project.Status = request.Status.Value;
        }

        if (request.Deadline is not null)
        {
            project.Deadline = NormalizeUtc(request.Deadline);
        }

        await context.SaveChangesAsync();

        return await GetByIdAsync(userId, project.Id);
    }

    public async Task ArchiveAsync(Guid userId, Guid projectId)
    {
        var project = await context.Projects.FirstOrDefaultAsync(x => x.Id == projectId);

        if (project is null)
        {
            throw new KeyNotFoundException("Project not found.");
        }

        await EnsureCanManageProjectsAsync(userId, project.WorkspaceId);

        project.Status = ProjectStatus.Archived;
        await context.SaveChangesAsync();
    }

    private async Task<WorkspaceRole> EnsureWorkspaceAccessAsync(Guid userId, Guid workspaceId)
    {
        var member = await context.WorkspaceMembers
            .FirstOrDefaultAsync(x => x.WorkspaceId == workspaceId && x.UserId == userId);

        if (member is null)
        {
            throw new UnauthorizedAccessException("You do not have access to this workspace.");
        }

        return member.Role;
    }

    private async Task EnsureCanManageProjectsAsync(Guid userId, Guid workspaceId)
    {
        var role = await EnsureWorkspaceAccessAsync(userId, workspaceId);

        if (role is not WorkspaceRole.Admin and not WorkspaceRole.Manager)
        {
            throw new UnauthorizedAccessException("Only admins and managers can manage projects.");
        }
    }

    private static DateTime? NormalizeUtc(DateTime? dateTime)
    {
        if (dateTime is null)
        {
            return null;
        }

        return dateTime.Value.Kind switch
        {
            DateTimeKind.Utc => dateTime.Value,
            DateTimeKind.Local => dateTime.Value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc)
        };
    }

    private static ProjectResponse MapProject(Project project)
    {
        return new ProjectResponse
        {
            Id = project.Id,
            WorkspaceId = project.WorkspaceId,
            Name = project.Name,
            Description = project.Description,
            Status = project.Status,
            CreatedById = project.CreatedById,
            Deadline = project.Deadline,
            CreatedAt = project.CreatedAt,
            TaskCount = project.Tasks.Count
        };
    }
}

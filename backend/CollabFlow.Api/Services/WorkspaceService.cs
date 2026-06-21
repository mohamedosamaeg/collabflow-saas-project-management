using CollabFlow.Api.Data;
using CollabFlow.Api.Dtos.Workspaces;
using CollabFlow.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CollabFlow.Api.Services;

public class WorkspaceService(AppDbContext context) : IWorkspaceService
{
    public async Task<WorkspaceResponse> CreateAsync(Guid userId, CreateWorkspaceRequest request)
    {
        var userExists = await context.Users.AnyAsync(x => x.Id == userId);

        if (!userExists)
        {
            throw new UnauthorizedAccessException("User not found.");
        }

        var workspace = new Workspace
        {
            Name = request.Name.Trim(),
            OwnerId = userId
        };

        workspace.Members.Add(new WorkspaceMember
        {
            UserId = userId,
            Role = WorkspaceRole.Admin
        });

        context.Workspaces.Add(workspace);
        await context.SaveChangesAsync();

        return await GetByIdAsync(userId, workspace.Id);
    }

    public async Task<List<WorkspaceResponse>> GetMyWorkspacesAsync(Guid userId)
    {
        var workspaceIds = await context.WorkspaceMembers
            .Where(x => x.UserId == userId)
            .Select(x => x.WorkspaceId)
            .ToListAsync();

        var workspaces = await context.Workspaces
            .Include(x => x.Members)
            .ThenInclude(x => x.User)
            .Where(x => workspaceIds.Contains(x.Id))
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return workspaces.Select(x => MapWorkspace(x, userId)).ToList();
    }

    public async Task<WorkspaceResponse> GetByIdAsync(Guid userId, Guid workspaceId)
    {
        var workspace = await context.Workspaces
            .Include(x => x.Members)
            .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == workspaceId);

        if (workspace is null)
        {
            throw new KeyNotFoundException("Workspace not found.");
        }

        var isMember = workspace.Members.Any(x => x.UserId == userId);

        if (!isMember)
        {
            throw new UnauthorizedAccessException("You do not have access to this workspace.");
        }

        return MapWorkspace(workspace, userId);
    }

    public async Task<WorkspaceResponse> AddMemberAsync(Guid adminUserId, Guid workspaceId, AddWorkspaceMemberRequest request)
    {
        await EnsureAdminAsync(adminUserId, workspaceId);

        var email = request.Email.Trim().ToLowerInvariant();

        var user = await context.Users.FirstOrDefaultAsync(x => x.Email == email);

        if (user is null)
        {
            throw new KeyNotFoundException("User with this email was not found.");
        }

        var alreadyMember = await context.WorkspaceMembers
            .AnyAsync(x => x.WorkspaceId == workspaceId && x.UserId == user.Id);

        if (alreadyMember)
        {
            throw new InvalidOperationException("User is already a member of this workspace.");
        }

        context.WorkspaceMembers.Add(new WorkspaceMember
        {
            WorkspaceId = workspaceId,
            UserId = user.Id,
            Role = request.Role
        });

        await context.SaveChangesAsync();

        return await GetByIdAsync(adminUserId, workspaceId);
    }

    public async Task<WorkspaceResponse> UpdateMemberRoleAsync(Guid adminUserId, Guid workspaceId, Guid memberUserId, UpdateWorkspaceMemberRoleRequest request)
    {
        await EnsureAdminAsync(adminUserId, workspaceId);

        var workspace = await context.Workspaces.FirstOrDefaultAsync(x => x.Id == workspaceId);

        if (workspace is null)
        {
            throw new KeyNotFoundException("Workspace not found.");
        }

        if (workspace.OwnerId == memberUserId)
        {
            throw new InvalidOperationException("Cannot change the role of the workspace owner.");
        }

        var member = await context.WorkspaceMembers
            .FirstOrDefaultAsync(x => x.WorkspaceId == workspaceId && x.UserId == memberUserId);

        if (member is null)
        {
            throw new KeyNotFoundException("Member not found.");
        }

        member.Role = request.Role;
        await context.SaveChangesAsync();

        return await GetByIdAsync(adminUserId, workspaceId);
    }

    public async Task RemoveMemberAsync(Guid adminUserId, Guid workspaceId, Guid memberUserId)
    {
        await EnsureAdminAsync(adminUserId, workspaceId);

        var workspace = await context.Workspaces.FirstOrDefaultAsync(x => x.Id == workspaceId);

        if (workspace is null)
        {
            throw new KeyNotFoundException("Workspace not found.");
        }

        if (workspace.OwnerId == memberUserId)
        {
            throw new InvalidOperationException("Cannot remove the workspace owner.");
        }

        var member = await context.WorkspaceMembers
            .FirstOrDefaultAsync(x => x.WorkspaceId == workspaceId && x.UserId == memberUserId);

        if (member is null)
        {
            throw new KeyNotFoundException("Member not found.");
        }

        context.WorkspaceMembers.Remove(member);
        await context.SaveChangesAsync();
    }

    private async Task EnsureAdminAsync(Guid userId, Guid workspaceId)
    {
        var member = await context.WorkspaceMembers
            .FirstOrDefaultAsync(x => x.WorkspaceId == workspaceId && x.UserId == userId);

        if (member is null || member.Role != WorkspaceRole.Admin)
        {
            throw new UnauthorizedAccessException("Only workspace admins can perform this action.");
        }
    }

    private static WorkspaceResponse MapWorkspace(Workspace workspace, Guid currentUserId)
    {
        var currentUserRole = workspace.Members
            .First(x => x.UserId == currentUserId)
            .Role;

        return new WorkspaceResponse
        {
            Id = workspace.Id,
            Name = workspace.Name,
            OwnerId = workspace.OwnerId,
            CreatedAt = workspace.CreatedAt,
            CurrentUserRole = currentUserRole,
            Members = workspace.Members
                .OrderBy(x => x.JoinedAt)
                .Select(x => new WorkspaceMemberResponse
                {
                    UserId = x.UserId,
                    FullName = x.User.FullName,
                    Email = x.User.Email,
                    Role = x.Role,
                    JoinedAt = x.JoinedAt
                })
                .ToList()
        };
    }
}

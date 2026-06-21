using CollabFlow.Api.Dtos.Workspaces;
using CollabFlow.Api.Extensions;
using CollabFlow.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollabFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class WorkspacesController(IWorkspaceService workspaceService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<WorkspaceResponse>> Create(CreateWorkspaceRequest request)
    {
        var userId = User.GetUserId();
        var response = await workspaceService.CreateAsync(userId, request);
        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<List<WorkspaceResponse>>> GetMyWorkspaces()
    {
        var userId = User.GetUserId();
        var response = await workspaceService.GetMyWorkspacesAsync(userId);
        return Ok(response);
    }

    [HttpGet("{workspaceId:guid}")]
    public async Task<ActionResult<WorkspaceResponse>> GetById(Guid workspaceId)
    {
        try
        {
            var userId = User.GetUserId();
            var response = await workspaceService.GetByIdAsync(userId, workspaceId);
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    [HttpPost("{workspaceId:guid}/members")]
    public async Task<ActionResult<WorkspaceResponse>> AddMember(Guid workspaceId, AddWorkspaceMemberRequest request)
    {
        try
        {
            var userId = User.GetUserId();
            var response = await workspaceService.AddMemberAsync(userId, workspaceId, request);
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    [HttpPut("{workspaceId:guid}/members/{memberUserId:guid}/role")]
    public async Task<ActionResult<WorkspaceResponse>> UpdateMemberRole(Guid workspaceId, Guid memberUserId, UpdateWorkspaceMemberRoleRequest request)
    {
        try
        {
            var userId = User.GetUserId();
            var response = await workspaceService.UpdateMemberRoleAsync(userId, workspaceId, memberUserId, request);
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    [HttpDelete("{workspaceId:guid}/members/{memberUserId:guid}")]
    public async Task<IActionResult> RemoveMember(Guid workspaceId, Guid memberUserId)
    {
        try
        {
            var userId = User.GetUserId();
            await workspaceService.RemoveMemberAsync(userId, workspaceId, memberUserId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }
}

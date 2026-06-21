using CollabFlow.Api.Dtos.Projects;
using CollabFlow.Api.Extensions;
using CollabFlow.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollabFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api")]
public class ProjectsController(IProjectService projectService) : ControllerBase
{
    [HttpPost("workspaces/{workspaceId:guid}/projects")]
    public async Task<ActionResult<ProjectResponse>> Create(Guid workspaceId, CreateProjectRequest request)
    {
        try
        {
            var userId = User.GetUserId();
            var response = await projectService.CreateAsync(userId, workspaceId, request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    [HttpGet("workspaces/{workspaceId:guid}/projects")]
    public async Task<ActionResult<List<ProjectResponse>>> GetWorkspaceProjects(Guid workspaceId)
    {
        try
        {
            var userId = User.GetUserId();
            var response = await projectService.GetWorkspaceProjectsAsync(userId, workspaceId);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    [HttpGet("projects/{projectId:guid}")]
    public async Task<ActionResult<ProjectResponse>> GetById(Guid projectId)
    {
        try
        {
            var userId = User.GetUserId();
            var response = await projectService.GetByIdAsync(userId, projectId);
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

    [HttpPut("projects/{projectId:guid}")]
    public async Task<ActionResult<ProjectResponse>> Update(Guid projectId, UpdateProjectRequest request)
    {
        try
        {
            var userId = User.GetUserId();
            var response = await projectService.UpdateAsync(userId, projectId, request);
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

    [HttpDelete("projects/{projectId:guid}")]
    public async Task<IActionResult> Archive(Guid projectId)
    {
        try
        {
            var userId = User.GetUserId();
            await projectService.ArchiveAsync(userId, projectId);
            return NoContent();
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
}

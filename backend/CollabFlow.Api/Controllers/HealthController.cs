using Microsoft.AspNetCore.Mvc;

namespace CollabFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "ok",
            app = "CollabFlow.Api",
            time = DateTime.UtcNow
        });
    }
}

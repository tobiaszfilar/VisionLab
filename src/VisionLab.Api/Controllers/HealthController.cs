using Microsoft.AspNetCore.Mvc;

namespace VisionLab.Api.Controllers;

[ApiController]
[Route("api/health")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "OK"
        });
    }
}
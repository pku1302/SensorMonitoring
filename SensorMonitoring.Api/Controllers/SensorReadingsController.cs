using Microsoft.AspNetCore.Mvc;
using SensorMonitoring.Application.Abstractions;
using SensorMonitoring.Contracts;

namespace SensorMonitoring.Api.Controllers;

[ApiController]
[Route("api/sensors/{deviceId}/readings")]
public class SensorReadingsController : ControllerBase
{
    private readonly ISensorReadingService _service;

    public SensorReadingsController(
        ISensorReadingService service)
    {
        _service = service;
    }

    [HttpGet("recent")]
    public async Task<ActionResult<List<SensorReadingDto>>> GetRecent(
        string deviceId,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.GetRecentAsync(
                deviceId,
                cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }
}

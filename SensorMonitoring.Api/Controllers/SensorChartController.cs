using Microsoft.AspNetCore.Mvc;
using SensorMonitoring.Api.Services;
using SensorMonitoring.Application.Services;
using SensorMonitoring.Contracts;

namespace SensorMonitoring.Api.Controllers;

[ApiController]
[Route("api/sensors/{deviceId}/chart")]
public class SensorChartController : ControllerBase
{
    private readonly SensorTrendService _service;

    public SensorChartController(
        SensorTrendService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<SensorTrendDto>>> Get(
        string deviceId,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.GetAsync(
                deviceId,
                cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("/api/sensors/charts")]
    public async Task<IActionResult> GetAll(
    CancellationToken cancellationToken)
    {
        var result = await _service.GetAllAsync(
            cancellationToken);

        return Ok(result);
    }
}

using Microsoft.AspNetCore.Mvc;
using SensorMonitoring.Application.Services;
using SensorMonitoring.Contracts;

namespace SensorMonitoring.Api.Controllers;

[ApiController]
[Route("api/sensors")]
public class SensorHistoryController : ControllerBase
{
    private readonly SensorHistoryService _historyService;

    public SensorHistoryController(
        SensorHistoryService historyService)
    {
        _historyService = historyService;
    }

    [HttpGet("with-readings")]
    public async Task<ActionResult<List<SensorListItemDto>>>
        GetSensorsWithReadings(
            [FromQuery] DateOnly date,
            CancellationToken cancellationToken)
    {
        var sensors = await _historyService.GetSensorsByDateAsync(
            date,
            cancellationToken);

        return Ok(sensors);
    }

    [HttpGet("{deviceId}/readings")]
    public async Task<ActionResult<List<SensorReadingDto>>> GetReadings(
        [FromRoute] string deviceId,
        [FromQuery] DateOnly date,
        CancellationToken cancellationToken)
    {
        var readings = await _historyService.GetReadingsByDateAsync(
            deviceId,
            date,
            cancellationToken);

        if (readings is null)
            return NotFound();

        return Ok(readings);
    }

    [HttpGet("{deviceId}/history/chart")]
    public async Task<ActionResult<SensorHistoryChartDto>>
        GetChart(
            [FromRoute] string deviceId,
            [FromQuery] DateOnly date,
            CancellationToken cancellationToken)
    {
        var result = await _historyService.GetChartByDateAsync(
            deviceId,
            date,
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("history/readings")]
    public async Task<ActionResult<List<SensorHistoryReadingDto>>>
        GetHourlyReadingsAsync(
            [FromQuery] DateOnly date,
            [FromQuery] int hour,
            [FromQuery] string? deviceId,
            CancellationToken cancellationToken)
    {
        if (hour is < 0 or > 23)
        {
            return BadRequest(
                "hour는 0부터 23까지 입력해야 합니다.");
        }

        var readings = await _historyService.GetHourlyReadingAsync(
            date,
            hour,
            deviceId,
            cancellationToken);

        return Ok(readings);
    }
}

using Microsoft.AspNetCore.Mvc;
using SensorMonitoring.Api.Services;
using SensorMonitoring.Contracts;

namespace SensorMonitoring.Api.Controllers;

[ApiController]
[Route("api/alarms")]
public class SensorAlarmController : ControllerBase
{
    private readonly SensorAlarmService _alarmService;

    public SensorAlarmController(SensorAlarmService alarmService)
    {
        _alarmService = alarmService;
    }

    [HttpGet("active")]
    public IActionResult GetActive()
    {
        return Ok(_alarmService.GetActive());
    }
}

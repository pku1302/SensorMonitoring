using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SensorMonitoring.Api.Hubs;
using SensorMonitoring.Application.Abstractions;
using SensorMonitoring.Contracts;

namespace SensorMonitoring.Api.Services;

public class SensorRealtimeService : ISensorRealtimeNotifier
{
    private readonly IHubContext<SensorHub> _hubContext;

    public SensorRealtimeService(
        IHubContext<SensorHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task SendSensorUpdatedAsync(
        SensorRealTimeDto dto,
        CancellationToken cancellationToken)
    {
        return _hubContext.Clients.All.SendAsync(
            "SensorUpdated",
            dto,
            cancellationToken);
    }

    public Task SendAlarmChangedAsync(
        SensorAlarmChangedDto dto,
        CancellationToken cancellationToken)
    {
        return _hubContext.Clients.All.SendAsync(
            "AlarmChanged",
            dto,
            cancellationToken);
    }
}

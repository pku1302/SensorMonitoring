using SensorMonitoring.Application.Abstractions;
using SensorMonitoring.Contracts;

namespace SensorMonitoring.Api.Services;

public class SensorChartCache : ISensorChartCache
{
    private const int MaxCount = 180;
    private readonly Dictionary<string, Queue<SensorChartPointDto>> _queues
        = new();
    private readonly object _lock = new();
    
    public void Add (SensorPacket packet)
    {
        lock (_lock)
        {
            var deviceId = packet.DeviceId;

            var point = new SensorChartPointDto
            {
                Temperature = packet.Temperature,

                Humidity = packet.Humidity,

                Timestamp = packet.Timestamp
            };

            if (!_queues.TryGetValue(deviceId, out var queue))
            {
                queue = new Queue<SensorChartPointDto>();

                _queues[deviceId] = queue;
            }

            queue.Enqueue(point);

            while (queue.Count > MaxCount)
            {
                queue.Dequeue();
            }
        }
    }

    public IReadOnlyList<SensorChartPointDto> Get(
        string deviceID)
    {
        lock (_lock)
        {
            if (!_queues.TryGetValue(deviceID, out var queue))
            {
                return [];
            }

            return queue.ToList();
        }
    }
}

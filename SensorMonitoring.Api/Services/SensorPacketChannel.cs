using SensorMonitoring.Contracts;
using System.Threading.Channels;

namespace SensorMonitoring.Api.Services;

public class SensorPacketChannel
{
    private readonly Channel<SensorPacket> _channel;

    public SensorPacketChannel()
    {
        var options =
            new BoundedChannelOptions(10_000)
            {
                FullMode =
                    BoundedChannelFullMode.Wait,

                SingleReader = true,

                SingleWriter = false
            };

        _channel =
            Channel.CreateBounded<SensorPacket>(
                options);
    }

    public ValueTask WriteAsync(
        SensorPacket packet,
        CancellationToken cancellationToken)
    {
        return _channel.Writer.WriteAsync(
            packet,
            cancellationToken);
    }

    public IAsyncEnumerable<SensorPacket>
        ReadAllAsync(
            CancellationToken cancellationToken)
    {
        return _channel.Reader.ReadAllAsync(
            cancellationToken);
    }
}

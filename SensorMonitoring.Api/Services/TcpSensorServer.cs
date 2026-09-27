using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;
using SensorMonitoring.Contracts;

namespace SensorMonitoring.Api.Services;

public class TcpSensorServer : BackgroundService
{
    private readonly ILogger<TcpSensorServer> _logger;
    private readonly SensorPacketChannel _channel;
    private TcpListener? _listener;

    public TcpSensorServer(
        ILogger<TcpSensorServer> logger,
        SensorPacketChannel channel)
    {
        _logger = logger;
        _channel = channel;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _listener =
            new TcpListener(
                IPAddress.Any,
                9000);

        _listener.Start();

        _logger.LogInformation(
            "TCP Server started on port {Port}",
            9000);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var client =
                    await _listener.AcceptTcpClientAsync(
                        stoppingToken);

                _ = HandleClientAsync(
                    client,
                    stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {

        }
        finally
        {
            _listener.Stop();

            _logger.LogInformation(
                "TCP Sensor Server stopped");
        }
    }

    private async Task HandleClientAsync(
        TcpClient client,
        CancellationToken cancellationToken)
    {
        var remoteEndPoint =
            client.Client.RemoteEndPoint?.ToString();

        _logger.LogInformation(
            "TCP client connected: {RemoteEndPoint}",
            remoteEndPoint);

        try
        {
            using (client)
            await using (var stream = client.GetStream())
            using (var reader = new StreamReader(
                stream,
                Encoding.UTF8))
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var line =
                        await reader.ReadLineAsync(
                            cancellationToken);

                    if (line is null)
                    {
                        break;
                    }

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    try
                    {
                        var packet =
                            JsonSerializer.Deserialize<SensorPacket>(
                                line);

                        if (packet is null)
                        {
                            continue;
                        }

                        await _channel.WriteAsync(
                            packet,
                            cancellationToken);

                        _logger.LogInformation(
                            "[{DeviceId}] " +
                            "Temperature: {Temperature:F2} °C, " +
                            "Humidity: {Humidity:F2} %, " +
                            "Time: {Timestamp}",
                            packet.DeviceId,
                            packet.Temperature,
                            packet.Humidity,
                            packet.Timestamp);
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogWarning(
                            ex,
                            "Invalid packet received: {Packet}",
                            line);
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            // 프로그램 종료
        }
        catch (IOException ex)
        {
            _logger.LogWarning(
                ex,
                "TCP connection closed unexpectedly: {RemoteEndPoint}",
                remoteEndPoint);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "TCP client processing error: {RemoteEndPoint}",
                remoteEndPoint);
        }
        finally
        {
            _logger.LogInformation(
                "TCP client disconnected: {RemoteEndPoint}",
                remoteEndPoint);
        }
    }
}

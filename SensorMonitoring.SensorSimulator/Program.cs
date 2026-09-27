using SensorMonitoring.Contracts;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

const string serverIp = "127.0.0.1";
const int serverPort = 9000;

var sensorTasks = Enumerable
    .Range(1, 10)
    .Select(sensorNumber =>
    {
        var deviceId = $"SENSOR-{sensorNumber:000}";

        return RunSensorAsync(
            deviceId,
            serverIp,
            serverPort);
    });

await Task.WhenAll(sensorTasks);

static async Task RunSensorAsync(
    string deviceId,
    string serverIp,
    int serverPort)
{
    while (true)
    {
        try
        {
            using var client = new TcpClient();

            Console.WriteLine(
                $"[{deviceId}] 서버 연결 시도...");

            await client.ConnectAsync(
                serverIp,
                serverPort);

            Console.WriteLine(
                $"[{deviceId}] 연결 성공");

            await using var stream =
                client.GetStream();

            while (client.Connected)
            {
                var packet = new SensorPacket
                {
                    DeviceId = deviceId,

                    Temperature =
                        Math.Round(
                            20 + Random.Shared.NextDouble() * 10,
                            2),

                    Humidity =
                        Math.Round(
                            40 + Random.Shared.NextDouble() * 30,
                            2),

                    Timestamp = DateTime.UtcNow
                };

                var json =
                    JsonSerializer.Serialize(packet);

                var message = json + "\n";

                var bytes =
                    Encoding.UTF8.GetBytes(message);

                await stream.WriteAsync(bytes);

                await stream.FlushAsync();

                Console.WriteLine(
                    $"[{deviceId}] " +
                    $"{packet.Temperature:F2} °C, " +
                    $"{packet.Humidity:F2} %");

                await Task.Delay(1000);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[{deviceId}] 연결 오류: {ex.Message}");

            Console.WriteLine(
                $"[{deviceId}] 3초 후 재연결");

            await Task.Delay(3000);
        }
    }
}


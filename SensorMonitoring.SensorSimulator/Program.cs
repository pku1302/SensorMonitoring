using SensorMonitoring.Contracts;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

// 실행 환경 확인 (기본값은 Development)
var environment =
    Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
    ?? "Development";

// 설정 파일 로드
var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

// TCP 서버 주소와 포트 읽기
var serverIp = configuration["TcpServer:Host"]
    ?? throw new InvalidOperationException("TCP 서버 주소가 없습니다.");

var serverPort = configuration.GetValue<int>("TcpServer:Port");

Console.WriteLine(
    $"실행 환경: {environment}");

Console.WriteLine(
    $"TCP 연결 대상: {serverIp}:{serverPort}");

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
    int packetCount = 0;
    SensorStatusDto randomStatus = SensorStatusDto.Normal;

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
                int phase = packetCount / 5;

                SensorStatusDto? scenarioStatus = deviceId switch
                {
                    "SENSOR-005" => phase % 2 == 0
                        ? SensorStatusDto.Normal
                        : SensorStatusDto.Warning,

                    "SENSOR-006" => phase % 2 == 0
                        ? SensorStatusDto.Warning
                        : SensorStatusDto.Danger,

                    "SENSOR-007" => randomStatus,

                    _ => null
                };

                // SENSOR-007: 5개 패킷마다 상태 무작위 변경
                if (deviceId == "SENSOR-007" && packetCount % 5 == 0)
                {
                    SensorStatusDto[] statuses =
                    [
                        SensorStatusDto.Normal,
                        SensorStatusDto.Warning,
                        SensorStatusDto.Danger
                    ];

                    randomStatus = statuses[
                        Random.Shared.Next(statuses.Length)];

                    scenarioStatus = randomStatus;
                }

                var packet = CreatePacket(deviceId, scenarioStatus);

                var json =
                    JsonSerializer.Serialize(packet);

                var message = json + "\n";

                var bytes =
                    Encoding.UTF8.GetBytes(message);

                await stream.WriteAsync(bytes);

                await stream.FlushAsync();

                Console.WriteLine(
                    $"[{deviceId}] " +
                    $"Temp={packet.Temperature:F2} °C, " +
                    $"Humidity={packet.Humidity:F2} %" +
                    $"Produced={packet.ProducedQuantity}");

                // SENSOR-004: 첫 패킷 이후 전송 종료
                if (deviceId == "SENSOR-004")
                {
                    Console.WriteLine(
                        $"[{deviceId}] 연결 끊김 테스트 시작");

                    return;
                }

                await Task.Delay(1000);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[{deviceId}] 연결 오류: {ex.Message}");

            // SENSOR-004는 재연결하지 않음
            if (deviceId == "SENSOR-004")
                return;

            Console.WriteLine(
                $"[{deviceId}] 3초 후 재연결");

            await Task.Delay(3000);
        }
    }
}

static SensorPacket CreatePacket(
    string deviceId,
    SensorStatusDto? scenarioStatus = null)
{
    double temperature;
    double humidity = 45 + Random.Shared.NextDouble() * 5;

    if (scenarioStatus.HasValue)
    {
        temperature = scenarioStatus.Value switch
        {
            SensorStatusDto.Normal =>
                20 + Random.Shared.NextDouble() * 5,

            SensorStatusDto.Warning =>
                31 + Random.Shared.NextDouble() * 3,

            SensorStatusDto.Danger =>
                36 + Random.Shared.NextDouble() * 4,

            _ => 25
        };
    }
    else
    {
        temperature = deviceId switch
        {
            "SENSOR-001" =>
                20 + Random.Shared.NextDouble() * 5,

            "SENSOR-002" =>
                31 + Random.Shared.NextDouble() * 3,

            "SENSOR-003" =>
                36 + Random.Shared.NextDouble() * 4,

            "SENSOR-004" => 25,

            _ => 20 + Random.Shared.NextDouble() * 10
        };

        if (deviceId is "SENSOR-008" or "SENSOR-009" or "SENSOR-010")
        {
            humidity = 40 + Random.Shared.NextDouble() * 30;
        }
    }

    return new SensorPacket
    {
        DeviceId = deviceId,
        Temperature = Math.Round(temperature, 2),
        Humidity = Math.Round(humidity, 2),
        ProducedQuantity = Random.Shared.Next(0, 6),
        Timestamp = DateTime.UtcNow
    };
}


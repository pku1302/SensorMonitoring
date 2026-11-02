using Microsoft.EntityFrameworkCore;
using SensorMonitoring.Api.Hubs;
using SensorMonitoring.Api.Services;
using SensorMonitoring.Application.Abstractions;
using SensorMonitoring.Application.Services;
using SensorMonitoring.Infrastructure.Caching;
using SensorMonitoring.Infrastructure.Data;
using SensorMonitoring.Infrastructure.Querues;
using SensorMonitoring.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ====== TCP/ Data Transfer =============

builder.Services.AddHostedService<TcpSensorServer>();

builder.Services.AddSingleton<SensorPacketChannel>();

builder.Services.AddHostedService<SensorDataProcessor>();

builder.Services.AddSingleton<SensorChartCache>();

builder.Services.AddSingleton<ISensorChartCache>(
    sp => sp.GetRequiredService<SensorChartCache>());

builder.Services.AddSingleton<SensorAlarmService>();

builder.Services.AddSingleton<ISensorAlarmService>(
    sp => sp.GetRequiredService<SensorAlarmService>());

builder.Services.AddSingleton<ISensorRealtimeNotifier, SensorRealtimeService>();

builder.Services.AddSingleton<SensorIdCache>();

builder.Services.AddScoped<SensorTrendService>();

builder.Services.AddScoped<ISensorHistoryReader, SensorHistoryReader>();

builder.Services.AddScoped<SensorHistoryService>();

builder.Services.AddScoped<ISensorCacheService, SensorCacheService>();

builder.Services.AddScoped<ISensorReadingRepository, SensorReadingRepository>();

builder.Services.AddScoped<ISensorReadingReader, SensorReadingReader>();

builder.Services.AddScoped<ISensorReadingService, SensorReadingService>();

builder.Services.AddScoped<SensorDataProcessingService>();

builder.Services.AddSignalR();

// ====== DB ============
var connectionString =
    builder.Configuration
        .GetConnectionString("MonitoringDb");

builder.Services.AddDbContextFactory<MonitoringDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<ISensorReadingService, SensorReadingService>();

builder.Services.AddScoped<ISensorTrendDataReader, SensorTrendDataReader>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<SensorHub>("/hubs/sensor");

app.Run();

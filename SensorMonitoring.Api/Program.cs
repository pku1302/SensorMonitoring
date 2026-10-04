using Microsoft.EntityFrameworkCore;
using SensorMonitoring.Api.Hubs;
using SensorMonitoring.Api.Services;
using SensorMonitoring.Api.Services.Get;
using SensorMonitoring.Infrastructure.Data;

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

builder.Services.AddSingleton<ProductionCounterService>();

builder.Services.AddHostedService<SensorDataProcessor>();

builder.Services.AddSingleton<SensorStatusService>();

builder.Services.AddSingleton<SensorIdCache>();

builder.Services.AddSingleton<SensorCacheService>();

builder.Services.AddSignalR();

// ====== DB ============
var connectionString =
    builder.Configuration
        .GetConnectionString("MonitoringDb");

builder.Services.AddScoped<ISensorReadingService, SensorReadingService>();

builder.Services.AddDbContext<MonitoringDbContext>(
    options =>
    {
        options.UseSqlServer(connectionString);
    });

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

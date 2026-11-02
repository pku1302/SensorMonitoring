using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SensorMonitoring.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Tests.Helpers;

public sealed class TestDbContext : IDisposable
{
    public SqliteConnection Connection { get; }
    public DbContextOptions<MonitoringDbContext> Options { get; }

    public TestDbContext()
    {
        Connection =
            new SqliteConnection("DataSource=:memory:");

        Connection.Open();

        Options =
            new DbContextOptionsBuilder<MonitoringDbContext>()
                .UseSqlite(Connection)
                .Options;

        using var context =
            new MonitoringDbContext(Options);

        context.Database.EnsureCreated();
    }

    public MonitoringDbContext CreateContext()
    {
        return new MonitoringDbContext(Options);
    }

    public void Dispose()
    {
        Connection.Dispose();
    }
}

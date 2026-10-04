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
    public MonitoringDbContext Context { get; }

    public TestDbContext()
    {
        Connection =
            new SqliteConnection("DataSource=:memory:");

        Connection.Open();

        var options =
            new DbContextOptionsBuilder<MonitoringDbContext>()
                .UseSqlite(Connection)
                .Options;

        Context =
            new MonitoringDbContext(options);

        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Dispose();
        Connection.Dispose();
    }
}

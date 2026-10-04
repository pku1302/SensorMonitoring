using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SensorMonitoring.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Tests.Helpers;

public static class TestDbContextFactory
{


    public static MonitoringDbContext Create()
    {
       
        var connection =
            new SqliteConnection("DataSource=:memory:");

        connection.Open();

        var options =
            new DbContextOptionsBuilder<MonitoringDbContext>()
                .UseSqlite(connection)
                .Options;

        var context =
            new MonitoringDbContext(options);

        context.Database.EnsureCreated();

        return context;
    }
}

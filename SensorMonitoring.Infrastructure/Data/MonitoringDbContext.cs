using Microsoft.EntityFrameworkCore;
using SensorMonitoring.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Infrastructure.Data;
public class MonitoringDbContext : DbContext
{
    public MonitoringDbContext(
        DbContextOptions<MonitoringDbContext> options)
        : base(options)
    {
    }

    public DbSet<SensorReading> SensorReadings =>
        Set<SensorReading>();

    public DbSet<Sensor> Sensors =>
        Set<Sensor>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SensorReading>(
            entity =>
            {
                entity.ToTable("SensorReadings");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.DeviceId)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.Humidity)
                    .IsRequired();

                entity.Property(x => x.TotalProduction)
                    .IsRequired();

                entity.Property(x => x.RecordedAt)
                    .IsRequired();

                entity.HasIndex(x => new
                {
                    x.DeviceId,
                    x.RecordedAt
                });
            });

        modelBuilder.Entity<Sensor>(entity =>
        {
            entity.ToTable("Sensors");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.DeviceId)
                .HasMaxLength(50)
                .IsRequired();

            entity.HasIndex(x => x.DeviceId)
                .IsUnique();

            entity.Property(x => x.CreatedAt)
                .IsRequired();

            entity.Property(x => x.UpdatedAt)
                .IsRequired();
        });
    }

}

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

    public DbSet<SensorAlarmHistory> SensorAlarmHistories => 
        Set<SensorAlarmHistory>();

    public DbSet<SensorAlarmSetting> SensorAlarmSettings => 
        Set<SensorAlarmSetting>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SensorReading>(
            entity =>
            {
                entity.ToTable("SensorReadings");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.SensorId)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.Humidity)
                    .IsRequired();

                entity.Property(x => x.TotalProduction)
                    .IsRequired();

                entity.Property(x => x.RecordedAt)
                    .IsRequired();

                entity.HasOne(x => x.Sensor)
                    .WithMany(x => x.Readings)
                    .HasForeignKey(x => x.SensorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new
                {
                    x.SensorId,
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

        modelBuilder.Entity<SensorAlarmSetting>(entity =>
        {
            entity.ToTable("SensorAlarmSettings");

            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.Sensor)
                .WithOne(x => x.AlarmSetting)
                .HasForeignKey<SensorAlarmSetting>(
                    x => x.SensorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.SensorId)
                .IsUnique();
        });

        modelBuilder.Entity<SensorAlarmHistory>(entity =>
        {
            entity.ToTable("SensorAlarmHistories");

            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.Sensor)
                .WithMany(x => x.AlarmHistories)
                .HasForeignKey(x => x.SensorId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

}

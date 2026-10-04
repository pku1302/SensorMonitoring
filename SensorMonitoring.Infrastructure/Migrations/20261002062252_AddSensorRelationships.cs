using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SensorMonitoring.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSensorRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SensorReadings_DeviceId_RecordedAt",
                table: "SensorReadings");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "SensorReadings");

            migrationBuilder.AddColumn<int>(
                name: "SensorId",
                table: "SensorReadings",
                type: "int",
                maxLength: 50,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SensorAlarmHistories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SensorId = table.Column<int>(type: "int", nullable: false),
                    AlarmCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AlarmMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CleareaAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorAlarmHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SensorAlarmHistories_Sensors_SensorId",
                        column: x => x.SensorId,
                        principalTable: "Sensors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SensorAlarmSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SensorId = table.Column<int>(type: "int", nullable: false),
                    TemperatureWarning = table.Column<double>(type: "float", nullable: true),
                    TemperatureDanger = table.Column<double>(type: "float", nullable: true),
                    HumidityWarning = table.Column<double>(type: "float", nullable: true),
                    HumidityDanger = table.Column<double>(type: "float", nullable: true),
                    OfflineTimeoutSeconds = table.Column<int>(type: "int", nullable: false),
                    IsEnalbed = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorAlarmSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SensorAlarmSettings_Sensors_SensorId",
                        column: x => x.SensorId,
                        principalTable: "Sensors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SensorReadings_SensorId_RecordedAt",
                table: "SensorReadings",
                columns: new[] { "SensorId", "RecordedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SensorAlarmHistories_SensorId",
                table: "SensorAlarmHistories",
                column: "SensorId");

            migrationBuilder.CreateIndex(
                name: "IX_SensorAlarmSettings_SensorId",
                table: "SensorAlarmSettings",
                column: "SensorId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SensorReadings_Sensors_SensorId",
                table: "SensorReadings",
                column: "SensorId",
                principalTable: "Sensors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SensorReadings_Sensors_SensorId",
                table: "SensorReadings");

            migrationBuilder.DropTable(
                name: "SensorAlarmHistories");

            migrationBuilder.DropTable(
                name: "SensorAlarmSettings");

            migrationBuilder.DropIndex(
                name: "IX_SensorReadings_SensorId_RecordedAt",
                table: "SensorReadings");

            migrationBuilder.DropColumn(
                name: "SensorId",
                table: "SensorReadings");

            migrationBuilder.AddColumn<string>(
                name: "DeviceId",
                table: "SensorReadings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_SensorReadings_DeviceId_RecordedAt",
                table: "SensorReadings",
                columns: new[] { "DeviceId", "RecordedAt" });
        }
    }
}

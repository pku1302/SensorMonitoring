using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SensorMonitoring.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalProductionToSensorReading : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TotalProduction",
                table: "SensorReadings",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalProduction",
                table: "SensorReadings");
        }
    }
}

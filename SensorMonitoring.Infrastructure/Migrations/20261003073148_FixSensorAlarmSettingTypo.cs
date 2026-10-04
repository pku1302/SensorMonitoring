using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SensorMonitoring.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixSensorAlarmSettingTypo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsEnalbed",
                table: "SensorAlarmSettings",
                newName: "IsEnabled");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsEnabled",
                table: "SensorAlarmSettings",
                newName: "IsEnalbed");
        }
    }
}

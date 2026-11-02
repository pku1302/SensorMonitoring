using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SensorMonitoring.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixAlarmClearedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CleareaAt",
                table: "SensorAlarmHistories",
                newName: "ClearedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ClearedAt",
                table: "SensorAlarmHistories",
                newName: "CleareaAt");
        }
    }
}

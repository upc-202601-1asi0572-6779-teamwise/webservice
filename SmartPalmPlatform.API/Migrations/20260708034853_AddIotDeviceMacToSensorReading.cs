using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartPalmPlatform.API.Migrations
{
    /// <inheritdoc />
    public partial class AddIotDeviceMacToSensorReading : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "iot_device_mac_address",
                table: "sensor_readings",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "i_x_sensor_readings_iot_device_mac_address_measured_at",
                table: "sensor_readings",
                columns: new[] { "iot_device_mac_address", "measured_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_sensor_readings_iot_device_mac_address_measured_at",
                table: "sensor_readings");

            migrationBuilder.DropColumn(
                name: "iot_device_mac_address",
                table: "sensor_readings");
        }
    }
}

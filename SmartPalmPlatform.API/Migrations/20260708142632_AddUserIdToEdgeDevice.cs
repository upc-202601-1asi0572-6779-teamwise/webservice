using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartPalmPlatform.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToEdgeDevice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "user_id",
                table: "edge_devices",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "user_id",
                table: "edge_devices");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace SmartPalmPlatform.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "agronomic_thresholds",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    edge_device_mac_address = table.Column<string>(type: "longtext", nullable: false),
                    iot_device_mac_address = table.Column<string>(type: "longtext", nullable: false),
                    min = table.Column<double>(type: "double", nullable: false),
                    max = table.Column<double>(type: "double", nullable: false),
                    description = table.Column<string>(type: "longtext", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_agronomic_thresholds", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "edge_devices",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    mac_address = table.Column<string>(type: "varchar(255)", nullable: false),
                    monitoring_zone_id = table.Column<int>(type: "int", nullable: false),
                    last_connectivity_check_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    last_sync_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_edge_devices", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "edge_registries",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    edge_mac_address = table.Column<string>(type: "longtext", nullable: false),
                    iot_device_mac_addresses = table.Column<string>(type: "longtext", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_edge_registries", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "iot_devices",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    mac_address = table.Column<string>(type: "varchar(255)", nullable: false),
                    edge_device_mac_address = table.Column<string>(type: "longtext", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_iot_devices", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "recommendations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    plantation_id = table.Column<int>(type: "int", nullable: false),
                    agronomist_id = table.Column<int>(type: "int", nullable: false),
                    content = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    approved_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    published_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_recommendations", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sensor_readings",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    edge_device_mac_address = table.Column<string>(type: "longtext", nullable: false),
                    measured_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    unit = table.Column<int>(type: "int", nullable: false),
                    value = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_sensor_readings", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "agronomic_interventions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    recommendation_id = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    performed_by = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    execution_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_agronomic_interventions", x => x.id);
                    table.ForeignKey(
                        name: "f_k_agronomic_interventions_recommendations_recommendation_id",
                        column: x => x.recommendation_id,
                        principalTable: "recommendations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_agronomic_interventions_recommendation_id",
                table: "agronomic_interventions",
                column: "recommendation_id");

            migrationBuilder.CreateIndex(
                name: "i_x_edge_devices_mac_address",
                table: "edge_devices",
                column: "mac_address",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_iot_devices_mac_address",
                table: "iot_devices",
                column: "mac_address",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "agronomic_interventions");

            migrationBuilder.DropTable(
                name: "agronomic_thresholds");

            migrationBuilder.DropTable(
                name: "edge_devices");

            migrationBuilder.DropTable(
                name: "edge_registries");

            migrationBuilder.DropTable(
                name: "iot_devices");

            migrationBuilder.DropTable(
                name: "sensor_readings");

            migrationBuilder.DropTable(
                name: "recommendations");
        }
    }
}

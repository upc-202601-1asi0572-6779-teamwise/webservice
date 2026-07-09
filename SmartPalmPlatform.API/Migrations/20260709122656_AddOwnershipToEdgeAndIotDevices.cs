using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace SmartPalmPlatform.API.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnershipToEdgeAndIotDevices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_agronomic_interventions_recommendations_recommendation_id",
                table: "agronomic_interventions");

            migrationBuilder.DropIndex(
                name: "i_x_agronomic_interventions_recommendation_id",
                table: "agronomic_interventions");

            migrationBuilder.AddColumn<int>(
                name: "user_id",
                table: "iot_devices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "user_id",
                table: "edge_devices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "recommendation_id",
                table: "agronomic_interventions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "sector_id",
                table: "agronomic_interventions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "alerts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    sensor_type = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    message = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    level = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    timestamp = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_alerts", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "payment_transactions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    plan_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    period_start = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    period_end = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    transaction_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    processed_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_payment_transactions", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "plantations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    palm_grower_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    address = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    coordinates = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    hectares = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    estimated_sensors = table.Column<int>(type: "int", nullable: false),
                    installation_message = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_plantations", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "subscriptions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    plan_type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    plan_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    max_hectares = table.Column<int>(type: "int", nullable: true),
                    max_sensors = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    billing_cycle = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_subscriptions", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_alert_settings",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    sensor_type = table.Column<int>(type: "int", nullable: false),
                    is_muted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_user_alert_settings", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    username = table.Column<string>(type: "longtext", nullable: false),
                    password_hash = table.Column<string>(type: "longtext", nullable: false),
                    email = table.Column<string>(type: "longtext", nullable: false),
                    full_name = table.Column<string>(type: "longtext", nullable: false),
                    role = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    subscription_id = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_users", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sectors",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    plantation_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    iot_device_mac_address = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    activated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_sectors", x => x.id);
                    table.ForeignKey(
                        name: "f_k_sectors_plantations_plantation_id",
                        column: x => x.plantation_id,
                        principalTable: "plantations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_iot_devices_user_id",
                table: "iot_devices",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "i_x_edge_devices_user_id",
                table: "edge_devices",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "i_x_sectors_iot_device_mac_address",
                table: "sectors",
                column: "iot_device_mac_address",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_sectors_plantation_id",
                table: "sectors",
                column: "plantation_id");

            migrationBuilder.CreateIndex(
                name: "i_x_user_alert_settings_user_id_sensor_type",
                table: "user_alert_settings",
                columns: new[] { "user_id", "sensor_type" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "f_k_edge_devices_users_user_id",
                table: "edge_devices",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_iot_devices_users_user_id",
                table: "iot_devices",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_edge_devices_users_user_id",
                table: "edge_devices");

            migrationBuilder.DropForeignKey(
                name: "f_k_iot_devices_users_user_id",
                table: "iot_devices");

            migrationBuilder.DropTable(
                name: "alerts");

            migrationBuilder.DropTable(
                name: "payment_transactions");

            migrationBuilder.DropTable(
                name: "sectors");

            migrationBuilder.DropTable(
                name: "subscriptions");

            migrationBuilder.DropTable(
                name: "user_alert_settings");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "plantations");

            migrationBuilder.DropIndex(
                name: "i_x_iot_devices_user_id",
                table: "iot_devices");

            migrationBuilder.DropIndex(
                name: "i_x_edge_devices_user_id",
                table: "edge_devices");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "iot_devices");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "edge_devices");

            migrationBuilder.DropColumn(
                name: "sector_id",
                table: "agronomic_interventions");

            migrationBuilder.AlterColumn<int>(
                name: "recommendation_id",
                table: "agronomic_interventions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "i_x_agronomic_interventions_recommendation_id",
                table: "agronomic_interventions",
                column: "recommendation_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_agronomic_interventions_recommendations_recommendation_id",
                table: "agronomic_interventions",
                column: "recommendation_id",
                principalTable: "recommendations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

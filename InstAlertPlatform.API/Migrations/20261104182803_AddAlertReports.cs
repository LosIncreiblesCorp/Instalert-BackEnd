using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstAlertPlatform.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAlertReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "version",
                table: "business_profiles",
                type: "longtext",
                nullable: false);

            migrationBuilder.CreateTable(
                name: "alerts",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    business_id = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    employee_id = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    kind = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    report_status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    category = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    location = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false),
                    latitude = table.Column<double>(type: "double", nullable: true),
                    longitude = table.Column<double>(type: "double", nullable: true),
                    moment = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    description = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false),
                    activated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    resolved_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    submitted_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    completed_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    revision = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    version = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_alerts", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_alerts_business_id_status",
                table: "alerts",
                columns: new[] { "business_id", "status" });

            migrationBuilder.CreateIndex(
                name: "i_x_alerts_employee_id",
                table: "alerts",
                column: "employee_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alerts");

            migrationBuilder.DropColumn(
                name: "version",
                table: "business_profiles");
        }
    }
}

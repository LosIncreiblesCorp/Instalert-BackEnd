using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstAlertPlatform.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDashboardReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dashboard_reports",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    business_id = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    cutoff_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    summary_json = table.Column<string>(type: "longtext", nullable: false),
                    generated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    generation_source = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    generated_by_user_id = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: true),
                    schema_version = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    version = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_dashboard_reports", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_dashboard_reports_business_id_generated_at",
                table: "dashboard_reports",
                columns: new[] { "business_id", "generated_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dashboard_reports");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstAlertPlatform.API.Migrations;

public partial class AddMappingProjections : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "incident_projections",
            columns: table => new
            {
                id = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                business_id = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                type = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                category = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                description = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false),
                latitude = table.Column<double>(type: "double", nullable: true),
                longitude = table.Column<double>(type: "double", nullable: true),
                reported_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                source_revision = table.Column<int>(type: "int", nullable: false),
                visible = table.Column<bool>(type: "tinyint(1)", nullable: false),
                created_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                version = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("p_k_incident_projections", x => x.id);
            })
            .Annotation("MySQL:Charset", "utf8mb4");

        migrationBuilder.CreateIndex(
            name: "i_x_incident_projections_visible",
            table: "incident_projections",
            column: "visible");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "incident_projections");
    }
}
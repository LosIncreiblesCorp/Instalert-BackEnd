using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstAlertPlatform.API.Migrations
{
    /// <inheritdoc />
    public partial class AddEmergencyContacts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "emergency_contacts",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    employee_id = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    full_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    relationship = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false),
                    phone = table.Column<string>(type: "varchar(16)", maxLength: 16, nullable: false),
                    email = table.Column<string>(type: "varchar(254)", maxLength: 254, nullable: true),
                    notes = table.Column<string>(type: "varchar(140)", maxLength: 140, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    version = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_emergency_contacts", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_emergency_contacts_employee_id",
                table: "emergency_contacts",
                column: "employee_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "emergency_contacts");
        }
    }
}

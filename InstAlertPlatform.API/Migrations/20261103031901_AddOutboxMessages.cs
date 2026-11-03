using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstAlertPlatform.API.Migrations
{
    /// <inheritdoc />
    public partial class AddOutboxMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "outbox_messages",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(255)", nullable: false),
                    event_type = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    payload = table.Column<string>(type: "longtext", nullable: false),
                    processed_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    lease_until = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    attempts = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    version = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_outbox_messages", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_outbox_messages_processed_at",
                table: "outbox_messages",
                column: "processed_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "outbox_messages");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstAlertPlatform.API.Migrations
{
    /// <inheritdoc />
    public partial class ProtectInvitationDelivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "invitation_credential",
                table: "delivery_requests",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "invitation_sequence",
                table: "delivery_requests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "invitation_credential",
                table: "delivery_requests");

            migrationBuilder.DropColumn(
                name: "invitation_sequence",
                table: "delivery_requests");
        }
    }
}

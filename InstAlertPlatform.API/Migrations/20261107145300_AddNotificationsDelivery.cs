using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstAlertPlatform.API.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationsDelivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "delivery_requests",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    source_invitation_id = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: true),
                    source_alert_id = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    channel = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    destination = table.Column<string>(type: "varchar(254)", maxLength: 254, nullable: false),
                    user_id = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    attempts = table.Column<int>(type: "int", nullable: false),
                    sent_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    lease_until = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    version = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_delivery_requests", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "notification_preferences",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    user_id = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    business_id = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    nearby_alerts_enabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    nearby_radius_meters = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    version = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_notification_preferences", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    source_alert_id = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    recipient_user_id = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    target_business_id = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    type = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    title = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    message = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false),
                    distance_meters = table.Column<double>(type: "double", nullable: false),
                    read_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    version = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_notifications", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_delivery_requests_status_lease_until_created_at",
                table: "delivery_requests",
                columns: new[] { "status", "lease_until", "created_at" });

            migrationBuilder.CreateIndex(
                name: "i_x_delivery_requests_user_id_created_at",
                table: "delivery_requests",
                columns: new[] { "user_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "i_x_notification_preferences_user_id_business_id",
                table: "notification_preferences",
                columns: new[] { "user_id", "business_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_notifications_alert_recipient_business",
                table: "notifications",
                columns: new[] { "source_alert_id", "recipient_user_id", "target_business_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "delivery_requests");

            migrationBuilder.DropTable(
                name: "notification_preferences");

            migrationBuilder.DropTable(
                name: "notifications");
        }
    }
}

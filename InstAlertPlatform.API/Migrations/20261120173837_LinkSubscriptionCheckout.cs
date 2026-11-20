using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstAlertPlatform.API.Migrations
{
    /// <inheritdoc />
    public partial class LinkSubscriptionCheckout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "provider_subscription_id",
                table: "checkout_intents",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "provider_subscription_id",
                table: "checkout_intents");
        }
    }
}

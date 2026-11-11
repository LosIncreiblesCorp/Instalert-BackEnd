using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstAlertPlatform.API.Migrations
{
    /// <inheritdoc />
    public partial class EnforceSingleCommerceAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_business_profiles_owner_user_id",
                table: "business_profiles");

            migrationBuilder.DropIndex(
                name: "i_x_business_members_business_id_user_id",
                table: "business_members");

            migrationBuilder.AddColumn<string>(
                name: "pending_email",
                table: "staff_invitations",
                type: "varchar(254)",
                maxLength: 254,
                nullable: true,
                computedColumnSql: "CASE WHEN `status` = 'pending' THEN `email` ELSE NULL END",
                stored: true);

            migrationBuilder.CreateIndex(
                name: "i_x_staff_invitations_pending_email",
                table: "staff_invitations",
                column: "pending_email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_business_profiles_owner_user_id",
                table: "business_profiles",
                column: "owner_user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_business_members_user_id",
                table: "business_members",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_staff_invitations_pending_email",
                table: "staff_invitations");

            migrationBuilder.DropIndex(
                name: "i_x_business_profiles_owner_user_id",
                table: "business_profiles");

            migrationBuilder.DropIndex(
                name: "i_x_business_members_user_id",
                table: "business_members");

            migrationBuilder.DropColumn(
                name: "pending_email",
                table: "staff_invitations");

            migrationBuilder.CreateIndex(
                name: "i_x_business_profiles_owner_user_id",
                table: "business_profiles",
                column: "owner_user_id");

            migrationBuilder.CreateIndex(
                name: "i_x_business_members_business_id_user_id",
                table: "business_members",
                columns: new[] { "business_id", "user_id" },
                unique: true);
        }
    }
}

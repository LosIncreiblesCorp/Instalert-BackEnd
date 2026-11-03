using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstAlertPlatform.API.Migrations
{
    /// <inheritdoc />
    public partial class AddBusinessAccessAndSeatQueries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "seat_revision",
                table: "business_profiles",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "business_members",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    business_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    display_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "varchar(254)", maxLength: 254, nullable: false),
                    role = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    version = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_business_members", x => x.id);
                    table.ForeignKey(
                        name: "f_k_business_members__business_profile_business_id",
                        column: x => x.business_id,
                        principalTable: "business_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "staff_invitations",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    business_id = table.Column<int>(type: "int", nullable: false),
                    display_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "varchar(254)", maxLength: 254, nullable: false),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    delivery_sequence = table.Column<int>(type: "int", nullable: false),
                    last_requested_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    version = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_staff_invitations", x => x.id);
                    table.ForeignKey(
                        name: "f_k_staff_invitations_business_profiles_business_id",
                        column: x => x.business_id,
                        principalTable: "business_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_business_members_business_id_status",
                table: "business_members",
                columns: new[] { "business_id", "status" });

            migrationBuilder.CreateIndex(
                name: "i_x_business_members_business_id_user_id",
                table: "business_members",
                columns: new[] { "business_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_business_members_user_id_status",
                table: "business_members",
                columns: new[] { "user_id", "status" });

            migrationBuilder.CreateIndex(
                name: "i_x_staff_invitations_business_id_status",
                table: "staff_invitations",
                columns: new[] { "business_id", "status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "business_members");

            migrationBuilder.DropTable(
                name: "staff_invitations");

            migrationBuilder.DropColumn(
                name: "seat_revision",
                table: "business_profiles");
        }
    }
}

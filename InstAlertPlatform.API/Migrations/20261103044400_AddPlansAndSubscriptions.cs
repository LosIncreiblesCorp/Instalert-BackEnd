using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace InstAlertPlatform.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPlansAndSubscriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "plans",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false),
                    billing_cycle = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    max_employees = table.Column<int>(type: "int", nullable: false),
                    provider_plan_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    version = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_plans", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "subscriptions",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    business_id = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    plan_id = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    provider_subscription_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    current_period_end = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    cancel_at_period_end = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    version = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_subscriptions", x => x.id);
                    table.ForeignKey(
                        name: "f_k_subscriptions_plans_plan_id",
                        column: x => x.plan_id,
                        principalTable: "plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.InsertData(
                table: "plans",
                columns: new[] { "id", "billing_cycle", "created_at", "currency", "max_employees", "name", "price", "provider_plan_id", "updated_at", "version" },
                values: new object[,]
                {
                    { "business", "monthly", null, "USD", 15, "Business", 45m, "P-8D911646UD924981MNLOCK6Q", null, "catalog-v1" },
                    { "essential", "monthly", null, "USD", 4, "Essential", 15m, "P-9G467950ML2745201NLOCKGI", null, "catalog-v1" },
                    { "professional", "monthly", null, "USD", 8, "Professional", 30m, "P-66N5798614080793BNLOCKNY", null, "catalog-v1" }
                });

            migrationBuilder.CreateIndex(
                name: "i_x_plans_provider_plan_id",
                table: "plans",
                column: "provider_plan_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_subscriptions_business_id",
                table: "subscriptions",
                column: "business_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_subscriptions_plan_id",
                table: "subscriptions",
                column: "plan_id");

            migrationBuilder.CreateIndex(
                name: "i_x_subscriptions_provider_subscription_id",
                table: "subscriptions",
                column: "provider_subscription_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "subscriptions");

            migrationBuilder.DropTable(
                name: "plans");
        }
    }
}

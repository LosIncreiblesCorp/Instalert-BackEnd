using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstAlertPlatform.API.Migrations;

public partial class AddBusinessProfileVersion : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "version",
            table: "business_profiles",
            type: "varchar(36)",
            maxLength: 36,
            nullable: true);

        migrationBuilder.Sql(
            "UPDATE `business_profiles` SET `version` = UUID() WHERE `version` IS NULL;");

        migrationBuilder.AlterColumn<string>(
            name: "version",
            table: "business_profiles",
            type: "varchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(36)",
            oldMaxLength: 36,
            oldNullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "version",
            table: "business_profiles");
    }
}

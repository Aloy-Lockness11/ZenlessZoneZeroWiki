using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZenlessZoneZeroWiki.Migrations
{
    /// <inheritdoc />
    public partial class AddTierToCharacterAndWeapon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tier",
                table: "Weapons",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Tier",
                table: "Characters",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tier",
                table: "Weapons");

            migrationBuilder.DropColumn(
                name: "Tier",
                table: "Characters");
        }
    }
}

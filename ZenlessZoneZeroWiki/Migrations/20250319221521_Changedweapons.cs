using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZenlessZoneZeroWiki.Migrations
{
    /// <inheritdoc />
    public partial class Changedweapons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Weapons",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrllink",
                table: "Weapons",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Weapons");

            migrationBuilder.DropColumn(
                name: "ImageUrllink",
                table: "Weapons");
        }
    }
}

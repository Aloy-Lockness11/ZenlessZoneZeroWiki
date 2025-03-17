using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZenlessZoneZeroWiki.Migrations
{
    /// <inheritdoc />
    public partial class CombinedChangesMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favourites_Users_UserFirebaseUid",
                table: "Favourites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Favourites",
                table: "Favourites");

            migrationBuilder.DropIndex(
                name: "IX_Favourites_UserFirebaseUid",
                table: "Favourites");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Favourites");

            migrationBuilder.RenameColumn(
                name: "UserFirebaseUid",
                table: "Favourites",
                newName: "FirebaseUid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Favourites",
                table: "Favourites",
                columns: new[] { "FirebaseUid", "CharacterID", "WeaponID" });

            migrationBuilder.AddForeignKey(
                name: "FK_Favourites_Users_FirebaseUid",
                table: "Favourites",
                column: "FirebaseUid",
                principalTable: "Users",
                principalColumn: "FirebaseUid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favourites_Users_FirebaseUid",
                table: "Favourites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Favourites",
                table: "Favourites");

            migrationBuilder.RenameColumn(
                name: "FirebaseUid",
                table: "Favourites",
                newName: "UserFirebaseUid");

            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "Favourites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Favourites",
                table: "Favourites",
                columns: new[] { "UserID", "CharacterID", "WeaponID" });

            migrationBuilder.CreateIndex(
                name: "IX_Favourites_UserFirebaseUid",
                table: "Favourites",
                column: "UserFirebaseUid");

            migrationBuilder.AddForeignKey(
                name: "FK_Favourites_Users_UserFirebaseUid",
                table: "Favourites",
                column: "UserFirebaseUid",
                principalTable: "Users",
                principalColumn: "FirebaseUid",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WEBCOMIC_FINALPROJECT_.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMangaAndUserSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                table: "Mangas",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthorName",
                table: "Mangas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsNovel",
                table: "Mangas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ViewCount",
                table: "Mangas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Mangas_AuthorId",
                table: "Mangas",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mangas_AspNetUsers_AuthorId",
                table: "Mangas",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mangas_AspNetUsers_AuthorId",
                table: "Mangas");

            migrationBuilder.DropIndex(
                name: "IX_Mangas_AuthorId",
                table: "Mangas");

            migrationBuilder.DropColumn(
                name: "AuthorName",
                table: "Mangas");

            migrationBuilder.DropColumn(
                name: "IsNovel",
                table: "Mangas");

            migrationBuilder.DropColumn(
                name: "ViewCount",
                table: "Mangas");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                table: "Mangas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}

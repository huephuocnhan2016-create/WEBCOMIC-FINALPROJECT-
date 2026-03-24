using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WEBCOMIC_FINALPROJECT_.Migrations
{
    /// <inheritdoc />
    public partial class FixTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MangaImages_Chapters_ChapterId",
                table: "MangaImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MangaImages",
                table: "MangaImages");

            migrationBuilder.RenameTable(
                name: "MangaImages",
                newName: "Images");

            migrationBuilder.RenameIndex(
                name: "IX_MangaImages_ChapterId",
                table: "Images",
                newName: "IX_Images_ChapterId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Images",
                table: "Images",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Chapters_ChapterId",
                table: "Images",
                column: "ChapterId",
                principalTable: "Chapters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Chapters_ChapterId",
                table: "Images");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Images",
                table: "Images");

            migrationBuilder.RenameTable(
                name: "Images",
                newName: "MangaImages");

            migrationBuilder.RenameIndex(
                name: "IX_Images_ChapterId",
                table: "MangaImages",
                newName: "IX_MangaImages_ChapterId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MangaImages",
                table: "MangaImages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MangaImages_Chapters_ChapterId",
                table: "MangaImages",
                column: "ChapterId",
                principalTable: "Chapters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

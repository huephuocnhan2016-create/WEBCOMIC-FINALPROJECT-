using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WEBCOMIC_FINALPROJECT_.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthorAndViewCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Mangas",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "AuthorName",
                table: "Mangas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ViewCount",
                table: "Mangas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ChapterNumber",
                table: "Chapters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ReadingHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MangaId = table.Column<int>(type: "int", nullable: false),
                    LastRead = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadingHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReadingHistories_Mangas_MangaId",
                        column: x => x.MangaId,
                        principalTable: "Mangas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReadingHistories_MangaId",
                table: "ReadingHistories",
                column: "MangaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReadingHistories");

            migrationBuilder.DropColumn(
                name: "AuthorName",
                table: "Mangas");

            migrationBuilder.DropColumn(
                name: "ViewCount",
                table: "Mangas");

            migrationBuilder.DropColumn(
                name: "ChapterNumber",
                table: "Chapters");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Mangas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WEBCOMIC_FINALPROJECT_.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QuizType",
                table: "QuizHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "MangaChapterRatings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Score = table.Column<int>(type: "int", nullable: false),
                    ChapterId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MangaChapterRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MangaChapterRatings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MangaChapterRatings_Chapters_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "Chapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MangaComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChapterId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MangaComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MangaComments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MangaComments_Chapters_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "Chapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MangaRatings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Score = table.Column<int>(type: "int", nullable: false),
                    MangaId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MangaRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MangaRatings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MangaRatings_Mangas_MangaId",
                        column: x => x.MangaId,
                        principalTable: "Mangas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Novels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GenreId = table.Column<int>(type: "int", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    AuthorId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsVipOnly = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Novels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Novels_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NovelChapters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NovelId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsVipOnly = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NovelChapters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NovelChapters_Novels_NovelId",
                        column: x => x.NovelId,
                        principalTable: "Novels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NovelRatings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Score = table.Column<int>(type: "int", nullable: false),
                    NovelId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NovelRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NovelRatings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NovelRatings_Novels_NovelId",
                        column: x => x.NovelId,
                        principalTable: "Novels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NovelChapterRatings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Score = table.Column<int>(type: "int", nullable: false),
                    NovelChapterId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NovelChapterRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NovelChapterRatings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NovelChapterRatings_NovelChapters_NovelChapterId",
                        column: x => x.NovelChapterId,
                        principalTable: "NovelChapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NovelComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NovelChapterId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NovelComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NovelComments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NovelComments_NovelChapters_NovelChapterId",
                        column: x => x.NovelChapterId,
                        principalTable: "NovelChapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MangaChapterRatings_ChapterId",
                table: "MangaChapterRatings",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_MangaChapterRatings_UserId",
                table: "MangaChapterRatings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MangaComments_ChapterId",
                table: "MangaComments",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_MangaComments_UserId",
                table: "MangaComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MangaRatings_MangaId",
                table: "MangaRatings",
                column: "MangaId");

            migrationBuilder.CreateIndex(
                name: "IX_MangaRatings_UserId",
                table: "MangaRatings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_NovelChapterRatings_NovelChapterId",
                table: "NovelChapterRatings",
                column: "NovelChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_NovelChapterRatings_UserId",
                table: "NovelChapterRatings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_NovelChapters_NovelId",
                table: "NovelChapters",
                column: "NovelId");

            migrationBuilder.CreateIndex(
                name: "IX_NovelComments_NovelChapterId",
                table: "NovelComments",
                column: "NovelChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_NovelComments_UserId",
                table: "NovelComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_NovelRatings_NovelId",
                table: "NovelRatings",
                column: "NovelId");

            migrationBuilder.CreateIndex(
                name: "IX_NovelRatings_UserId",
                table: "NovelRatings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Novels_GenreId",
                table: "Novels",
                column: "GenreId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MangaChapterRatings");

            migrationBuilder.DropTable(
                name: "MangaComments");

            migrationBuilder.DropTable(
                name: "MangaRatings");

            migrationBuilder.DropTable(
                name: "NovelChapterRatings");

            migrationBuilder.DropTable(
                name: "NovelComments");

            migrationBuilder.DropTable(
                name: "NovelRatings");

            migrationBuilder.DropTable(
                name: "NovelChapters");

            migrationBuilder.DropTable(
                name: "Novels");

            migrationBuilder.DropColumn(
                name: "QuizType",
                table: "QuizHistories");
        }
    }
}

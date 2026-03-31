using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WEBCOMIC_FINALPROJECT_.Models;

namespace WEBCOMIC_FINALPROJECT_.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // --- CÁC BẢNG GỐC TỪ GITHUB ---
        public DbSet<Manga> Mangas { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<SystemConfig> SystemConfigs { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<UserMangaUnlock> UserMangaUnlocks { get; set; }
        public DbSet<MangaImage> Images { get; set; } // Tên DbSet là Images
        public DbSet<QuizHistory> QuizHistories { get; set; }
        public DbSet<MangaImage> MangaImages { get; set; }
        public DbSet<ReadingHistory> ReadingHistories { get; set; }

        // --- CÁC BẢNG MỚI THÊM VÀO ---
        public DbSet<Novel> Novels { get; set; }
        public DbSet<NovelChapter> NovelChapters { get; set; }
        public DbSet<MangaComment> MangaComments { get; set; }
        public DbSet<NovelComment> NovelComments { get; set; }
        public DbSet<MangaRating> MangaRatings { get; set; }
        public DbSet<MangaChapterRating> MangaChapterRatings { get; set; }
        public DbSet<NovelRating> NovelRatings { get; set; }
        public DbSet<NovelChapterRating> NovelChapterRatings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // BẮT BUỘC: Ép tên bảng trong SQL là "Images" để không bị lỗi 'MangaImage'
            builder.Entity<MangaImage>().ToTable("Images");

            builder.Entity<MangaImage>()
                .HasOne(i => i.Chapter)
                .WithMany(c => c.Images)
                .HasForeignKey(i => i.ChapterId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
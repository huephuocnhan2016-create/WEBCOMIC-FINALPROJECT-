using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WEBCOMIC_FINALPROJECT_.Models;

namespace WEBCOMIC_FINALPROJECT_.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Manga> Mangas { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<SystemConfig> SystemConfigs { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<MangaImage> MangaImages { get; set; }
        public DbSet<UserMangaUnlock> UserMangaUnlocks { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Critical for Identity!

            // Cascade delete: If a Chapter is deleted, delete its Images automatically
            builder.Entity<MangaImage>()
                .HasOne(i => i.Chapter)
                .WithMany(c => c.Images)
                .HasForeignKey(i => i.ChapterId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class SystemConfig
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public int Value { get; set; }
    }
}
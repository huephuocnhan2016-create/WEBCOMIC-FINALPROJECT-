using WEBCOMIC_FINALPROJECT_.Data;
using WEBCOMIC_FINALPROJECT_.Models;

namespace WEBCOMIC_FINALPROJECT_.Models
{
    public class MangaSeed // Thêm dòng này để bao bọc hàm SeedManga
    {
        public void SeedManga(ApplicationDbContext context) // Đổi MyDbContext thành ApplicationDbContext
        {
            if (!context.Mangas.Any())
            {
                context.Mangas.AddRange(
                    new Manga
                    {
                        Title = "One Piece",
                        AuthorName = "Eiichiro Oda",
                        Description = "Hành trình trở thành Vua Hải Tặc",
                        ImageUrl = "onepiece.jpg",
                        GenreId = 1 // Nhớ cho đúng ID thể loại đã có trong DB
                    },
                    new Manga
                    {
                        Title = "Naruto",
                        AuthorName = "Masashi Kishimoto",
                        Description = "Hành trình của một Ninja",
                        ImageUrl = "naruto.jpg",
                        GenreId = 1
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
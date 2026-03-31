using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WEBCOMIC_FINALPROJECT_.Models;

namespace WEBCOMIC_FINALPROJECT_.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // 1. Tạo các Role (Quyền)
            string[] roleNames = { "Admin", "Author", "Member" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 2. Tạo Thể loại (Genres)
            if (!await context.Genres.AnyAsync())
            {
                context.Genres.AddRange(
                    new Genre { Name = "Hành động" },
                    new Genre { Name = "Phiêu lưu" },
                    new Genre { Name = "Hài hước" }
                );
                await context.SaveChangesAsync();
            }

            // 3. Tạo tài khoản Admin
            var adminEmail = "admin@manga.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "Admin@123");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // 4. Tạo 3 bộ truyện mẫu với 3 ảnh bạn đã gửi
            if (!await context.Mangas.AnyAsync())
            {
                var firstGenre = await context.Genres.FirstOrDefaultAsync();

                context.Mangas.AddRange(
                    new Manga
                    {
                        Title = "Đảo Hải Tặc (One Piece)",
                        Description = "Hành trình trở thành Vua Hải Tặc.",
                        ImageUrl = "/images/dao-hai-tac.jpg",
                        AuthorName = "Eiichiro Oda",
                        GenreId = firstGenre?.Id ?? 1,
                        AuthorId = adminUser.Id,
                        IsApproved = true
                    },
                    new Manga
                    {
                        Title = "Naruto",
                        Description = "Hành trình của Ninja làng Lá.",
                        ImageUrl = "/images/naruto.jpg",
                        AuthorName = "Masashi Kishimoto",
                        GenreId = firstGenre?.Id ?? 1,
                        AuthorId = adminUser.Id,
                        IsApproved = true
                    },
                    new Manga
                    {
                        Title = "Doraemon",
                        Description = "Chú mèo máy đến từ tương lai.",
                        ImageUrl = "/images/doraemon.jpg",
                        AuthorName = "Fujiko F. Fujio",
                        GenreId = firstGenre?.Id ?? 1,
                        AuthorId = adminUser.Id,
                        IsApproved = true
                    }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
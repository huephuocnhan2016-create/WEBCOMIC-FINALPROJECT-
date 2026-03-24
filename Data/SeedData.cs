using Microsoft.AspNetCore.Identity;
using WEBCOMIC_FINALPROJECT_.Models;
using Microsoft.EntityFrameworkCore;
namespace WEBCOMIC_FINALPROJECT_.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            if (!await context.Genres.AnyAsync())
            {
                context.Genres.AddRange(
                    new Genre { Name = "Action" },
                    new Genre { Name = "Comedy" },
                    new Genre { Name = "Drama" },
                    new Genre { Name = "Fantasy" },
                    new Genre { Name = "Horror" },
                    new Genre { Name = "Romance" },
                    new Genre { Name = "Sci-Fi" },
                    new Genre { Name = "Slice of Life" }
                );
                await context.SaveChangesAsync();
            }
            // 1. Tạo các Role nếu chưa có
            string[] roleNames = { "Admin", "Moderator", "Author", "Member" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 2. Tạo tài khoản Admin mặc định
            var adminEmail = "admin@manga.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    RewardPoints = 999
                };
                await userManager.CreateAsync(user, "Admin@123");
                await userManager.AddToRoleAsync(user, "Admin");
            }

            // 3. Tạo tài khoản Quản trị viên (Moderator) mặc định
            var modEmail = "mod@manga.com";
            var modUser = await userManager.FindByEmailAsync(modEmail);
            if (modUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = modEmail,
                    Email = modEmail,
                    EmailConfirmed = true,
                    RewardPoints = 100
                };
                // Tạo User với mật khẩu mặc định
                var result = await userManager.CreateAsync(user, "Mod@123");
                if (result.Succeeded)
                {
                    // Gán quyền Moderator
                    await userManager.AddToRoleAsync(user, "Moderator");
                }
            }
        }
    }
}
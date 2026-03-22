using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEBCOMIC_FINALPROJECT_.Data; // Thêm dòng này để nhận diện ApplicationDbContext
using WEBCOMIC_FINALPROJECT_.Models;

namespace WEBCOMIC_FINALPROJECT_.Controllers
{
    // CỰC KỲ QUAN TRỌNG: Phải có Class bao quanh các phương thức
    public class MangaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // Constructor để Inject Database và Identity User Manager
        public MangaController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var manga = await _context.Mangas
                .Include(m => m.Chapters)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (manga == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);

            // Kiểm tra nếu là truyện VIP
            if (manga.IsVipOnly)
            {
                // 1. Nếu là Admin thì miễn phí
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

                // 2. Kiểm tra xem User đã mua truyện này chưa
                var isUnlocked = await _context.UserMangaUnlocks
                    .AnyAsync(u => u.UserId == user.Id && u.MangaId == id);

                if (!isAdmin && !isUnlocked)
                {
                    var vipConfig = await _context.SystemConfigs.FirstOrDefaultAsync(c => c.Key == "PointsToVip");
                    int price = vipConfig?.Value ?? 50;

                    ViewBag.IsLocked = true;
                    ViewBag.VipPrice = price;
                    ViewBag.UserPoints = user.RewardPoints;
                }
            }

            return View(manga);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UnlockManga(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var vipConfig = await _context.SystemConfigs.FirstOrDefaultAsync(c => c.Key == "PointsToVip");
            int price = vipConfig?.Value ?? 50;

            if (user.RewardPoints >= price)
            {
                // Trừ điểm
                user.RewardPoints -= price;

                // Lưu vết mở khóa
                _context.UserMangaUnlocks.Add(new UserMangaUnlock
                {
                    UserId = user.Id,
                    MangaId = id
                });

                // Cập nhật cả 2 bảng cùng lúc
                await _userManager.UpdateAsync(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", new { id = id });
            }

            TempData["Error"] = "Bạn không đủ điểm để mở khóa truyện này!";
            return RedirectToAction("Details", new { id = id });
        }

        // Bạn nên thêm Action Read ở đây luôn để hoàn thiện luồng đọc truyện
        public async Task<IActionResult> Read(int id)
        {
            var chapter = await _context.Chapters
                .Include(c => c.Images)
                .Include(c => c.Manga)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (chapter == null) return NotFound();

            // Bảo mật: Kiểm tra xem user đã unlock Manga này chưa nếu là VIP
            if (chapter.Manga.IsVipOnly)
            {
                var user = await _userManager.GetUserAsync(User);
                var isUnlocked = await _context.UserMangaUnlocks
                    .AnyAsync(u => u.UserId == user.Id && u.MangaId == chapter.MangaId);

                if (!isUnlocked && !User.IsInRole("Admin"))
                {
                    return RedirectToAction("Details", new { id = chapter.MangaId });
                }
            }

            return View(chapter);
        }
    }
}
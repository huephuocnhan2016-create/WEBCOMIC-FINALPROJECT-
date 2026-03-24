using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEBCOMIC_FINALPROJECT_.Data; // Namespace chứa ApplicationDbContext
using WEBCOMIC_FINALPROJECT_.Models; // Namespace chứa ApplicationUser, Manga, v.v.

namespace WEBCOMIC_FINALPROJECT_.Controllers
{
    public class MangaController : Controller
    {
        // 1. Khai báo các dịch vụ cần thiết
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // 2. Inject dịch vụ thông qua Constructor (BẮT BUỘC để hết lỗi _context)
        public MangaController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 3. Hàm Read xử lý trang đọc truyện
        public async Task<IActionResult> Read(int id)
        {
            var chapter = await _context.Chapters
                .Include(c => c.Images)
                .Include(c => c.Manga)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (chapter == null) return NotFound();

            // Xử lý bảo mật cho truyện VIP
            if (chapter.Manga != null && chapter.Manga.IsVipOnly)
            {
                if (!User.Identity.IsAuthenticated) return Challenge();

                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Challenge();

                bool isAdmin = User.IsInRole("Admin");
                bool isAuthor = chapter.Manga.AuthorId == user.Id;

                // Gán ID ra biến riêng để tránh lỗi dịch Expression Tree trong một số phiên bản EF
                var currentUserId = user.Id;
                bool isUnlocked = await _context.UserMangaUnlocks
                    .AnyAsync(u => u.UserId == currentUserId && u.MangaId == chapter.MangaId);

                if (!isUnlocked && !isAdmin && !isAuthor)
                {
                    TempData["Error"] = "Bạn cần mở khóa để đọc chương VIP này!";
                    // Chuyển hướng về trang Details của MangaManagerController
                    return RedirectToAction("Details", "MangaManager", new { id = chapter.MangaId });
                }
            }

            // Logic tìm chương Trước (Prev) và Sau (Next)
            var allChapters = await _context.Chapters
                .Where(c => c.MangaId == chapter.MangaId)
                .OrderBy(c => c.Id)
                .ToListAsync();

            var currentIndex = allChapters.FindIndex(c => c.Id == id);

            ViewBag.PrevId = currentIndex > 0 ? allChapters[currentIndex - 1].Id : (int?)null;
            ViewBag.NextId = currentIndex < allChapters.Count - 1 ? allChapters[currentIndex + 1].Id : (int?)null;

            return View(chapter);
        }
    }
}
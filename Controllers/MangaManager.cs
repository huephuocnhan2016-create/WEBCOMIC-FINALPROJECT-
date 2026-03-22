using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WEBCOMIC_FINALPROJECT_.Data;
using WEBCOMIC_FINALPROJECT_.Models;
using Microsoft.EntityFrameworkCore;
namespace WEBCOMIC_FINALPROJECT_.Controllers
{
    public class MangaManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public async Task<IActionResult> Details(int id)
        {
            var manga = await _context.Mangas.FindAsync(id);
            if (manga.IsVipOnly)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null || !user.IsVip || user.VipExpiryDate < DateTime.Now)
                {
                    return RedirectToAction("UpgradeVip", "User");
                }
            }
            return View(manga);
        }
        public MangaManagerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Read(int id)
        {
            var manga = await _context.Mangas.FindAsync(id);
            if (manga == null) return NotFound();

            if (manga.IsVipOnly)
            {
                var user = await _userManager.GetUserAsync(User);
                // Kiểm tra đăng nhập, quyền VIP và hạn dùng
                if (user == null || !user.IsVip || user.VipExpiryDate < DateTime.Now)
                {
                    return RedirectToAction("UpgradeVip", "User", new { message = "Truyện này dành cho thành viên VIP!" });
                }
            }

            return View(manga);
        }
        // Tác giả đăng truyện
        [Authorize(Roles = "Author")]
        public IActionResult Create() => View();

        [HttpPost]
        [Authorize(Roles = "Author")]
        public async Task<IActionResult> Create(Manga model)
        {
            model.AuthorId = _userManager.GetUserId(User);
            model.IsApproved = false; // Luôn mặc định là false khi mới đăng
            _context.Mangas.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("MyStories");
        }

        // Quản trị viên duyệt truyện
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> PendingList()
        {
            var pending = await _context.Mangas.Where(m => !m.IsApproved).ToListAsync();
            return View(pending);
        }

        [HttpPost]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> Approve(int id)
        {
            var manga = await _context.Mangas.FindAsync(id);
            if (manga != null)
            {
                manga.IsApproved = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("PendingList");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using WEBCOMIC_FINALPROJECT_.Data;
using WEBCOMIC_FINALPROJECT_.Models;

namespace WEBCOMIC_FINALPROJECT_.Controllers
{
    public class MangaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MangaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. HIỂN THỊ DANH SÁCH & TÌM KIẾM
        public async Task<IActionResult> Index(string searchString)
        {
            var mangas = _context.Mangas.Include(m => m.Genre).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                mangas = mangas.Where(s => s.Title!.Contains(searchString)
                                        || s.AuthorName!.Contains(searchString));
            }

            ViewData["CurrentFilter"] = searchString;
            return View(await mangas.ToListAsync());
        }

        // 2. XEM CHI TIẾT TRUYỆN & LƯU LỊCH SỬ ĐỌC
        // 2. XEM CHI TIẾT TRUYỆN & LƯU LỊCH SỬ ĐỌC
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var manga = await _context.Mangas
                .Include(m => m.Genre)
                .Include(m => m.Chapters)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (manga == null) return NotFound();

            // --- TĂNG LƯỢT XEM ---
            manga.ViewCount++;
            _context.Update(manga);

            // --- LƯU LỊCH SỬ ĐỌC (Nếu User đã đăng nhập) ---
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                var existingHistory = await _context.ReadingHistories
                    .FirstOrDefaultAsync(h => h.UserId == userId && h.MangaId == id);

                if (existingHistory == null)
                {
                    // Nếu chưa có thì thêm mới
                    _context.ReadingHistories.Add(new ReadingHistory
                    {
                        UserId = userId,
                        MangaId = id.Value,
                        LastRead = DateTime.Now
                    });
                }
                else
                {
                    // Nếu đã đọc rồi thì cập nhật lại thời gian và đánh dấu là Đã Thay Đổi
                    existingHistory.LastRead = DateTime.Now;
                    _context.Entry(existingHistory).State = EntityState.Modified; // Dòng quan trọng để ép buộc cập nhật
                }
            }

            // Lưu tất cả thay đổi (ViewCount và ReadingHistory) vào Database
            await _context.SaveChangesAsync();

            return View(manga);
        }

        // 3. THÊM TRUYỆN MỚI
        [Authorize(Roles = "Admin,Author")]
        public IActionResult Create()
        {
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Author")]
        public async Task<IActionResult> Create(Manga manga)
        {
            manga.AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                _context.Add(manga);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", manga.GenreId);
            return View(manga);
        }

        // 4. XÓA TRUYỆN
        [HttpPost]
        [Authorize(Roles = "Admin,Author")]
        public async Task<IActionResult> Delete(int id)
        {
            var manga = await _context.Mangas.FindAsync(id);
            if (manga == null) return NotFound();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (User.IsInRole("Admin") || manga.AuthorId == currentUserId)
            {
                _context.Mangas.Remove(manga);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return Forbid();
        }
    }
}
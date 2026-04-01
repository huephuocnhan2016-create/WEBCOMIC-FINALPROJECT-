using Microsoft.AspNetCore.Authorization; // Dòng này cực kỳ quan trọng để fix lỗi của bạn
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WEBCOMIC_FINALPROJECT_.Data;
using WEBCOMIC_FINALPROJECT_.Models;

namespace WEBCOMIC_FINALPROJECT_.Controllers
{
    [Authorize(Roles = "Author,Admin")] // Bây giờ lỗi sẽ biến mất
    public class ChapterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChapterController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Read(int id) // id là ChapterId
        {
            var chapter = await _context.Chapters
                .Include(c => c.Manga)
                .Include(c => c.Images.OrderBy(i => i.Order))
                .FirstOrDefaultAsync(c => c.Id == id);

            if (chapter == null) return NotFound();

            // Lấy UserId của người dùng hiện tại
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Chỉ lưu lịch sử nếu người dùng đã đăng nhập
            if (!string.IsNullOrEmpty(userId))
            {
                var history = await _context.ReadingHistories
                    .FirstOrDefaultAsync(h => h.UserId == userId && h.MangaId == chapter.MangaId);

                if (history == null)
                {
                    // Nếu chưa có lịch sử cho truyện này, tạo mới
                    _context.ReadingHistories.Add(new ReadingHistory
                    {
                        UserId = userId,
                        MangaId = chapter.MangaId,
                        LastRead = DateTime.Now
                    });
                }
                else
                {
                    // Nếu đã có, chỉ cập nhật lại thời gian đọc mới nhất
                    history.LastRead = DateTime.Now;
                }

                await _context.SaveChangesAsync();
            }

            return View(chapter);
        }
        [HttpGet]
        public async Task<IActionResult> Create(int mangaId)
        {
            var manga = await _context.Mangas.FindAsync(mangaId);
            if (manga == null) return NotFound();

            ViewBag.MangaTitle = manga.Title;
            ViewBag.MangaId = mangaId;

            return View();
        }

        // Đừng quên thêm Action POST để lưu dữ liệu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int mangaId, string title, string imageUrls)
        {
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(imageUrls))
            {
                return View();
            }

            var chapter = new Chapter
            {
                MangaId = mangaId,
                Title = title,
                CreatedAt = DateTime.Now,
                Images = new List<MangaImage>()
            };

            var urls = imageUrls.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < urls.Length; i++)
            {
                chapter.Images.Add(new MangaImage { Url = urls[i].Trim(), Order = i });
            }

            _context.Chapters.Add(chapter);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "MangaManager", new { id = mangaId });
        }
    }
}
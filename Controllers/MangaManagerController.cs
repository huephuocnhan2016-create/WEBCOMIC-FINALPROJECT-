using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WEBCOMIC_FINALPROJECT_.Data;
using WEBCOMIC_FINALPROJECT_.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace WEBCOMIC_FINALPROJECT_.Controllers
{
    [Route("MangaManager/[action]/{id?}")]
    public class MangaManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemoryCache _cache;

        public MangaManagerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMemoryCache cache)
        {
            _context = context;
            _userManager = userManager;
            _cache = cache;
        }

        // ==========================================
        // 1. QUẢN LÝ TRUYỆN (MANGA)
        // ==========================================

        [Authorize(Roles = "Author")]
        public IActionResult CreateManga()
        {
            ViewBag.GenreList = new SelectList(_context.Genres, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateManga(Manga manga)
        {
            ModelState.Remove("AuthorId");
            ModelState.Remove("Genre");
            ModelState.Remove("Chapters"); // Loại bỏ kiểm tra danh sách chương trống

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                manga.AuthorId = user.Id;
                manga.IsApproved = false;

                _context.Mangas.Add(manga);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Gửi yêu cầu duyệt truyện thành công!";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.GenreList = new SelectList(_context.Genres, "Id", "Name");
            return View(manga);
        }

        // ==========================================
        // 2. QUẢN LÝ CHƯƠNG (CHAPTER)
        // ==========================================

        [Authorize(Roles = "Author")]
        public async Task<IActionResult> Create(int mangaId)
        {
            var manga = await _context.Mangas.FindAsync(mangaId);
            if (manga == null) return NotFound("Không tìm thấy truyện.");

            ViewBag.MangaId = mangaId; // Cần thiết để giữ liên kết MangaId trong View
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Chapter chapter)
        {
            // KHẮC PHỤC LỖI FOREIGN KEY: Loại bỏ kiểm tra các thuộc tính liên kết
            ModelState.Remove("Manga");
            ModelState.Remove("Images");

            if (ModelState.IsValid)
            {
                _context.Chapters.Add(chapter);
                await _context.SaveChangesAsync();

                // Trả về trang Details của truyện vừa thêm chương
                return RedirectToAction("UploadImages", new { id = chapter.Id });
            }

            ViewBag.MangaId = chapter.MangaId;
            return View(chapter);
        }

        // ==========================================
        // 3. HIỂN THỊ TRUYỆN (DETAILS & READ)
        // ==========================================

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var manga = await _context.Mangas
                .Include(m => m.Genre)
                .Include(m => m.Chapters)
                    .ThenInclude(c => c.Images) // Load kèm ảnh để tránh lỗi đếm ảnh
                .FirstOrDefaultAsync(m => m.Id == id);

            if (manga == null) return NotFound();

            // Lấy điểm đánh giá tổng bộ truyện
            var ratings = await _context.MangaRatings.Where(r => r.MangaId == id).ToListAsync();
            ViewBag.AvgRating = ratings.Any() ? ratings.Average(r => r.Score) : 0;

            if (User.Identity.IsAuthenticated)
            {
                var currentUserId = _userManager.GetUserId(User);
                ViewBag.UserRating = ratings.FirstOrDefault(r => r.UserId == currentUserId)?.Score ?? 0;
            }
            else
            {
                ViewBag.UserRating = 0;
            }

            return View(manga);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Read(int id)
        {
            var chapter = await _context.Chapters
                .Include(c => c.Manga)
                .Include(c => c.Images)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (chapter == null) return NotFound();

            bool canRead = true;

            // KIỂM TRA CƠ CHẾ VIP
            if (chapter.Manga.IsVipOnly)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null || (chapter.Manga.AuthorId != currentUser.Id && !currentUser.IsVip))
                {
                    canRead = false;
                }
            }
            ViewBag.CanRead = canRead;

            // Chuyển chương
            var prevChapter = await _context.Chapters
                .Where(c => c.MangaId == chapter.MangaId && c.Id < id)
                .OrderByDescending(c => c.Id)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            var nextChapter = await _context.Chapters
                .Where(c => c.MangaId == chapter.MangaId && c.Id > id)
                .OrderBy(c => c.Id)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            ViewBag.PrevId = prevChapter != 0 ? prevChapter : (int?)null;
            ViewBag.NextId = nextChapter != 0 ? nextChapter : (int?)null;

            // Lấy danh sách bình luận
            ViewBag.Comments = await _context.MangaComments
                 .Include(c => c.User)
                 .Where(c => c.ChapterId == id)
                 .OrderByDescending(c => c.CreatedAt)
                 .ToListAsync();

            // Điểm đánh giá chương
            var chapterRatings = await _context.MangaChapterRatings
                .Where(r => r.ChapterId == id)
                .ToListAsync();

            ViewBag.AvgChapterRating = chapterRatings.Any() ? chapterRatings.Average(r => r.Score) : 0;

            if (User.Identity.IsAuthenticated)
            {
                var currentUserId = _userManager.GetUserId(User);
                ViewBag.UserChapterRating = chapterRatings.FirstOrDefault(r => r.UserId == currentUserId)?.Score ?? 0;
            }
            else
            {
                ViewBag.UserChapterRating = 0;
            }

            return View(chapter);
        }

        // ==========================================
        // 4. BÌNH LUẬN & ĐÁNH GIÁ (RATING)
        // ==========================================

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RateManga(int id, int score)
        {
            var userId = _userManager.GetUserId(User);
            var existingRating = await _context.MangaRatings
                .FirstOrDefaultAsync(r => r.MangaId == id && r.UserId == userId);

            if (existingRating != null)
            {
                existingRating.Score = score;
            }
            else
            {
                _context.MangaRatings.Add(new MangaRating { MangaId = id, UserId = userId, Score = score });
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = id });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RateChapter(int chapterId, int score)
        {
            var userId = _userManager.GetUserId(User);
            var existingRating = await _context.MangaChapterRatings
                .FirstOrDefaultAsync(r => r.ChapterId == chapterId && r.UserId == userId);

            if (existingRating != null)
            {
                existingRating.Score = score;
            }
            else
            {
                _context.MangaChapterRatings.Add(new MangaChapterRating
                {
                    ChapterId = chapterId,
                    UserId = userId,
                    Score = score
                });
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Read", new { id = chapterId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostComment(int chapterId, string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return BadRequest();

            var comment = new MangaComment
            {
                ChapterId = chapterId,
                Content = content,
                UserId = _userManager.GetUserId(User),
                CreatedAt = DateTime.Now
            };

            _context.MangaComments.Add(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Read", new { id = chapterId });
        }

        // ==========================================
        // 5. CÁC TÍNH NĂNG KHÁC (TÌM KIẾM, UPLOAD, DUYỆT BÀI)
        // ==========================================

        [AllowAnonymous]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction("Index", "Home");
            }

            var results = await _context.Mangas
                .Include(m => m.Genre)
                .Where(m => m.IsApproved && m.Title.Contains(query))
                .ToListAsync();

            if (results == null || !results.Any())
            {
                TempData["Error"] = "Không tìm thấy truyện nào phù hợp với: " + query;
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Query = query;
            return View(results);
        }

        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> PendingList()
        {
            var pending = await _context.Mangas
                .Include(m => m.Genre)
                .Where(m => !m.IsApproved)
                .ToListAsync();
            return View(pending);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var manga = await _context.Mangas.FindAsync(id);
            if (manga == null) return NotFound();

            manga.IsApproved = true;
            _context.Update(manga);
            await _context.SaveChangesAsync();

            _cache.Remove("LatestMangas");

            TempData["Success"] = $"Đã duyệt: {manga.Title}";
            return RedirectToAction("PendingList");
        }

        [Authorize(Roles = "Author")]
        public async Task<IActionResult> UploadImages(int id)
        {
            var chapter = await _context.Chapters.Include(c => c.Manga).FirstOrDefaultAsync(c => c.Id == id);
            if (chapter == null) return NotFound();
            return View(chapter);
        }

        [HttpPost]
        [Authorize(Roles = "Author")]
        public async Task<IActionResult> UploadImages(int chapterId, List<IFormFile> files)
        {
            var chapter = await _context.Chapters.FindAsync(chapterId);
            if (chapter == null) return NotFound();

            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/Mangas");
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            foreach (var file in files)
            {
                string fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _context.MangaImages.Add(new MangaImage
                {
                    ChapterId = chapterId,
                    Url = "/Images/Mangas/" + fileName
                });
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = chapter.MangaId });
        }

        [Authorize(Roles = "Admin,Moderator,Author")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteChapter(int id)
        {
            var chapter = await _context.Chapters.FindAsync(id);
            if (chapter == null) return NotFound();

            int mangaId = chapter.MangaId;
            _context.Chapters.Remove(chapter);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = mangaId });
        }
    }
}
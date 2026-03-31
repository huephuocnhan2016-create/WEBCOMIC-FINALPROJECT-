using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WEBCOMIC_FINALPROJECT_.Data;
using WEBCOMIC_FINALPROJECT_.Models;

namespace WEBCOMIC_FINALPROJECT_.Controllers
{
    [Route("NovelManager/{action=Index}/{id?}")]
    public class NovelManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemoryCache _cache;

        public NovelManagerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMemoryCache cache)
        {
            _context = context;
            _userManager = userManager;
            _cache = cache;
        }

        // =============================================
        // 1. QUẢN LÝ TRUYỆN (NOVEL)
        // =============================================

        [Authorize(Roles = "Author")]
        public IActionResult CreateNovel()
        {
            ViewBag.GenreList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Genres, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Author")]
        public async Task<IActionResult> CreateNovel(Novel novel)
        {
            ModelState.Remove("AuthorId");
            ModelState.Remove("Genre");
            ModelState.Remove("Chapters");

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                novel.AuthorId = user.Id;
                novel.IsApproved = false;

                _context.Novels.Add(novel);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Gửi yêu cầu duyệt truyện thành công!";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.GenreList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Genres, "Id", "Name");
            return View(novel);
        }

        // =============================================
        // 2. QUẢN LÝ CHƯƠNG (NOVEL CHAPTER)
        // =============================================

        [Authorize(Roles = "Author")]
        public async Task<IActionResult> Create(int novelId)
        {
            var novel = await _context.Novels.FindAsync(novelId);
            if (novel == null) return NotFound("Không tìm thấy truyện.");

            ViewBag.NovelId = novelId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Author")]
        public async Task<IActionResult> Create(NovelChapter chapter)
        {
            ModelState.Remove("Novel");

            if (ModelState.IsValid)
            {
                _context.NovelChapters.Add(chapter);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", new { id = chapter.NovelId });
            }

            ViewBag.NovelId = chapter.NovelId;
            return View(chapter);
        }

        // =============================================
        // 3. HIỂN THỊ TRUYỆN (DETAILS & READ)
        // =============================================

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var novel = await _context.Novels
                .Include(n => n.Genre)
                .Include(n => n.Chapters)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (novel == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            bool isOwner = user != null && novel.AuthorId == user.Id;

            ViewBag.IsOwner = isOwner;

            var ratings = await _context.NovelRatings.Where(r => r.NovelId == id).ToListAsync();

            ViewBag.AvgRating = ratings.Any() ? ratings.Average(r => r.Score) : 0;

            if (User.Identity.IsAuthenticated)
            {
                var currentUserId = user?.Id; // Đã có sẵn biến user ở trên
                ViewBag.UserRating = ratings.FirstOrDefault(r => r.UserId == currentUserId)?.Score ?? 0;
            }
            else
            {
                ViewBag.UserRating = 0;
            }
            // --------------------------------------------------------

            return View(novel);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Read(int id)
        {
            var chapter = await _context.NovelChapters
                .Include(c => c.Novel)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (chapter == null) return NotFound();

            // Kiểm tra VIP
            if (chapter.Novel != null && chapter.Novel.IsVipOnly)
            {
                if (!User.Identity.IsAuthenticated) return Challenge();

                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Challenge();

                bool isAdmin = User.IsInRole("Admin");
                bool isAuthor = chapter.Novel.AuthorId == user.Id;

                if (!isAdmin && !isAuthor && !user.IsVip)
                {
                    TempData["Error"] = "Bạn cần mở khóa để đọc chương VIP này!";
                    return RedirectToAction("Details", new { id = chapter.NovelId });
                }
            }

            ViewBag.Comments = await _context.NovelComments
                .Include(c => c.User)
                .Where(c => c.NovelChapterId == id)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            // Chương trước / sau
            var allChapters = await _context.NovelChapters
                .Where(c => c.NovelId == chapter.NovelId)
                .OrderBy(c => c.Id)
                .ToListAsync();

            var currentIndex = allChapters.FindIndex(c => c.Id == id);
            ViewBag.PrevId = currentIndex > 0 ? allChapters[currentIndex - 1].Id : (int?)null;
            ViewBag.NextId = currentIndex < allChapters.Count - 1 ? allChapters[currentIndex + 1].Id : (int?)null;

            var ratings = await _context.NovelRatings.Where(r => r.NovelId == id).ToListAsync();
            ViewBag.AvgRating = ratings.Any() ? ratings.Average(r => r.Score) : 0;
            ViewBag.UserRating = User.Identity.IsAuthenticated ?
                (await _context.NovelRatings.FirstOrDefaultAsync(r => r.NovelId == id && r.UserId == _userManager.GetUserId(User)))?.Score : 0;

            var chapterRatings = await _context.NovelChapterRatings
                .Where(r => r.NovelChapterId == id)
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostComment(int chapterId, string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return BadRequest();

            var comment = new NovelComment
            {
                NovelChapterId = chapterId,
                Content = content,
                UserId = _userManager.GetUserId(User),
                CreatedAt = DateTime.Now
            };

            _context.NovelComments.Add(comment);
            await _context.SaveChangesAsync();

            // Quay lại đúng chương vừa bình luận
            return RedirectToAction("Read", new { id = chapterId });
        }

        // =============================================
        // 4. DUYỆT BÀI & XỬ LÝ
        // =============================================

        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> PendingList()
        {
            var pending = await _context.Novels
                .Include(n => n.Genre)
                .Where(n => !n.IsApproved)
                .ToListAsync();
            return View(pending);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var novel = await _context.Novels.FindAsync(id);
            if (novel == null) return NotFound();

            novel.IsApproved = true;
            _context.Update(novel);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã duyệt: {novel.Title}";
            return RedirectToAction("PendingList");
        }

        [Authorize(Roles = "Admin,Moderator,Author")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteChapter(int id)
        {
            var chapter = await _context.NovelChapters.FindAsync(id);
            if (chapter == null) return NotFound();

            int novelId = chapter.NovelId;
            _context.NovelChapters.Remove(chapter);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = novelId });
        }
        // --- HÀM XỬ LÝ ĐÁNH GIÁ TỔNG BỘ TRUYỆN CHỮ ---
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RateNovel(int id, int score) // Đổi novelId thành id
        {
            var userId = _userManager.GetUserId(User);

            var existingRating = await _context.NovelRatings
                .FirstOrDefaultAsync(r => r.NovelId == id && r.UserId == userId); // Dùng id

            if (existingRating != null)
            {
                existingRating.Score = score;
            }
            else
            {
                _context.NovelRatings.Add(new NovelRating
                {
                    NovelId = id, // Dùng id
                    UserId = userId,
                    Score = score
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = id }); // Dùng id
        }

        // Đánh giá chương Novel
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RateNovelChapter(int chapterId, int score)
        {
            var userId = _userManager.GetUserId(User);
            var existingRating = await _context.NovelChapterRatings
                .FirstOrDefaultAsync(r => r.NovelChapterId == chapterId && r.UserId == userId);

            if (existingRating != null)
            {
                existingRating.Score = score;
            }
            else
            {
                _context.NovelChapterRatings.Add(new NovelChapterRating
                {
                    NovelChapterId = chapterId,
                    UserId = userId,
                    Score = score
                });
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Read", new { id = chapterId });
        }
    }
}
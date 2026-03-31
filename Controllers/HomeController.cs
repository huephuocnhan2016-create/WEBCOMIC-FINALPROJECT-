using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WEBCOMIC_FINALPROJECT_.Data;
using WEBCOMIC_FINALPROJECT_.Models;

namespace WEBCOMIC_FINALPROJECT_.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        pusing Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using WEBCOMIC_FINALPROJECT_.Data;
using WEBCOMIC_FINALPROJECT_.Models;

namespace WEBCOMIC_FINALPROJECT_.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public HomeController(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            int pageSize = 12;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // --- TÍNH NĂNG 1: TRUYỆN HOT (Dựa trên ViewCount) ---
            ViewBag.HotMangas = await _context.Mangas
                .Where(m => m.IsApproved)
                .OrderByDescending(m => m.ViewCount)
                .Take(5) // Lấy 5 truyện hot nhất
                .ToListAsync();

            // --- TÍNH NĂNG 1.2: TRUYỆN THEO SỞ THÍCH (Dựa trên thể loại đã đọc gần nhất) ---
            if (userId != null)
            {
                var lastReadGenreId = await _context.ReadingHistories
                    .Where(h => h.UserId == userId)
                    .OrderByDescending(h => h.LastRead)
                    .Select(h => h.Manga.GenreId)
                    .FirstOrDefaultAsync();

                if (lastReadGenreId != 0)
                {
                    ViewBag.Recommended = await _context.Mangas
                        .Where(m => m.IsApproved && m.GenreId == lastReadGenreId)
                        .Take(4)
                        .ToListAsync();
                }
            }

            // --- LOGIC PHÂN TRANG & SEARCH CŨ CỦA BẠN ---
            IQueryable<Manga> query = _context.Mangas.Include(m => m.Genre).Where(m => m.IsApproved);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(m => m.Title.Contains(search));
                ViewBag.Search = search;
            }

            var totalCount = await query.CountAsync();
            var pagedMangas = await query
                .OrderByDescending(m => m.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return View(pagedMangas);
        }

        // --- TÍNH NĂNG 2: XEM LỊCH SỬ ĐỌC TRUYỆN ---
        public async Task<IActionResult> History()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToPage("/Account/Login", new { area = "Identity" });

            var history = await _context.ReadingHistories
                .Include(h => h.Manga)
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.LastRead)
                .ToListAsync();

            return View(history);
        }

        // --- TÍNH NĂNG 3: KÊNH CHAT CHUNG (Chỉ chuyển hướng đến View Chat) ---
        public IActionResult CommunityChat()
        {
            return View();
        }

        public IActionResult Search(string query)
        {
            return RedirectToAction("Index", new { search = query });
        }
    }
}   rivate readonly IMemoryCache _cache;
        public IActionResult Search(string query)
        {
            // Nếu không tìm thấy kết quả hoặc query trống, logic bên Index sẽ xử lý
            // Chúng ta chỉ cần chuyển hướng tham số 'query' sang tham số 'search' của hàm Index
            return RedirectToAction("Index", new { search = query });
        }
        public HomeController(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            int pageSize = 12;
            const string CacheKey = "LatestMangas";

            // SỬA LỖI 1: Nếu có tìm kiếm, bỏ qua Cache hoàn toàn để kết quả chính xác
            if (!string.IsNullOrEmpty(search))
            {
                var searchResults = await _context.Mangas
                    .Include(m => m.Genre)
                    .Where(m => m.IsApproved && m.Title.Contains(search))
                    .OrderByDescending(m => m.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var totalSearch = await _context.Mangas.CountAsync(m => m.IsApproved && m.Title.Contains(search));
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = (int)Math.Ceiling((double)totalSearch / pageSize);
                ViewBag.Search = search;
                return View(searchResults);
            }

            // SỬA LỖI 2: Xử lý Cache thông minh hơn cho trang chủ (page 1)
            if (page == 1)
            {
                if (!_cache.TryGetValue(CacheKey, out List<Manga> mangas))
                {
                    // Nếu không có cache, lấy từ DB
                    mangas = await _context.Mangas
                        .Include(m => m.Genre)
                        .Where(m => m.IsApproved)
                        .OrderByDescending(m => m.Id)
                        .Take(pageSize)
                        .AsNoTracking()
                        .ToListAsync();

                    // Lưu cache ngắn thôi (ví dụ 2 phút) để truyện mới mau hiện lên
                    // Hoặc xóa dòng này nếu bạn muốn truyện hiện ngay lập tức sau khi duyệt
                    _cache.Set(CacheKey, mangas, TimeSpan.FromMinutes(2));
                }

                var totalItems = await _context.Mangas.CountAsync(m => m.IsApproved);
                ViewBag.CurrentPage = 1;
                ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                return View(mangas);
            }

            // Xử lý các trang từ 2 trở đi (Không dùng cache để tránh sai lệch dữ liệu)
            var query = _context.Mangas.Include(m => m.Genre).Where(m => m.IsApproved);
            var totalCount = await query.CountAsync();
            var pagedMangas = await query
                .OrderByDescending(m => m.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return View(pagedMangas);
        }
    }
}
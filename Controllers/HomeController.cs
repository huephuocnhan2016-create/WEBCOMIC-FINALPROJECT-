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
        private readonly IMemoryCache _cache;
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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

        public IActionResult Search(string query)
        {
            return RedirectToAction("Index", new { search = query });
        }

        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            int pageSize = 24;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            const string CacheKey = "LatestMangas";

            // --- 1. LẤY TRUYỆN CHỮ (NOVEL) ---
            ViewBag.Novels = await _context.Novels
                .Include(n => n.Genre)
                .Where(n => n.IsApproved)
                .OrderByDescending(n => n.Id)
                .ToListAsync();

            // --- 2. LẤY TRUYỆN TRANH HOT (Dựa trên ViewCount) ---
            ViewBag.HotMangas = await _context.Mangas
                .Where(m => m.IsApproved)
                .OrderByDescending(m => m.ViewCount)
                .Take(5)
                .ToListAsync();

            // --- 3. TRUYỆN THEO SỞ THÍCH (Dựa trên thể loại đã đọc gần nhất) ---
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

            // --- 4. TÌM KIẾM TRUYỆN TRANH ---
            if (!string.IsNullOrEmpty(search))
            {
                var searchQuery = _context.Mangas
                    .Include(m => m.Genre)
                    .Where(m => m.IsApproved && m.Title.Contains(search));

                var totalSearch = await searchQuery.CountAsync();
                var searchResults = await searchQuery
                    .OrderByDescending(m => m.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = (int)Math.Ceiling((double)totalSearch / pageSize);
                ViewBag.Search = search;

                return View(searchResults);
            }

            // --- 5. HIỂN THỊ DANH SÁCH TRUYỆN TRANH (Trang 1 có Cache) ---
            if (page == 1)
            {
                if (!_cache.TryGetValue(CacheKey, out List<Manga> mangas))
                {
                    mangas = await _context.Mangas
                        .Include(m => m.Genre)
                        .Where(m => m.IsApproved)
                        .OrderByDescending(m => m.Id)
                        .Take(pageSize)
                        .AsNoTracking()
                        .ToListAsync();

                    _cache.Set(CacheKey, mangas, TimeSpan.FromMinutes(2));
                }

                var totalItems = await _context.Mangas.CountAsync(m => m.IsApproved);
                ViewBag.CurrentPage = 1;
                ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                return View(mangas);
            }

            // --- 6. PHÂN TRANG CHO TRUYỆN TRANH (Từ trang 2 trở đi) ---
            var queryAll = _context.Mangas.Include(m => m.Genre).Where(m => m.IsApproved);
            var totalCount = await queryAll.CountAsync();
            var pagedMangas = await queryAll
                .OrderByDescending(m => m.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return View(pagedMangas);
        }

        // --- TÍNH NĂNG 7: XEM LỊCH SỬ ĐỌC TRUYỆN ---
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

        // --- TÍNH NĂNG 8: KÊNH CHAT CHUNG ---
        public IActionResult CommunityChat()
        {
            return View();
        }
    }
}
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

        // --- TÌM KIẾM ---
        public IActionResult Search(string query)
        {
            return RedirectToAction("Index", new { search = query });
        }

        // --- TRANG CHỦ ---
        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            int pageSize = 24;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            const string CacheKey = "LatestMangas";

            if (userId != null)
            {
                ViewBag.History = await _context.ReadingHistories
                    .Include(h => h.Manga)
                    .Where(h => h.UserId == userId)
                    .OrderByDescending(h => h.LastRead)
                    .Take(4)
                    .ToListAsync();
            }

            ViewBag.Novels = await _context.Novels
                .Include(n => n.Genre)
                .Where(n => n.IsApproved)
                .OrderByDescending(n => n.Id)
                .ToListAsync();

            var queryManga = _context.Mangas.Include(m => m.Genre).Where(m => m.IsApproved);
            if (!string.IsNullOrEmpty(search))
            {
                queryManga = queryManga.Where(m => m.Title.Contains(search));
            }

            var totalItems = await queryManga.CountAsync();
            var mangas = await queryManga
                .OrderByDescending(m => m.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.Search = search;

            return View(mangas);
        }

        // --- XEM TOÀN BỘ LỊCH SỬ ---
        [HttpGet]
        public async Task<IActionResult> History()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");

            var allHistory = await _context.ReadingHistories
                .Include(h => h.Manga)
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.LastRead)
                .ToListAsync();

            return View(allHistory);
        }

        // --- CỘNG ĐỒNG CHAT (Đã sửa theo yêu cầu của bạn) ---
        public async Task<IActionResult> CommunityChat()
        {
            var history = await _context.ChatMessages
                .OrderByDescending(m => m.SentAt) // 1. Lấy tin mới nhất đưa lên đầu để 'Take'
                .Take(50)                         // 2. Chỉ lấy 50 tin mới nhất đó
                .OrderBy(m => m.SentAt)           // 3. Sắp xếp lại tăng dần để hiện đúng thứ tự chat
                .ToListAsync();

            return View(history);
        }
    }
}
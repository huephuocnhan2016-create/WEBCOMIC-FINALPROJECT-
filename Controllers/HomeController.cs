using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using WEBCOMIC_FINALPROJECT_.Data; // Ensure this is here
using WEBCOMIC_FINALPROJECT_.Models;

namespace WEBCOMIC_FINALPROJECT_.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context; // Add this
        private readonly IMemoryCache _cache; // Add this

        // Inject everything through the constructor
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IMemoryCache cache)
        {
            _logger = logger;
            _context = context;
            _cache = cache;
        }

        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            int pageSize = 12;
            const string CacheKey = "LatestMangas";

            // 1. Check Cache ONLY if there is no search query (so users get fresh search results)
            if (string.IsNullOrEmpty(search) && page == 1)
            {
                if (_cache.TryGetValue(CacheKey, out List<Manga> cachedMangas))
                {
                    ViewBag.CurrentPage = 1;
                    ViewBag.TotalPages = 1; // Simplify for cache
                    return View(cachedMangas);
                }
            }

            // 2. Build the Query
            var query = _context.Mangas.Where(m => m.IsApproved);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(m => m.Title.Contains(search));
            }

            // 3. Get Data
            var totalItems = await query.CountAsync();
            var mangas = await query
                .OrderByDescending(m => m.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // 4. Set Cache (optional, only for the first page of results)
            if (string.IsNullOrEmpty(search) && page == 1)
            {
                _cache.Set(CacheKey, mangas, TimeSpan.FromMinutes(10));
            }

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.Search = search;

            return View(mangas);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
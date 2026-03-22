using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WEBCOMIC_FINALPROJECT_.Data;
using WEBCOMIC_FINALPROJECT_.Models;

namespace WEBCOMIC_FINALPROJECT_.Controllers
{
    [Authorize(Roles = "Author,Admin")]
    public class ChapterController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ChapterController(ApplicationDbContext context) => _context = context;

        public IActionResult Create(int mangaId)
        {
            ViewBag.MangaId = mangaId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(int mangaId, string name, List<string> imageUrls)
        {
            var chapter = new Chapter
            {
                MangaId = mangaId,
                Name = name,
                Images = imageUrls.Select((url, index) => new MangaImage
                {
                    Url = url,
                    Order = index
                }).ToList()
            };

            _context.Chapters.Add(chapter);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Manga", new { id = mangaId });
        }
    }
}

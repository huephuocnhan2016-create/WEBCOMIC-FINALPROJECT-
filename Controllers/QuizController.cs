using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WEBCOMIC_FINALPROJECT_.Data;
using WEBCOMIC_FINALPROJECT_.Models;

namespace WEBCOMIC_FINALPROJECT_.Controllers
{
    public class QuizController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public QuizController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public IActionResult Index() => View(); // Hiển thị nút xác nhận chơi

        [HttpPost]
        public IActionResult StartGame(int genreId)
        {
            // Lấy 10 truyện ngẫu nhiên thuộc thể loại
            var questions = _context.Mangas
                .Where(m => m.GenreId == genreId)
                .OrderBy(r => Guid.NewGuid())
                .Take(10)
                .Select(m => new QuizViewModel
                {
                    CorrectMangaId = m.Id,
                    ImageUrl = m.ImageUrl,
                    Options = _context.Mangas
                        .Where(x => x.GenreId == genreId)
                        .OrderBy(r => Guid.NewGuid())
                        .Take(3)
                        .Select(x => x.Title)
                        .Append(m.Title)
                        .OrderBy(x => Guid.NewGuid()).ToList()
                }).ToList();

            return View("Play", questions);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitResult(int score)
        {
            if (score == 10)
            {
                var user = await _userManager.GetUserAsync(User);
                user.RewardPoints += 1;
                await _userManager.UpdateAsync(user);
                return Json(new { success = true, message = "Bạn nhận được 1 điểm!" });
            }
            return Json(new { success = false, message = "Rất tiếc, bạn cần đúng 10/10." });
        }
    }
}

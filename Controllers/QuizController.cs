using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEBCOMIC_FINALPROJECT_.Data;
using WEBCOMIC_FINALPROJECT_.Models;

namespace WEBCOMIC_FINALPROJECT_.Controllers
{
    [Authorize]
    public class QuizController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public QuizController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 1. Trang giới thiệu Quiz
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var today = DateTime.Today;

            // Kiểm tra xem hôm nay người dùng đã chơi chưa
            bool hasPlayed = await _context.QuizHistories
                .AnyAsync(q => q.UserId == user.Id && q.DatePlayed == today);

            ViewBag.HasPlayed = hasPlayed;
            return View();
        }

        // 2. API Lấy 10 câu hỏi ngẫu nhiên (Gọi qua AJAX)
        [HttpGet]
        public async Task<IActionResult> StartQuiz()
        {
            var user = await _userManager.GetUserAsync(User);
            var today = DateTime.Today;

            if (await _context.QuizHistories.AnyAsync(q => q.UserId == user.Id && q.DatePlayed == today))
                return BadRequest("Hôm nay bạn đã nhận thưởng rồi!");

            // Lấy 10 ảnh ngẫu nhiên. Lưu ý: Đổi tên bảng thành 'Images' nếu 'MangaImages' lỗi
            var randomImages = await _context.Images
                .Include(i => i.Chapter).ThenInclude(c => c.Manga)
                .OrderBy(r => Guid.NewGuid())
                .Take(10)
                .Select(i => new {
                    ImageUrl = i.Url,
                    MangaTitle = i.Chapter.Manga.Title
                })
                .ToListAsync();

            if (randomImages.Count < 10)
                return BadRequest("Hệ thống chưa đủ 10 ảnh để tạo Quiz.");

            return Json(randomImages);
        }

        // 3. API Nhận thưởng
        [HttpPost]
        [ValidateAntiForgeryToken] // Bảo mật tránh spam
        public async Task<IActionResult> ClaimReward()
        {
            var user = await _userManager.GetUserAsync(User);
            var today = DateTime.Today;

            bool alreadyPlayed = await _context.QuizHistories.AnyAsync(q => q.UserId == user.Id && q.DatePlayed == today);

            if (!alreadyPlayed)
            {
                user.RewardPoints += 1;
                _context.QuizHistories.Add(new QuizHistory { UserId = user.Id, DatePlayed = today });

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Chúc mừng! Bạn nhận được 1 điểm thưởng.", newPoints = user.RewardPoints });
                }
            }
            return BadRequest("Bạn đã nhận thưởng hôm nay rồi.");
        }
    }
}
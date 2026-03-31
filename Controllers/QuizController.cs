using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEBCOMIC_FINALPROJECT_.Data;
using WEBCOMIC_FINALPROJECT_.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

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

            // Kiểm tra xem đã chơi loại nào rồi
            ViewBag.HasPlayedManga = await _context.QuizHistories.AnyAsync(q => q.UserId == user.Id && q.DatePlayed == today && q.QuizType == "Manga");
            ViewBag.HasPlayedNovel = await _context.QuizHistories.AnyAsync(q => q.UserId == user.Id && q.DatePlayed == today && q.QuizType == "Novel");

            return View();
        }

        // 2A. API Lấy 10 câu hỏi MANGA
        [HttpGet]
        public async Task<IActionResult> StartQuiz()
        {
            var user = await _userManager.GetUserAsync(User);
            var today = DateTime.Today;

            if (await _context.QuizHistories.AnyAsync(q => q.UserId == user.Id && q.DatePlayed == today && q.QuizType == "Manga"))
                return BadRequest("Hôm nay bạn đã nhận thưởng Manga rồi!");

            var randomImages = await _context.Images
                .Include(i => i.Chapter).ThenInclude(c => c.Manga)
                .OrderBy(r => Guid.NewGuid()).Take(10)
                .Select(i => new { ImageUrl = i.Url, MangaTitle = i.Chapter.Manga.Title })
                .ToListAsync();

            if (randomImages.Count < 10) return BadRequest("Hệ thống chưa đủ 10 ảnh Manga để tạo Quiz.");
            return Json(randomImages);
        }

        // 2B. API Lấy 10 câu hỏi NOVEL (MỚI)
        [HttpGet]
        public async Task<IActionResult> StartNovelQuiz()
        {
            var user = await _userManager.GetUserAsync(User);
            var today = DateTime.Today;

            if (await _context.QuizHistories.AnyAsync(q => q.UserId == user.Id && q.DatePlayed == today && q.QuizType == "Novel"))
                return BadRequest("Hôm nay bạn đã nhận thưởng Novel rồi!");

            // Lấy ngẫu nhiên 10 truyện chữ có ảnh bìa
            var randomNovels = await _context.Novels
                .Where(n => !string.IsNullOrEmpty(n.ImageUrl))
                .OrderBy(r => Guid.NewGuid()).Take(10)
                .Select(n => new { ImageUrl = n.ImageUrl, MangaTitle = n.Title }) // Giữ nguyên tên biến MangaTitle để View dễ xài chung
                .ToListAsync();

            if (randomNovels.Count < 10) return BadRequest("Hệ thống chưa đủ 10 ảnh bìa Novel để tạo Quiz.");
            return Json(randomNovels);
        }

        // 3. API Nhận thưởng (Dùng chung)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClaimReward(string type) // Truyền thêm tham số type ("Manga" hoặc "Novel")
        {
            var user = await _userManager.GetUserAsync(User);
            var today = DateTime.Today;

            bool alreadyPlayed = await _context.QuizHistories.AnyAsync(q => q.UserId == user.Id && q.DatePlayed == today && q.QuizType == type);

            if (!alreadyPlayed)
            {
                user.RewardPoints += 1;
                _context.QuizHistories.Add(new QuizHistory { UserId = user.Id, DatePlayed = today, QuizType = type });

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    await _context.SaveChangesAsync();
                    return Ok(new { message = $"Chúc mừng! Bạn nhận được 1 điểm thưởng từ Quiz {type}.", newPoints = user.RewardPoints });
                }
            }
            return BadRequest($"Bạn đã nhận thưởng {type} hôm nay rồi.");
        }
    }
}
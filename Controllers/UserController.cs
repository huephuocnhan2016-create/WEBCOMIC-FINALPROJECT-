using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WEBCOMIC_FINALPROJECT_.Data;
using WEBCOMIC_FINALPROJECT_.Models;

namespace WEBCOMIC_FINALPROJECT_.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            if (user.IsVip && user.VipExpiryDate < DateTime.Now)
            {
                user.IsVip = false;
                await _userManager.UpdateAsync(user);
            }

            ViewBag.PointsToVip = _context.SystemConfigs.FirstOrDefault(c => c.Key == "PointsToVip")?.Value ?? 10;
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RedeemVip() // Đã sửa tên từ UpgradeVip -> RedeemVip
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var requiredPoints = _context.SystemConfigs.FirstOrDefault(c => c.Key == "PointsToVip")?.Value ?? 10;

            if (user.RewardPoints < requiredPoints)
            {
                TempData["Error"] = $"Bạn không đủ điểm. Cần {requiredPoints} điểm.";
                return RedirectToAction("Profile");
            }

            user.RewardPoints -= requiredPoints;
            user.IsVip = true;

            // Tính toán ngày hết hạn (cộng dồn 7 ngày)
            DateTime start = (user.VipExpiryDate.HasValue && user.VipExpiryDate.Value > DateTime.Now)
                             ? user.VipExpiryDate.Value
                             : DateTime.Now;
            user.VipExpiryDate = start.AddDays(7);

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = $"Đổi VIP thành công! Hạn dùng: {user.VipExpiryDate?.ToString("dd/MM/yyyy")}";
            }
            return RedirectToAction("Profile");
        }
    }
}
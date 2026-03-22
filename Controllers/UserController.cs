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
            ViewBag.PointsToVip = _context.SystemConfigs.FirstOrDefault(c => c.Key == "PointsToVip")?.Value ?? 50;
            return View(user);
        }

        public async Task CheckVipStatus(ApplicationUser user)
        {
            if (user.IsVip && user.VipExpiryDate < DateTime.Now)
            {
                user.IsVip = false;
                user.VipExpiryDate = null;
                await _userManager.UpdateAsync(user);
            }
        }
        [HttpPost]
        public async Task<IActionResult> RedeemVip()
        {
            var user = await _userManager.GetUserAsync(User);
            var requiredPoints = _context.SystemConfigs.FirstOrDefault(c => c.Key == "PointsToVip")?.Value ?? 50;

            if (user.RewardPoints >= requiredPoints)
            {
                user.RewardPoints -= requiredPoints;
                user.IsVip = true;
                // Cộng dồn 7 ngày nếu đang là VIP, nếu không thì tính từ hôm nay
                DateTime start = (user.VipExpiryDate > DateTime.Now) ? user.VipExpiryDate.Value : DateTime.Now;
                user.VipExpiryDate = start.AddDays(7);

                await _userManager.UpdateAsync(user);
                TempData["Success"] = "Đổi VIP 1 tuần thành công!";
            }
            else
            {
                TempData["Error"] = "Bạn không đủ điểm.";
            }
            return RedirectToAction("Profile");

        }
        [HttpPost]
public async Task<IActionResult> RequestAuthor()
        {
            var user = await _userManager.GetUserAsync(User);
            // Lưu yêu cầu vào một bảng tạm hoặc gửi Email cho Admin
            TempData["Success"] = "Yêu cầu của bạn đã được gửi. Admin sẽ sớm xét duyệt!";
            return RedirectToAction("Profile");
        }
    }
}

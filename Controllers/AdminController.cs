using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEBCOMIC_FINALPROJECT_.Data;
using WEBCOMIC_FINALPROJECT_.Models;

namespace WEBCOMIC_FINALPROJECT_.Controllers
{
    // Chỉ những người có Role Admin mới được phép truy cập vào Controller này
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // Hiển thị danh sách User và cấu hình hệ thống
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userList = new List<UserWithRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new UserWithRolesViewModel
                {
                    User = user,
                    Roles = roles.ToList()
                });
            }

            // Sử dụng đường dẫn đầy đủ để tránh lỗi nhập nhằng (Ambiguous reference)
            var pointsToVipConfig = await _context.SystemConfigs
                .FirstOrDefaultAsync(c => c.Key == "PointsToVip");

            ViewBag.PointsToVip = pointsToVipConfig?.Value ?? 100; // Mặc định 100 nếu chưa có
            ViewBag.AllRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

            return View(userList);
        }

        // Chức năng thay đổi quyền (Role) của User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);

            // Xóa quyền cũ và thêm quyền mới
            var result = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, roleName);
                TempData["Success"] = $"Đã đổi quyền của {user.Email} sang {roleName}";
            }

            return RedirectToAction(nameof(Index));
        }

        // Chức năng cập nhật cấu hình điểm đổi VIP
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVipConfig(int points)
        {
            // Tìm kiếm cấu hình trong database
            var config = await _context.SystemConfigs.FirstOrDefaultAsync(c => c.Key == "PointsToVip");

            if (config == null)
            {
                // Chỉ định rõ ràng lớp SystemConfig từ namespace Models
                _context.SystemConfigs.Add(new WEBCOMIC_FINALPROJECT_.Models.SystemConfig
                {
                    Key = "PointsToVip",
                    Value = points
                });
            }
            else
            {
                config.Value = points;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Mức điểm đổi VIP đã được cập nhật thành {points} điểm.";

            return RedirectToAction(nameof(Index));
        }
    }

    // ViewModel hỗ trợ hiển thị dữ liệu
    public class UserWithRolesViewModel
    {
        public ApplicationUser User { get; set; }
        public List<string> Roles { get; set; }
    }
}
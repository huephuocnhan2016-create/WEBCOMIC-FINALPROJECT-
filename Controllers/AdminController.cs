using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEBCOMIC_FINALPROJECT_.Data;
using WEBCOMIC_FINALPROJECT_.Models;

namespace WEBCOMIC_FINALPROJECT_.Controllers
{
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

            // Lấy danh sách tất cả các Role để hiển thị trong Dropdown
            ViewBag.AllRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

            // Lấy cấu hình điểm đổi VIP hiện tại
            ViewBag.PointsToVip = await _context.SystemConfigs
                .Where(c => c.Key == "PointsToVip")
                .Select(c => c.Value)
                .FirstOrDefaultAsync();

            return View(userList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // Xóa tất cả quyền cũ và thêm quyền mới
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                TempData["Success"] = $"Đã cập nhật quyền cho {user.Email} thành {roleName}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVipConfig(int points)
        {
            var config = await _context.SystemConfigs.FirstOrDefaultAsync(c => c.Key == "PointsToVip");

            if (config == null)
            {
                _context.SystemConfigs.Add(new SystemConfig { Key = "PointsToVip", Value = points });
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

    public class UserWithRolesViewModel
    {
        public ApplicationUser User { get; set; }
        public List<string> Roles { get; set; }
    }
}
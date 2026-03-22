using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WEBCOMIC_FINALPROJECT_.Data;
using WEBCOMIC_FINALPROJECT_.Models;
using Microsoft.EntityFrameworkCore;
namespace WEBCOMIC_FINALPROJECT_.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, roleName);
            return RedirectToAction("Index");
        }

        [HttpPost]
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
            return Ok();
        }
    }
}

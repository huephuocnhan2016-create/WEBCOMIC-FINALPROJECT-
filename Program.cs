using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WEBCOMIC_FINALPROJECT_.Data;
using WEBCOMIC_FINALPROJECT_.Models;

var builder = WebApplication.CreateBuilder(args);

// --- PHẦN 1: ĐĂNG KÝ SERVICES ---

// 1. Lấy Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// 2. Đăng ký DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 3. Cấu hình Identity (Sử dụng ApplicationUser và IdentityRole)
// LƯU Ý: Đã xóa AddDefaultIdentity để tránh xung đột
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.SignIn.RequireConfirmedAccount = false; // Tắt xác nhận email để dễ test
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI(); // Cần thiết để Identity Razor Pages (Login/Register) hoạt động

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// 4. Cấu hình Phân quyền (Policies)
builder.Services.AddAuthorization(options => {
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ModeratorOnly", policy => policy.RequireRole("Moderator"));
    options.AddPolicy("AuthorOnly", policy => policy.RequireRole("Author"));
});

var app = builder.Build();

// --- PHẦN 2: CẤU HÌNH PIPELINE (MIDDLEWARE) ---

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Thứ tự Authentication TRƯỚC Authorization là bắt buộc
app.UseAuthentication();
app.UseAuthorization();

// Cấu hình Route cho Controller
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Ánh xạ các trang Identity (Login, Register, v.v.)
app.MapRazorPages();

// --- PHẦN 3: SEED DATA (CHẠY KHI ỨNG DỤNG KHỞI ĐỘNG) ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Khởi tạo Roles, Admin User và System Configs
        await SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Một lỗi đã xảy ra trong quá trình Seed dữ liệu.");
    }
}

app.Run();
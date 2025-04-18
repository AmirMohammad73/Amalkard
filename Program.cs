using EmployeePerformanceSystem.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// اضافه کردن سرویس‌های MVC
builder.Services.AddControllersWithViews();

// ثبت کانتکست دیتابیس
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// پیکربندی Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Strict;

    // در محیط Production فعال شود
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.None
        : CookieSecurePolicy.Always;
});

// اضافه کردن سرویس HttpContextAccessor برای دسترسی به HttpContext در کنترلرها
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// ایجاد پوشه uploads در صورت عدم وجود
var uploadsFolder = Path.Combine(app.Environment.WebRootPath, "uploads");
if (!Directory.Exists(uploadsFolder))
{
    Directory.CreateDirectory(uploadsFolder);
}

// پیکربندی خطاها
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// پیکربندی میدلورها
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

// پیکربندی مسیرها
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "performance",
    pattern: "Performance/{action=Index}/{id?}");

// میدلور مدیریت لاگاوت
app.Use(async (context, next) =>
{
    await next();

    if (context.Request.Path.Equals("/Login/Logout", StringComparison.OrdinalIgnoreCase) &&
        context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
    {
        context.Session.Clear();
        await context.Session.CommitAsync();
        context.Response.Cookies.Delete(".AspNetCore.Session");
    }
});

app.Run();
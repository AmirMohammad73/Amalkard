using EmployeePerformanceSystem.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// اضافه کردن سرویس‌های MVC
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

// ثبت کانتکست دیتابیس
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    // در محیط Development این خط را غیرفعال کنید
    // options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});
var app = builder.Build();

// پیکربندی مسیرهای برنامه
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Login}/{action=Index}/{id?}");
app.MapControllerRoute(name: "performance", pattern: "Performance/{action=Index}/{id?}");
app.Use(
    async (context, next) =>
    {
        await next();

        // فقط برای درخواست‌های Logout اقدام کنیم
        if (
            context.Request.Path.Equals("/Login/Logout", StringComparison.OrdinalIgnoreCase)
            && context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase)
        )
        {
            context.Session.Clear();
            _ = context.Session.CommitAsync();
            context.Response.Cookies.Delete(".AspNetCore.Session");
        }
    }
);
app.MapControllers();
app.Run();

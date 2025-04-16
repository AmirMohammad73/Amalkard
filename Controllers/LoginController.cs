using System.Linq;
using EmployeePerformanceSystem.Data;
using EmployeePerformanceSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeePerformanceSystem.Controllers
{
    public class LoginController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // بررسی وجود Session
            var fullname = HttpContext.Session.GetString("Fullname");
            if (!string.IsNullOrEmpty(fullname))
            {
                // اگر Session وجود ندارد، به صفحه Login هدایت شود
                return RedirectToAction("Index", "Login");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Authenticate(User user)
        {
            var existingUser = _context.User.FirstOrDefault(u =>
                u.username == user.username && u.password_hash == user.password_hash
            );

            if (existingUser != null)
            {
                // تنظیم Session با گزینه‌های امنیتی
                HttpContext.Session.SetString("Fullname", existingUser.fullname);
                HttpContext.Session.SetInt32("OfficePermission", existingUser.office_permission);
                HttpContext.Session.SetInt32("OstanPermission", existingUser.ostan_permission);

                // تنظیم کوکی Session
                Response.Cookies.Append(
                    ".AspNetCore.Session",
                    HttpContext.Session.Id,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                    }
                );

                return RedirectToAction("Index", "Record");
            }

            if (existingUser.is_blacklisted)
            {
                ViewBag.ErrorMessage = "حساب شما مسدود شده است.";
                return View("Index");
            }

            return View("Index");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            // 1. حذف تمام داده‌های Session
            HttpContext.Session.Clear();

            // 2. باطل کردن Session (بدون await)
            _ = HttpContext.Session.CommitAsync();

            // 3. حذف کوکی Session
            Response.Cookies.Delete(
                ".AspNetCore.Session",
                new CookieOptions
                {
                    Path = "/",
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                }
            );

            return RedirectToAction("Index", "Login");
        }
    }
}

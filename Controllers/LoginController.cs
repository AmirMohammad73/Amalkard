using EmployeePerformanceSystem.Data;
using EmployeePerformanceSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace EmployeePerformanceSystem.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [HttpPost]
        public IActionResult Authenticate(User user)
        {
            var existingUser = _context.User
                .FirstOrDefault(u => u.username == user.username && u.password_hash == user.password_hash);

            if (existingUser == null)
            {
                ViewBag.ErrorMessage = "نام کاربری یا رمز عبور اشتباه است.";
                return View("Index");
            }

            if (existingUser.is_blacklisted)
            {
                ViewBag.ErrorMessage = "حساب شما مسدود شده است.";
                return View("Index");
            }

            HttpContext.Session.SetString("Fullname", existingUser.fullname);
            ViewBag.Message = "Login successful!";
            return RedirectToAction("Index", "Record");
        }
    }
}

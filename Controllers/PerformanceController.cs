using System.Globalization;
using EmployeePerformanceSystem.Data;
using EmployeePerformanceSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeePerformanceSystem.Controllers
{
    public class PerformanceController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public PerformanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetPerformanceData(int userId)
        {
            // دریافت اطلاعات کاربر جاری از Session
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            var currentUser = _context.User.FirstOrDefault(u => u.id == currentUserId);

            var records = _context
                .MonthlyRecords.Where(m => m.user_id == userId)
                .OrderBy(m => m.month)
                .ToList();

            var formattedRecords = records
                .Select(r => new
                {
                    month = r.month,
                    monthName = GetPersianMonthName(r.month),
                    work = r.work,
                    vacation = r.vacation,
                    vacation_sick = r.vacation_sick,
                    mission = r.mission,
                    overtime_system = r.overtime_system,
                    overtime_final = r.overtime_final,
                    sum_work = r.sum_work,
                    // اضافه کردن وضعیت دسترسی کاربر
                    canEditOvertimeFinal = currentUser?.office_permission == 0
                        && currentUser?.ostan_permission == 0,
                })
                .ToList();
            Console.WriteLine(currentUser?.office_permission);
            return Json(formattedRecords);
        }

        [HttpPost]
        public IActionResult SavePerformanceData([FromBody] List<MonthlyRecord> records)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            var currentUser = _context.User.FirstOrDefault(u => u.id == currentUserId);

            if (ModelState.IsValid)
            {
                foreach (var record in records)
                {
                    var exists = _context.MonthlyRecords.Any(m =>
                        m.user_id == record.user_id && m.month == record.month
                    );

                    if (exists)
                    {
                        // به‌روزرسانی رکورد موجود بدون sum_work
                        string sql =
                            $@"
                            UPDATE MonthlyRecords
                            SET
                                work = {record.work ?? 0},
                                vacation = {record.vacation ?? 0},
                                vacation_sick = {record.vacation_sick ?? 0},
                                mission = {record.mission ?? 0},
                                overtime_system = {record.overtime_system ?? 0}
                                {(currentUser?.office_permission == 0 && currentUser?.ostan_permission == 0
                                    ? $", overtime_final = {record.overtime_final ?? 0}" : "")}
                            WHERE user_id = {record.user_id} AND month = {record.month};
                            ";
                        _context.Database.ExecuteSqlRaw(sql);
                    }
                    else
                    {
                        // درج رکورد جدید (در صورت داشتن مجوز) بدون sum_work
                        if (
                            currentUser?.office_permission == 0
                            && currentUser?.ostan_permission == 0
                        )
                        {
                            string sql =
                                $@"
                                INSERT INTO MonthlyRecords
                                (user_id, month, work, vacation, vacation_sick, mission, overtime_system, overtime_final)
                                VALUES
                                ({record.user_id}, {record.month}, {record.work ?? 0}, {record.vacation ?? 0},
                                {record.vacation_sick ?? 0}, {record.mission ?? 0}, {record.overtime_system ?? 0},
                                {record.overtime_final ?? 0});
                                ";
                            _context.Database.ExecuteSqlRaw(sql);
                        }
                        else
                        {
                            Console.WriteLine(
                                $"کاربر مجوز ایجاد رکورد جدید را ندارد. user_id={record.user_id}, month={record.month}"
                            );
                        }
                    }
                }
                return Json(new { success = true });
            }
            else
            {
                Console.WriteLine("خطا در اعتبارسنجی مدل.");
                return Json(new { success = false });
            }
        }

        // کلاس برای ذخیره مقادیر خروجی
        public class TempOutput
        {
            public int id { get; set; }
            public int? overtime_final { get; set; }
            public int? sum_work { get; set; }
        }

        private string GetPersianMonthName(byte month)
        {
            var persianMonthNames = new[]
            {
                "فروردین",
                "اردیبهشت",
                "خرداد",
                "تیر",
                "مرداد",
                "شهریور",
                "مهر",
                "آبان",
                "آذر",
                "دی",
                "بهمن",
                "اسفند",
            };

            return month >= 0 && month <= 11 ? persianMonthNames[month] : "نامشخص";
        }
    }
}

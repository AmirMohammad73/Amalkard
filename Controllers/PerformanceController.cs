using System.Globalization;
using EmployeePerformanceSystem.Data;
using EmployeePerformanceSystem.Models;
using Microsoft.AspNetCore.Mvc;

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
            // دریافت اطلاعات کاربر جاری از Session
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            var currentUser = _context.User.FirstOrDefault(u => u.id == currentUserId);
            Console.WriteLine("HEY YOU!");
            if (ModelState.IsValid)
            {
                foreach (var record in records)
                {
                    var existingRecord = _context.MonthlyRecords.FirstOrDefault(m =>
                        m.user_id == record.user_id && m.month == record.month
                    );

                    if (existingRecord != null)
                    {
                        // به‌روزرسانی رکورد موجود
                        existingRecord.work = record.work;
                        existingRecord.vacation = record.vacation;
                        existingRecord.vacation_sick = record.vacation_sick;
                        existingRecord.mission = record.mission;
                        existingRecord.overtime_system = record.overtime_system;

                        // فقط اگر کاربر مجوز دارد، مقدار overtime_final را به‌روزرسانی کنید
                        if (
                            currentUser?.office_permission == 0
                            && currentUser?.ostan_permission == 0
                        )
                        {
                            existingRecord.overtime_final = record.overtime_final;
                        }

                        existingRecord.sum_work = record.sum_work;
                    }
                    else
                    {
                        // اگر رکورد جدید است و کاربر مجوز دارد
                        if (
                            currentUser?.office_permission == 0
                            && currentUser?.ostan_permission == 0
                        )
                        {
                            _context.MonthlyRecords.Add(record);
                        }
                        else
                        {
                            Console.WriteLine(
                                $"کاربر مجوز ایجاد رکورد جدید را ندارد. user_id={record.user_id}, month={record.month}"
                            );
                        }
                    }
                }

                _context.SaveChanges();
                return Json(new { success = true });
            }
            else
            {
                Console.WriteLine("خطا در اعتبارسنجی مدل.");
                return Json(new { success = false });
            }
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

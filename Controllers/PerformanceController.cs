// Controllers/PerformanceController.cs
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
            var records = _context
                .MonthlyRecords.Where(m => m.user_id == userId)
                .OrderBy(m => m.month)
                .ToList();

            var formattedRecords = records
                .Select(r => new
                {
                    monthName = GetPersianMonthName(r.month),
                    work = r.work,
                    vacation = r.vacation,
                    vacation_sick = r.vacation_sick,
                    mission = r.mission,
                    overtime_system = r.overtime_system,
                    overtime_final = r.overtime_final,
                    sum_work = r.sum_work,
                })
                .ToList();

            return Json(formattedRecords);
        }

        [HttpPost]
        public IActionResult SavePerformanceData([FromBody] List<MonthlyRecord> records)
        {
            if (ModelState.IsValid)
            {
                foreach (var record in records)
                {
                    // جستجوی رکورد موجود بر اساس user_id و month
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
                        existingRecord.overtime_final = record.overtime_final;
                        existingRecord.sum_work = record.sum_work;
                    }
                    else
                    {
                        // اگر رکوردی وجود نداشت، خطایی ثبت کنید یا هیچ عملی انجام ندهید
                        Console.WriteLine(
                            $"رکوردی برای user_id={record.user_id} و month={record.month} وجود ندارد."
                        );
                    }
                }

                _context.SaveChanges(); // ذخیره تغییرات در دیتابیس
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

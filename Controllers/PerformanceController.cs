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
            Console.WriteLine("BINGO");
            // دریافت اطلاعات کاربر جاری از Session
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            var currentUser = _context.User.FirstOrDefault(u => u.id == currentUserId);

            if (currentUser == null)
            {
                return Json(new { success = false, message = "کاربر جاری یافت نشد." });
            }

            try
            {
                // ایجاد جدول موقت برای ذخیره خروجی
                _context.Database.ExecuteSqlRaw(
                    @"
            CREATE TABLE #TempOutput (
                id INT,
                overtime_final INT,
                sum_work INT
            );
        "
                );

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
                        if (currentUser.office_permission == 0 && currentUser.ostan_permission == 0)
                        {
                            existingRecord.overtime_final = record.overtime_final;
                        }

                        existingRecord.sum_work = record.sum_work;

                        // استفاده از OUTPUT INTO برای ذخیره خروجی
                        _context.Database.ExecuteSqlRaw(
                            @"
                    UPDATE MonthlyRecords
                    SET 
                        work = {0},
                        vacation = {1},
                        vacation_sick = {2},
                        mission = {3},
                        overtime_system = {4},
                        overtime_final = {5},
                        sum_work = {6}
                    OUTPUT INSERTED.id, INSERTED.overtime_final, INSERTED.sum_work INTO #TempOutput
                    WHERE id = {7};
                ",
                            existingRecord.work,
                            existingRecord.vacation,
                            existingRecord.vacation_sick,
                            existingRecord.mission,
                            existingRecord.overtime_system,
                            existingRecord.overtime_final,
                            existingRecord.sum_work,
                            existingRecord.id
                        );
                    }
                    else
                    {
                        // اگر رکورد جدید است و کاربر مجوز دارد
                        if (currentUser.office_permission == 0 && currentUser.ostan_permission == 0)
                        {
                            // اضافه کردن رکورد جدید
                            _context.Database.ExecuteSqlRaw(
                                @"
                        INSERT INTO MonthlyRecords (
                            user_id, month, work, vacation, vacation_sick, mission, overtime_system, overtime_final, sum_work
                        )
                        OUTPUT INSERTED.id, INSERTED.overtime_final, INSERTED.sum_work INTO #TempOutput
                        VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8});
                    ",
                                record.user_id,
                                record.month,
                                record.work,
                                record.vacation,
                                record.vacation_sick,
                                record.mission,
                                record.overtime_system,
                                record.overtime_final,
                                record.sum_work
                            );
                        }
                        else
                        {
                            Console.WriteLine(
                                $"کاربر مجوز ایجاد رکورد جدید را ندارد. user_id={record.user_id}, month={record.month}"
                            );
                        }
                    }
                }

                // ذخیره تغییرات در دیتابیس
                _context.SaveChanges();

                // خواندن مقادیر خروجی از جدول موقت
                var outputRecords = _context
                    .Database.SqlQueryRaw<TempOutput>(
                        @"
            SELECT id, overtime_final, sum_work FROM #TempOutput;
        "
                    )
                    .ToList();

                // حذف جدول موقت
                _context.Database.ExecuteSqlRaw("DROP TABLE #TempOutput;");

                return Json(new { success = true, outputRecords });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطا در ذخیره داده‌ها: {ex.Message}");
                return Json(new { success = false, message = "خطا در ذخیره داده‌ها." });
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

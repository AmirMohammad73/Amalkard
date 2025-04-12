// Controllers/PerformanceController.cs
using EmployeePerformanceSystem.Data;
using EmployeePerformanceSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

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
            var records = _context.MonthlyRecords
                .Where(m => m.user_id == userId)
                .OrderBy(m => m.month)
                .ToList();
            var persianCalendar = new PersianCalendar();
            var formattedRecords = records.Select(r => new
            {
                monthName = GetPersianMonthName(r.month),
                work = r.work,
                vacation = r.vacation,
                vacation_sick = r.vacation_sick,
                mission = r.mission,
                overtime_system = r.overtime_system,
                overtime_final = r.overtime_final,
                sum_work = r.sum_work
            }).ToList();

            return Json(formattedRecords);
        }

        [HttpPost]
        public IActionResult SavePerformanceData([FromBody] MonthlyRecord model)
        {
            if (ModelState.IsValid)
            {
                var existingRecord = _context.MonthlyRecords
                    .FirstOrDefault(m => m.user_id == model.user_id && m.month == model.month);

                if (existingRecord != null)
                {
                    // Update existing record
                    existingRecord.work = model.work;
                    existingRecord.vacation = model.vacation;
                    existingRecord.vacation_sick = model.vacation_sick;
                    existingRecord.mission = model.mission;
                    existingRecord.overtime_system = model.overtime_system;
                    existingRecord.overtime_final = model.overtime_final;
                    existingRecord.sum_work = model.sum_work;
                }
                else
                {
                    // Create new record
                    _context.MonthlyRecords.Add(model);
                }

                _context.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        private string GetPersianMonthName(int month)
        {
            var persianMonthNames = new[]
            {
                "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
                "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
            };

            return month >= 1 && month <= 12 ? persianMonthNames[month - 1] : "نامشخص";
        }
    }
}
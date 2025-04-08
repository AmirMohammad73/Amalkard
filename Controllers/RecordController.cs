using Microsoft.AspNetCore.Mvc;
using MyProject.Models;
using EmployeePerformanceSystem.Data;
using System.Collections.Generic;
using System.Linq;

namespace MyProject.Controllers
{
    public class RecordController : Controller
    {
        private readonly ApplicationDbContext _context;
        public RecordController(ApplicationDbContext context)
        {
            _context = context;
        }
        private static List<Record> Records = new List<Record>
        {
            new Record
            {
                Id = 1,
                FirstName = "Giacomo",
                LastName = "Guilizzoni",
                NationalCode = "1234567890",
                FatherName = "Pelidi",
                BirthDate = "",
                BirthCity = "تهران",
                CertificateIssuePlace = "تهران",
                EducationCert = "مهندسی عمران",
                EmploymentStartDate = "",
                MaritalStatus = "متاهل",
                ChildrenCount = 2,
                IsHeadOfHousehold = true,
                Sheba = "123456789101213141516171",
                BankName = "بانک سپه",
                HasInsurance = true,
                InsuranceNumber = 1234567890,
                InsuranceHistory = 5079
            }
        };

        public IActionResult Index()
        {
            // خواندن fullname از Session
            var fullname = HttpContext.Session.GetString("Fullname");

            // خواندن لیست ادارات و استان‌ها از دیتابیس
            var offices = _context.Offices.ToList();
            var provinces = _context.Provinces.ToList();

            // خواندن مقدار is_national برای اداره انتخاب‌شده
            var selectedOfficeId = HttpContext.Session.GetInt32("SelectedOfficeId");

            // اگر selectedOfficeId null بود و لیست ادارات خالی نباشد، اولین اداره را انتخاب کنید
            if (selectedOfficeId == null && offices.Any())
            {
                selectedOfficeId = offices.First().id;
                HttpContext.Session.SetInt32("SelectedOfficeId", selectedOfficeId.Value);
            }

            var selectedOffice = _context.Offices.FirstOrDefault(o => o.id == selectedOfficeId);
            var isNational = selectedOffice?.is_national ?? false;

            ViewBag.Jobs = new List<string> { "فهرست بردار", "آمارگر" };
            ViewBag.Degrees = new List<string> { "کارشناسی", "کارشناسی ارشد", "دکترا" };
            ViewBag.Fullname = fullname;
            ViewBag.Offices = offices;
            ViewBag.IsNational = isNational;
            ViewBag.SelectedOfficeId = selectedOfficeId; // اضافه کردن این خط برای استفاده در ویو
            ViewBag.Provinces = provinces; // اضافه کردن لیست استان‌ها به ViewBag

            return View(Records);
        }
        [HttpPost]
        public IActionResult SetOffice(int officeId)
        {
            // بررسی مقدار is_national برای اداره انتخاب‌شده
            var selectedOffice = _context.Offices.FirstOrDefault(o => o.id == officeId);
            if (selectedOffice != null)
            {
                HttpContext.Session.SetString("SelectedOfficeName", selectedOffice.name);
                HttpContext.Session.SetInt32("SelectedOfficeId", selectedOffice.id);

                return Json(new { isNational = selectedOffice.is_national });
            }

            return Json(new { isNational = false });
        }
        public IActionResult FromLogin()
        {
            // خواندن fullname از Session
            var fullname = HttpContext.Session.GetString("Fullname");

            // تنظیم اداره پیش‌فرض اگر وجود ندارد
            var offices = _context.Offices.ToList();
            if (offices.Any() && !HttpContext.Session.TryGetValue("SelectedOfficeId", out _))
            {
                HttpContext.Session.SetInt32("SelectedOfficeId", offices.First().id);
            }

            ViewBag.Fullname = fullname;
            return View();
        }
        [HttpGet]
        public IActionResult GetCurrentOffice()
        {
            var selectedOfficeId = HttpContext.Session.GetInt32("SelectedOfficeId");
            var selectedOffice = _context.Offices.FirstOrDefault(o => o.id == selectedOfficeId);

            return Json(new
            {
                officeId = selectedOffice?.id,
                isNational = selectedOffice?.is_national ?? false
            });
        }
        [HttpPost]
        public IActionResult AddRecord()
        {
            var newRecord = new Record
            {
                Id = Records.Count + 1
            };
            Records.Add(newRecord);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EditRecord(Record record)
        {
            var existingRecord = Records.FirstOrDefault(r => r.Id == record.Id);
            if (existingRecord != null)
            {
                existingRecord.FirstName = record.FirstName;
                existingRecord.LastName = record.LastName;
                existingRecord.NationalCode = record.NationalCode;
                existingRecord.FatherName = record.FatherName;
                existingRecord.BirthDate = record.BirthDate;
                existingRecord.BirthCity = record.BirthCity;
                existingRecord.CertificateIssuePlace = record.CertificateIssuePlace;
                existingRecord.EducationDegree = record.EducationDegree;
                existingRecord.EducationCert = record.EducationCert;
                existingRecord.Job = record.Job;
                existingRecord.EmploymentStartDate = record.EmploymentStartDate;
                existingRecord.MaritalStatus = record.MaritalStatus;
                existingRecord.ChildrenCount = record.ChildrenCount;
                existingRecord.IsHeadOfHousehold = record.IsHeadOfHousehold;
                existingRecord.Sheba = record.Sheba;
                existingRecord.BankName = record.BankName;
                existingRecord.HasInsurance = record.HasInsurance;
                existingRecord.InsuranceNumber = record.InsuranceNumber;
                existingRecord.InsuranceHistory = record.InsuranceHistory;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteRecord(int id)
        {
            var record = Records.FirstOrDefault(r => r.Id == id);
            if (record != null)
            {
                Records.Remove(record);
            }
            return RedirectToAction("Index");
        }
    }
}
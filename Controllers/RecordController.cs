using Microsoft.AspNetCore.Mvc;
using EmployeePerformanceSystem.Models;
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
                firstName = "Giacomo",
                lastName = "Guilizzoni",
                national_id = "1234567890",
                father_name = "Pelidi",
                birthdate = "",
                b_city = "تهران",
                p_city = "تهران",
                cert = "مهندسی عمران",
                startdate = "",
                is_married = false,
                children_no = 2,
                is_head = true,
                Sheba = "123456789101213141516171",
                bank_name = "بانک سپه",
                has_insurance = true,
                insurance_number = "1234567890",
                insurance_days = 5079
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

        // خواندن رکوردها از دیتابیس با شرط is_deleted = false
        var records = _context.Records
            .Where(r => !r.is_deleted)
            .Select(r => new Record
            {
                Id = r.Id,
                firstName = r.firstName,
                lastName = r.lastName,
                national_id = r.national_id,
                father_name = r.father_name,
                birthdate = r.birthdate,
                b_city = r.b_city,
                p_city = r.p_city,
                degree = r.degree,
                cert = r.cert,
                Job = r.Job,
                startdate = r.startdate,
                is_married = r.is_married,
                children_no = r.children_no,
                is_head = r.is_head,
                Sheba = r.Sheba,
                bank_name = r.bank_name,
                has_insurance = r.has_insurance,
                insurance_number = r.insurance_number,
                insurance_days = r.insurance_days
            })
            .ToList();

        ViewBag.Jobs = new List<string> { "فهرست بردار", "آمارگر" };
        ViewBag.Degrees = new List<string> { "کارشناسی", "کارشناسی ارشد", "دکترا" };
        ViewBag.Fullname = fullname;
        ViewBag.Offices = offices;
        ViewBag.IsNational = isNational;
        ViewBag.SelectedOfficeId = selectedOfficeId;
        ViewBag.Provinces = provinces;

        return View(records);
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
[HttpPost]
public IActionResult AddRecord()
{
    var newRecord = new EmployeePerformanceSystem.Models.Record
    {
        firstName = "",
        lastName = "",
        national_id = "",
        father_name = "",
        birthdate = "",
        b_city = "",
        p_city = "",
        degree = 0,
        cert = "",
        Job = 0,
        startdate = "",
        is_married = false,
        children_no = 0,
        is_head = false,
        Sheba = "",
        bank_name = "",
        has_insurance = false,
        insurance_number = "",
        insurance_days = 0,
        is_deleted = false
    };

    _context.Records.Add(newRecord);
    _context.SaveChanges();

    return RedirectToAction("Index");
}

[HttpPost]
public IActionResult EditRecord(List<Record> records)
{
    foreach (var record in records)
    {
        if (ModelState.IsValid) // بررسی اعتبارسنجی با استفاده از DataAnnotation
        {
            var existingRecord = _context.Records.FirstOrDefault(r => r.Id == record.Id);

            if (existingRecord != null) // ویرایش رکورد موجود
            {
                existingRecord.firstName = record.firstName;
                existingRecord.lastName = record.lastName;
                existingRecord.national_id = record.national_id;
                existingRecord.father_name = record.father_name;
                existingRecord.birthdate = record.birthdate;
                existingRecord.b_city = record.b_city;
                existingRecord.p_city = record.p_city;
                existingRecord.degree = record.degree;
                existingRecord.cert = record.cert;
                existingRecord.Job = record.Job;
                existingRecord.startdate = record.startdate;
                existingRecord.is_married = record.is_married;
                existingRecord.children_no = record.children_no;
                existingRecord.is_head = record.is_head;
                existingRecord.Sheba = record.Sheba;
                existingRecord.bank_name = record.bank_name;
                existingRecord.has_insurance = record.has_insurance;
                existingRecord.insurance_number = record.insurance_number;
                existingRecord.insurance_days = record.insurance_days;
            }
            else // اضافه کردن رکورد جدید
            {
                _context.Records.Add(record);
            }
        }
        else
        {
            // در صورت نامعتبر بودن داده‌ها خطا برگردانده شود
            TempData["ErrorMessage"] = "برخی از ورودی‌ها نامعتبر هستند. لطفاً تمام فیلدها را صحیح پر کنید.";
            return RedirectToAction("Index");
        }
    }

    _context.SaveChanges(); // ذخیره‌سازی تغییرات در دیتابیس
    TempData["SuccessMessage"] = "تغییرات با موفقیت ذخیره شد.";
    return RedirectToAction("Index");
}


[HttpPost]
public IActionResult DeleteRecord(int id)
{
    var record = _context.Records.FirstOrDefault(r => r.Id == id);
    if (record != null)
    {
        // به جای حذف فیزیکی، مقدار is_deleted را true می‌کنیم
        record.is_deleted = true;
        _context.SaveChanges();
    }
    return RedirectToAction("Index");
}
    }
}
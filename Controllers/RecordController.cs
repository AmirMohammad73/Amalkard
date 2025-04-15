using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using EmployeePerformanceSystem.Data;
using EmployeePerformanceSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeePerformanceSystem.Controllers
{
    public class RecordController : BaseController
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
                insurance_days = 5079,
            },
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
            var selectedProvinceId = HttpContext.Session.GetInt32("SelectedProvinceId"); // اضافه کردن این خط

            // اگر selectedOfficeId null بود و لیست ادارات خالی نباشد، اولین اداره را انتخاب کنید
            if (selectedOfficeId == null && offices.Any())
            {
                selectedOfficeId = offices.First().id;
                HttpContext.Session.SetInt32("SelectedOfficeId", selectedOfficeId.Value);
            }

            var selectedOffice = _context.Offices.FirstOrDefault(o => o.id == selectedOfficeId);
            var isNational = selectedOffice?.is_national ?? false;

            // خواندن رکوردها از دیتابیس با فیلتر office_id و ostan_id
            var dbRecords = _context
                .Records.Where(r =>
                    !r.is_deleted
                    && r.office_id == selectedOfficeId
                    && (selectedProvinceId == null || r.ostan_id == selectedProvinceId)
                ) // اضافه کردن این شرط
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
                    insurance_days = r.insurance_days,
                    office_id = r.office_id,
                    ostan_id = r.ostan_id, // اضافه کردن این خط
                })
                .ToList();

            // خواندن رکوردهای موقت از Session
            var tempRecords =
                HttpContext.Session.Get<List<Record>>("TempRecords") ?? new List<Record>();

            // ترکیب رکوردهای دیتابیس و موقت
            var allRecords = dbRecords.Concat(tempRecords).ToList();

            ViewBag.Jobs = new List<string>
            {
                "کارشناس پایگاه داده",
                "کارشناس پشتیبان وب سرویس",
                "کارشناس داده آمائی",
                "کارشناس رایانه",
                "کارشناس ساختمان و تاسیسات",
                "کارشناس فناوری اطلاعات",
                "کارشناس فهرست برداری",
                "کارشناس فهرست برداری و داده آمائی",
                "کارشناس مالی",
                "کارشناس نقشه",
            };
            ViewBag.Degrees = new List<string> { "کارشناسی", "کارشناسی ارشد", "دکترا" };
            ViewBag.Fullname = fullname;
            ViewBag.Offices = offices;
            ViewBag.IsNational = isNational;
            ViewBag.SelectedOfficeId = selectedOfficeId;
            ViewBag.SelectedProvinceId = selectedProvinceId; // اضافه کردن این خط
            ViewBag.Provinces = provinces;

            return View(allRecords);
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

        [HttpPost]
        public IActionResult SetProvince(int provinceId)
        {
            HttpContext.Session.SetInt32("SelectedProvinceId", provinceId);
            return Json(new { success = true });
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

            return Json(
                new
                {
                    officeId = selectedOffice?.id,
                    isNational = selectedOffice?.is_national ?? false,
                }
            );
        }

        [HttpPost]
        [HttpPost]
        public IActionResult AddRecord()
        {
            // ایجاد یک رکورد موقت در حافظه (بدون ذخیره در دیتابیس)
            var newRecord = new Record
            {
                Id = -1, // مقدار موقت برای شناسایی رکوردهای جدید
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
                is_deleted = false,
            };

            // اضافه کردن رکورد به لیست موقت (مثلاً در Session)
            var tempRecords =
                HttpContext.Session.Get<List<Record>>("TempRecords") ?? new List<Record>();
            tempRecords.Add(newRecord);
            HttpContext.Session.Set("TempRecords", tempRecords);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EditRecord(List<Record> records)
        {
            // لاگ کردن داده‌های ارسالی از فرم
            Console.WriteLine("Received Records:");
            foreach (var record in records)
            {
                Console.WriteLine(
                    $"Id: {record.Id}, FirstName: {record.firstName}, LastName: {record.lastName}, Contract: {record.insurance_number}"
                );
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model State is Invalid.");
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
                TempData["ErrorMessage"] =
                    "برخی از ورودی‌ها نامعتبر هستند. لطفاً تمام فیلدها را صحیح پر کنید.";
                return RedirectToAction("Index");
            }

            try
            {
                foreach (var record in records)
                {
                    if (record.Id == -1) // رکورد جدید
                    {
                        // فقط رکوردهای با اطلاعات معتبر را ذخیره کنید
                        if (
                            !string.IsNullOrEmpty(record.firstName)
                            && !string.IsNullOrEmpty(record.lastName)
                            && !string.IsNullOrEmpty(record.national_id)
                        )
                        {
                            record.Id = 0; // تنظیم Id برای رکورد جدید
                            _context.Records.Add(record);
                        }
                    }
                    else
                    {
                        var existingRecord = _context.Records.FirstOrDefault(r =>
                            r.Id == record.Id
                        );
                        if (existingRecord != null)
                        {
                            _context.Entry(existingRecord).CurrentValues.SetValues(record);
                        }
                    }
                }

                _context.SaveChanges();
                TempData["SuccessMessage"] = "تغییرات با موفقیت ذخیره شد.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during saving records: {ex.Message}");
                TempData["ErrorMessage"] = "خطا در ذخیره‌سازی تغییرات.";
            }

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

        [HttpPost]
        public IActionResult UploadContract(int id, IFormFile contractFile)
        {
            if (contractFile == null || contractFile.Length == 0)
            {
                return BadRequest("فایلی ارسال نشده است.");
            }

            // مسیر ذخیره‌سازی تصویر
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var extension = Path.GetExtension(contractFile.FileName);
            var fileName = $"{id}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);
            Console.WriteLine("Full file path: " + filePath);
            // ذخیره فایل
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                contractFile.CopyTo(stream);
            }

            // ذخیره اطلاعات تصویر در دیتابیس
            var record = _context.Records.FirstOrDefault(r => r.Id == id);
            if (record != null)
            {
                record.contract_image = $"/uploads/{fileName}"; // مسیر تصویر
                _context.SaveChanges();
            }

            return Ok(new { message = "تصویر با موفقیت ذخیره شد." });
        }

        [HttpGet]
        public IActionResult GetContractImage(int id)
        {
            var record = _context.Records.FirstOrDefault(r => r.Id == id);
            if (record == null || string.IsNullOrEmpty(record.contract_image))
            {
                return NotFound();
            }

            return Json(new { imageUrl = record.contract_image });
        }
    }

    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}

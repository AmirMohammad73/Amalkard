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
            // خواندن اطلاعات کاربر از Session
            var fullname = HttpContext.Session.GetString("Fullname");
            var officePermission = HttpContext.Session.GetInt32("OfficePermission") ?? 0;
            var ostanPermission = HttpContext.Session.GetInt32("OstanPermission") ?? 0;

            // خواندن لیست ادارات و استان‌ها از دیتابیس
            var offices = _context.Offices.ToList();
            var provinces = _context.Provinces.ToList();

            // فیلتر کردن ادارات و استان‌ها بر اساس مجوزهای کاربر
            if (officePermission != 0)
            {
                offices = offices.Where(o => o.id == officePermission).ToList();
            }

            if (ostanPermission != 0)
            {
                provinces = provinces.Where(p => p.id == ostanPermission).ToList();
            }

            // خواندن مقدار اداره و استان انتخاب‌شده
            var selectedOfficeId = HttpContext.Session.GetInt32("SelectedOfficeId");
            var selectedProvinceId = HttpContext.Session.GetInt32("SelectedProvinceId");

            // مقدار پیش‌فرض برای اداره اگر انتخاب نشده باشد
            if (selectedOfficeId == null && offices.Any())
            {
                selectedOfficeId = offices.First().id;
                HttpContext.Session.SetInt32("SelectedOfficeId", selectedOfficeId.Value);
            }

            // مقدار پیش‌فرض برای استان اگر انتخاب نشده باشد
            if (selectedProvinceId == null && provinces.Any())
            {
                selectedProvinceId = provinces.First().id;
                HttpContext.Session.SetInt32("SelectedProvinceId", selectedProvinceId.Value);
            }

            // خواندن رکوردها از دیتابیس
            var dbRecords = _context
                .Records.Where(r =>
                    !r.is_deleted
                    && r.office_id == selectedOfficeId
                    && r.ostan_id == selectedProvinceId
                )
                .ToList();

            // خواندن رکوردهای موقت از Session
            var tempRecords =
                HttpContext.Session.Get<List<Record>>("TempRecords") ?? new List<Record>();

            // ترکیب رکوردهای دیتابیس و موقت - این قسمت حیاتی است
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
            ViewBag.SelectedOfficeId = selectedOfficeId;
            ViewBag.SelectedProvinceId = selectedProvinceId;
            ViewBag.Provinces = provinces;

            return View(allRecords); // ارسال رکوردهای ترکیبی به ویو
        }

        [HttpPost]
        public IActionResult SetOffice(int officeId)
        {
            // پاک کردن رکوردهای موقت هنگام تغییر اداره
            HttpContext.Session.Remove("TempRecords");

            var selectedOffice = _context.Offices.FirstOrDefault(o => o.id == officeId);
            if (selectedOffice != null)
            {
                HttpContext.Session.SetInt32("SelectedOfficeId", selectedOffice.id);
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult SetProvince(int provinceId)
        {
            // پاک کردن رکوردهای موقت هنگام تغییر استان
            HttpContext.Session.Remove("TempRecords");

            var selectedProvince = _context.Provinces.FirstOrDefault(p => p.id == provinceId);
            if (selectedProvince != null)
            {
                HttpContext.Session.SetInt32("SelectedProvinceId", selectedProvince.id);
                return Json(new { success = true, provinceId = selectedProvince.id });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public IActionResult ClearTempRecords()
        {
            HttpContext.Session.Remove("TempRecords");
            return Ok();
        }

        [HttpGet]
        public IActionResult GetCurrentOffice()
        {
            return Json(
                new
                {
                    officeId = HttpContext.Session.GetInt32("SelectedOfficeId"),
                    provinceId = HttpContext.Session.GetInt32("SelectedProvinceId"),
                }
            );
        }

        [HttpPost]
        public IActionResult AddRecord()
        {
            // بررسی آیا رکورد خالی قبلی وجود دارد که پر نشده باشد
            var tempRecords =
                HttpContext.Session.Get<List<Record>>("TempRecords") ?? new List<Record>();

            // اگر رکورد خالی از قبل وجود دارد، همان را نگه دارید
            if (
                !tempRecords.Any(r =>
                    string.IsNullOrEmpty(r.firstName)
                    && string.IsNullOrEmpty(r.lastName)
                    && string.IsNullOrEmpty(r.national_id)
                )
            )
            {
                // دریافت مقادیر جلسه
                var selectedOfficeId = HttpContext.Session.GetInt32("SelectedOfficeId");
                var selectedProvinceId = HttpContext.Session.GetInt32("SelectedProvinceId");

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
                    office_id = selectedOfficeId,
                    ostan_id = selectedProvinceId, // اینجا مقداردهی می‌شود
                };

                tempRecords.Add(newRecord);
                HttpContext.Session.Set("TempRecords", tempRecords);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EditRecord(List<Record> records)
        {
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
                var selectedOfficeId = HttpContext.Session.GetInt32("SelectedOfficeId");
                var selectedProvinceId = HttpContext.Session.GetInt32("SelectedProvinceId");
                var tempRecords =
                    HttpContext.Session.Get<List<Record>>("TempRecords") ?? new List<Record>();
                var recordsToKeep = new List<Record>();

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
                            // تنظیم مقادیر office_id و ostan_id برای رکورد جدید
                            record.office_id = selectedOfficeId;
                            record.ostan_id = selectedProvinceId;
                            record.Id = 0; // تنظیم Id برای رکورد جدید
                            _context.Records.Add(record);
                        }
                        else
                        {
                            // اگر رکورد جدید معتبر نیست، آن را در لیست موقت نگه دارید
                            recordsToKeep.Add(record);
                        }
                    }
                    else
                    {
                        var existingRecord = _context.Records.FirstOrDefault(r =>
                            r.Id == record.Id
                        );
                        if (existingRecord != null)
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
                    }
                }

                _context.SaveChanges();

                // فقط رکوردهای نامعتبر را در Session نگه دارید
                HttpContext.Session.Set("TempRecords", recordsToKeep);

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
                return BadRequest(new { message = "فایلی انتخاب نشده است" });
            }

            // بررسی نوع فایل
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(contractFile.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(new { message = "فرمت فایل نامعتبر است. فقط فایل‌های JPG, JPEG و PNG قابل قبول هستند" });
            }

            try
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = $"{id}{extension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    contractFile.CopyTo(stream);
                }

                var record = _context.Records.FirstOrDefault(r => r.Id == id);
                if (record != null)
                {
                    record.contract_image = $"/uploads/{fileName}";
                    _context.SaveChanges();
                }

                return Ok(new
                {
                    success = true,
                    imageUrl = $"/uploads/{fileName}",
                    message = "تصویر با موفقیت ذخیره شد"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "خطا در پردازش تصویر: " + ex.Message
                });
            }
        }
        [HttpGet]
        [HttpGet]
        public IActionResult GetContractImage(int id)
        {
            var record = _context.Records.FirstOrDefault(r => r.Id == id);
            if (record == null)
            {
                return NotFound(new { message = "رکورد مورد نظر یافت نشد" });
            }

            if (string.IsNullOrEmpty(record.contract_image))
            {
                return Ok(new
                {
                    hasImage = false,
                    message = "تصویر قرارداد موجود نیست"
                });
            }

            return Json(new
            {
                hasImage = true,
                imageUrl = record.contract_image
            });
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

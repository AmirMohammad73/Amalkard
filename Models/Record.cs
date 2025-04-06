using System.ComponentModel.DataAnnotations;

namespace MyProject.Models
{
    public class Record
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نام الزامی است.")]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF\s]+$", ErrorMessage = "فقط حروف قابل قبول است.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "نام خانوادگی الزامی است.")]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF\s]+$", ErrorMessage = "فقط حروف قابل قبول است.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "کد ملی الزامی است.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "کد ملی باید فقط شامل اعداد باشد.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "کد ملی باید ۱۰ رقمی باشد.")]
        public string NationalCode { get; set; }

        [Required(ErrorMessage = "نام پدر الزامی است.")]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF\s]+$", ErrorMessage = "فقط حروف قابل قبول است.")]
        public string FatherName { get; set; }

        public string BirthDate { get; set; } // تاریخ تولد

        [Required(ErrorMessage = "شهر محل تولد الزامی است.")]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF\s]+$", ErrorMessage = "فقط حروف قابل قبول است.")]
        public string BirthCity { get; set; }

        [Required(ErrorMessage = "محل صدور شناسنامه الزامی است.")]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF\s]+$", ErrorMessage = "فقط حروف قابل قبول است.")]
        public string CertificateIssuePlace { get; set; }

        [Required(ErrorMessage = "مدرک تحصیلی الزامی است.")]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF\s]+$", ErrorMessage = "فقط حروف قابل قبول است.")]
        public string EducationDegree { get; set; }

        [Required(ErrorMessage = "شغل الزامی است.")]
        public string Job { get; set; }

        public string EmploymentStartDate { get; set; } // تاریخ شروع کار

        [Required(ErrorMessage = "وضعیت تاهل الزامی است.")]
        public string MaritalStatus { get; set; }

        [Required(ErrorMessage = "تعداد فرزند الزامی است.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "تعداد فرزند باید عددی باشد.")]
        public int ChildrenCount { get; set; }

        [Required(ErrorMessage = "سرپرست خانوار الزامی است.")]
        public bool IsHeadOfHousehold { get; set; }

        public bool HasInsurance { get; set; }
        public int InsuranceHistory { get; set; } // سابقه بیمه
    }
}
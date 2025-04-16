using System.ComponentModel.DataAnnotations;

namespace EmployeePerformanceSystem.Models
{
    public class Record
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نام الزامی است.")]
        [RegularExpression(
            @"^[a-zA-Z\u0600-\u06FF\s]+$",
            ErrorMessage = "فقط حروف قابل قبول است.1"
        )]
        public string firstName { get; set; }

        [Required(ErrorMessage = "نام خانوادگی الزامی است.")]
        [RegularExpression(
            @"^[a-zA-Z\u0600-\u06FF\s]+$",
            ErrorMessage = "فقط حروف قابل قبول است.2"
        )]
        public string lastName { get; set; }

        [Required(ErrorMessage = "کد ملی الزامی است.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "کد ملی باید فقط شامل اعداد باشد.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "کد ملی باید ۱۰ رقمی باشد.")]
        public string national_id { get; set; }

        [Required(ErrorMessage = "نام پدر الزامی است.")]
        [RegularExpression(
            @"^[a-zA-Z\u0600-\u06FF\s]+$",
            ErrorMessage = "فقط حروف قابل قبول است.3"
        )]
        public string father_name { get; set; }

        public string birthdate { get; set; } // تاریخ تولد

        [Required(ErrorMessage = "شهر محل تولد الزامی است.")]
        [RegularExpression(
            @"^[a-zA-Z\u0600-\u06FF\s]+$",
            ErrorMessage = "فقط حروف قابل قبول است.4"
        )]
        public string b_city { get; set; }

        [Required(ErrorMessage = "محل صدور شناسنامه الزامی است.")]
        public string p_city { get; set; }

        [Required(ErrorMessage = "مقطع تحصیلی الزامی است.")]
        public int degree { get; set; }

        [Required(ErrorMessage = "مدرک تحصیلی الزامی است.")]
        public string cert { get; set; }

        [Required(ErrorMessage = "شغل الزامی است.")]
        public int Job { get; set; }

        public string startdate { get; set; } // تاریخ شروع کار

        [Required(ErrorMessage = "وضعیت تاهل الزامی است.")]
        public bool is_married { get; set; }

        [Required(ErrorMessage = "تعداد فرزند الزامی است.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "تعداد فرزند باید عددی باشد.")]
        public int children_no { get; set; }

        [Required(ErrorMessage = "سرپرست خانوار الزامی است.")]
        public bool is_head { get; set; }

        [Required(ErrorMessage = "شماره شبا الزامی است.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "فقط عدد قابل قبول است.")]
        public string Sheba { get; set; }

        [Required(ErrorMessage = "نام بانک الزامی است.")]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF\s]+$", ErrorMessage = "فقط حروف قابل قبول است.")]
        public string bank_name { get; set; }

        public bool has_insurance { get; set; }

        [RegularExpression(
            @"^(\d{10})?$",
            ErrorMessage = "شماره بیمه باید فقط شامل اعداد باشد و دقیقاً 10 رقمی باشد."
        )]
        public string? insurance_number { get; set; }

        public int insurance_days { get; set; }

        // تصویر قرارداد (اختیاری)
        public string? contract_image { get; set; }

        public bool is_deleted { get; set; }
        public int? office_id { get; set; }
        public int? ostan_id { get; set; }
    }
}

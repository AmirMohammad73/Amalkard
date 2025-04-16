using System.ComponentModel.DataAnnotations;

namespace EmployeePerformanceSystem.Models
{
    public class User
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string username { get; set; }

        [Required]
        public string password_hash { get; set; }

        public bool is_blacklisted { get; set; } = false;

        // ستون جدید fullname
        public string fullname { get; set; }

        public int office_permission { get; set; }

        public int ostan_permission { get; set; }
    }
}

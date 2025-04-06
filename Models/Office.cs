using System.ComponentModel.DataAnnotations;

namespace EmployeePerformanceSystem.Models
{
    public class Office
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string name { get; set; }

        public bool is_national { get; set; }
    }
}
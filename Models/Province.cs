using System.ComponentModel.DataAnnotations;

namespace EmployeePerformanceSystem.Models
{
    public class Province
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string name { get; set; }
    }
}
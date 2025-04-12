// Models/MonthlyRecord.cs
using System.ComponentModel.DataAnnotations;

namespace EmployeePerformanceSystem.Models
{
    public class MonthlyRecord
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int month { get; set; }
        public int? work { get; set; }
        public int? vacation { get; set; }
        public int? vacation_sick { get; set; }
        public int? mission { get; set; }
        public int? overtime_system { get; set; }
        public int? overtime_final { get; set; }
        public int sum_work { get; set; }
    }
}
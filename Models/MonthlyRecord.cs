// Models/MonthlyRecord.cs
using System.ComponentModel.DataAnnotations;

namespace EmployeePerformanceSystem.Models
{
    public class MonthlyRecord
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public byte month { get; set; } // tinyint در SQL Server معادل byte در C#
        public byte? work { get; set; }
        public byte? vacation { get; set; }
        public byte? vacation_sick { get; set; }
        public byte? mission { get; set; }
        public int? overtime_system { get; set; }
        public int? overtime_final { get; set; }
        public int sum_work { get; set; }
    }
}

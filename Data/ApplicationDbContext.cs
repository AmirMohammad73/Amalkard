using EmployeePerformanceSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeePerformanceSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> User { get; set; }
        public DbSet<Office> Offices { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<MonthlyRecord> MonthlyRecords { get; set; }
        public DbSet<Record> Records { get; set; }
    }
}

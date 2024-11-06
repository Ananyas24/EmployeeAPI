using Microsoft.EntityFrameworkCore;
using EmployeeCrudApi.Models;

namespace EmployeeCrudApi.Data
{
    public class EmployeeContext : DbContext
    {
        public EmployeeContext(DbContextOptions<EmployeeContext> options) : base(options)
        {
        }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<EmployeeDepartment> EmployeeDepartments { get; set; }

 protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Define composite key for EmployeeDepartment
        modelBuilder.Entity<EmployeeDepartment>()
            .HasKey(ed => new { ed.EmployeeId, ed.DepartmentId });

        // Optionally, configure many-to-many relationships
        modelBuilder.Entity<EmployeeDepartment>()
            .HasOne(ed => ed.Employee)
            .WithMany(e => e.EmployeeDepartments)
            .HasForeignKey(ed => ed.EmployeeId);

        modelBuilder.Entity<EmployeeDepartment>()
            .HasOne(ed => ed.Department)
            .WithMany(d => d.EmployeeDepartments)
            .HasForeignKey(ed => ed.DepartmentId);
    }

        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     modelBuilder.Entity<Employee>()
        //         .ToTable("Employees")
        //         .HasKey(e => e.Id);

        //     modelBuilder.Entity<Department>()
        //         .ToTable("Departments")
        //         .HasKey(d => d.Id);

        //     // Configuring many-to-many relationship between Employee and Department
        //     modelBuilder.Entity<EmployeeDepartment>()
        //         .HasKey(ed => new { ed.EmployeeId, ed.DepartmentId });

        //     modelBuilder.Entity<EmployeeDepartment>()
        //         .HasOne(ed => ed.Employee)
        //         .WithMany(e => e.EmployeeDepartments)
        //         .HasForeignKey(ed => ed.EmployeeId);

        //     modelBuilder.Entity<EmployeeDepartment>()
        //         .HasOne(ed => ed.Department)
        //         .WithMany(d => d.EmployeeDepartments)
        //         .HasForeignKey(ed => ed.DepartmentId);
        // }
    }
}

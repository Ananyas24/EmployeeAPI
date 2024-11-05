using Microsoft.EntityFrameworkCore;
using EmployeeCrudApi.Models;

namespace EmployeeCrudApi.Data
{
    public class EmployeeContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }

        public EmployeeContext(DbContextOptions<EmployeeContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Fluent API configurations
            modelBuilder.Entity<Employee>()
                .ToTable("Employees")
                .HasKey(e => e.Id);

            modelBuilder.Entity<Department>()
                .ToTable("Departments")
                .HasKey(d => d.Id);

           modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department) // Specify the navigation property
                .WithMany(d => d.Employees) // Specify the collection of Employees in Department
                .HasForeignKey(e => e.DepartmentId); // Define the foreign key


        }
    }
}

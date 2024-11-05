namespace EmployeeCrudApi.Models
{
    public class Department
    {
        public int Id { get; set; }
        public required string DepartmentName { get; set; }

        // Navigation property for related Employees
        public ICollection<Employee> Employees { get; set; } = new List<Employee>(); // Initialize the collection
    }
}

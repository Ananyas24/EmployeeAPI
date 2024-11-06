namespace EmployeeCrudApi.Models
{
   public class Employee
{
    public int EmployeeId { get; set; }
    public required string Name { get; set; }
    public required string Position { get; set; }

    public ICollection<EmployeeDepartment> EmployeeDepartments { get; set; } = new List<EmployeeDepartment>();
}

}

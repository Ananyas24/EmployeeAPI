namespace EmployeeCrudApi.Models
{
   public class Department
{
    public int DepartmentId { get; set; }
    public required string DepartmentName { get; set; }

    public ICollection<EmployeeDepartment> EmployeeDepartments { get; set; } = new List<EmployeeDepartment>();
}

}

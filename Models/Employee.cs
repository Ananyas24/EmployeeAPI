namespace EmployeeCrudApi.Models
{
    public class Employee
    {
        public int Id { get; set; }
        
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        
        public int DepartmentId { get; set; } // Foreign key for Department
        

        
         public Department? Department { get; set; }
    }
}

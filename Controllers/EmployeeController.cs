using EmployeeCrudApi.Data;
using EmployeeCrudApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EmployeeCrudApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeContext _context;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(EmployeeContext context, ILogger<EmployeeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/employee
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            _logger.LogInformation("Fetching all employees with departments.");
            var employees = await _context.Employees.ToListAsync();
                                        //   .Include(e => e.Department) // Include Department data
                                        //   .ToListAsync();
            _logger.LogInformation("Fetched {Count} employees.", employees.Count);
            return Ok(employees);
        }

        // GET: api/employee/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            _logger.LogInformation("Fetching employee with ID {Id} including department.", id);
            var employee = await _context.Employees
                                         .Include(e => e.Department) // Include Department data
                                         .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                _logger.LogWarning("Employee with ID {Id} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Fetched employee: {Employee}.", employee);
            return Ok(employee);
        }

        // POST: api/employee
      [HttpPost]
public async Task<ActionResult<Employee>> PostEmployee([FromBody] Employee employee)
{
    if (!ModelState.IsValid)
    {
        _logger.LogWarning("Invalid model state for employee creation.");
        return BadRequest(ModelState);
    }

    // Check if the department exists
    var department = await _context.Departments.FindAsync(employee.DepartmentId);
    if (department == null)
    {
        _logger.LogWarning("Department with ID {DepartmentId} not found.", employee.DepartmentId);
        return NotFound(new { Message = $"Department with ID {employee.DepartmentId} not found." });
    }

    // Associate the existing department with the employee
    employee.Department = department;

    _logger.LogInformation("Creating a new employee: {Employee}.", employee);
    _context.Employees.Add(employee);
    await _context.SaveChangesAsync();

    _logger.LogInformation("Created employee with ID {Id}.", employee.Id);
    return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
}

        // PUT: api/employee/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, [FromBody] Employee employee)
        {
            if (id != employee.Id)
            {
                _logger.LogError("Employee ID mismatch. Provided ID: {ProvidedId}, Employee ID: {EmployeeId}", id, employee.Id);
                return BadRequest("Employee ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for employee update.");
                return BadRequest(ModelState);
            }

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated employee with ID {Id}.", id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    _logger.LogWarning("Employee with ID {Id} not found for update.", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogError("Concurrency exception occurred while updating employee with ID {Id}.", id);
                    throw; // Consider throwing a custom exception or returning a specific error
                }
            }

            return NoContent();
        }

        // DELETE: api/employee/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            _logger.LogInformation("Deleting employee with ID {Id}.", id);
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                _logger.LogWarning("Employee with ID {Id} not found for deletion.", id);
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted employee with ID {Id}.", id);
            return NoContent();
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}

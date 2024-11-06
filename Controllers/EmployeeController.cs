using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;  // Ensure this is included for Serilog logging
using EmployeeCrudApi.Models;  // Ensure this is included for Employee and other models
using EmployeeCrudApi.Data;    // Ensure this is included for AppDbContext
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeCrudApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeContext _context;
        private readonly Serilog.ILogger _logger;  

        public EmployeeController(EmployeeContext context)
        {
            _context = context;
            _logger = Log.ForContext<EmployeeController>();  
        }

        // 1. Get all employee details with their department name
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            _logger.Information("Fetching all employees with their department details.");
            try
            {
                var employees = await _context.Employees
                    .Include(e => e.EmployeeDepartments).ToListAsync();
                    // .ThenInclude(ed => ed.Department)
                    // .ToListAsync();

                _logger.Information("Successfully fetched {EmployeeCount} employees.", employees.Count);
                return Ok(employees);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while fetching employees.");
                return StatusCode(500, "Internal server error");
            }
        }

        // 2. Create a new employee with department name
        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] Employee employee)
        {
            _logger.Information("Creating new employee.");

            try
            {
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
                _logger.Information("Successfully created employee with ID {EmployeeId}.", employee.EmployeeId);
                return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.EmployeeId }, employee);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while creating employee.");
                return StatusCode(500, "Internal server error");
            }
        }

        // 3. Get all employees with department id
        [HttpGet("by-department/{departmentId}")]
        public async Task<IActionResult> GetEmployeesByDepartment(int departmentId)
        {
            _logger.Information("Fetching employees for department with ID {DepartmentId}.", departmentId);
            try
            {
                var employees = await _context.EmployeeDepartments
                    .Where(ed => ed.DepartmentId == departmentId)
                    .Include(ed => ed.Employee)
                    .Select(ed => ed.Employee)
                    .ToListAsync();

                return Ok(employees);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while fetching employees by department.");
                return StatusCode(500, "Internal server error");
            }
        }

        // 4. Get employee details with employee id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            _logger.Information("Fetching employee details for ID {EmployeeId}.", id);
            try
            {
                var employee = await _context.Employees
                    .Include(e => e.EmployeeDepartments)
                    .ThenInclude(ed => ed.Department)
                    .FirstOrDefaultAsync(e => e.EmployeeId == id);

                if (employee == null)
                {
                    _logger.Warning("Employee with ID {EmployeeId} not found.", id);
                    return NotFound();
                }

                return Ok(employee);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while fetching employee details.");
                return StatusCode(500, "Internal server error");
            }
        }

        // 5. Update employee details with department name
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] Employee employee)
        {
            _logger.Information("Updating employee details for ID {EmployeeId}.", id);

            if (id != employee.EmployeeId) return BadRequest();

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.Information("Successfully updated employee with ID {EmployeeId}.", id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    _logger.Warning("Employee with ID {EmployeeId} not found.", id);
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while updating employee.");
                return StatusCode(500, "Internal server error");
            }

            return NoContent();
        }

        // 6. Delete employee
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            _logger.Information("Deleting employee with ID {EmployeeId}.", id);

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                _logger.Warning("Employee with ID {EmployeeId} not found.", id);
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            _logger.Information("Successfully deleted employee with ID {EmployeeId}.", id);

            return NoContent();
        }

        // Helper method to check if employee exists
        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}

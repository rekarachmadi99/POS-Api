using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POS_Api.Models;
using POS_Api.Services;

namespace POS_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeService _employeeService;
        public EmployeeController(EmployeeService employeeService) {
            _employeeService = employeeService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(string id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployee()
        {
            var employees = await _employeeService.GetEmployeesAsync();
            return Ok(employees);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] Employee employeeData)
        {
            if (employeeData == null)
            {
                return BadRequest();
            }

            var createdEmployee = await _employeeService.CreateEmployeeAsync(employeeData);
            return CreatedAtAction(nameof(GetEmployeeById), new { id = createdEmployee.EmployeeId }, createdEmployee);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(string id, [FromBody] Employee employeeData)
        {
            if (employeeData == null || !id.Equals(employeeData.EmployeeId))
            {
                return BadRequest();
            }

            var updatedEmployee = await _employeeService.UpdateEmployeeAsync(id, employeeData);
            if (updatedEmployee == null)
            {
                return NotFound();
            }

            return Ok(updatedEmployee);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(string id)
        {
            var success = await _employeeService.DeleteEmployeeAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POS_Api.Helpers;
using POS_Api.Models;
using POS_Api.Services;
using System.Threading.Tasks;

namespace POS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeeAccountController : ControllerBase
    {
        private readonly EmployeeAccountService _employeeAccountService;

        public EmployeeAccountController(EmployeeAccountService employeeAccountService)
        {
            _employeeAccountService = employeeAccountService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeAccountByIdAsync(string id)
        {
            var employeeAccount = await _employeeAccountService.GetEmployeeAccountByIdAsync(id);
            if (employeeAccount == null)
            {
                return NotFound();
            }
            return Ok(employeeAccount);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployeeAccountAsync([FromBody] EmployeeAccount employeeAccount)
        {
            if (employeeAccount == null)
            {
                return BadRequest("Employee account cannot be null.");
            }

            try
            {
                employeeAccount.Password = PasswordHelper.HashPassword(employeeAccount.Password);
                var createdAccount = await _employeeAccountService.CreateEmployeeAccountAsync(employeeAccount);
                return CreatedAtAction(nameof(GetEmployeeAccountByIdAsync), new { id = createdAccount.EmployeeAccountId }, createdAccount);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateEmployeeAccountAsync([FromBody] EmployeeAccount updatedEmployeeAccount)
        {
            if (updatedEmployeeAccount == null)
            {
                return BadRequest("Employee account cannot be null.");
            }
            updatedEmployeeAccount.Password = PasswordHelper.HashPassword(updatedEmployeeAccount.Password);
            var isSave = await _employeeAccountService.UpdateEmployeeAccountAsync(updatedEmployeeAccount);
            if (!isSave)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeAccountAsync(string id)
        {
            var result = await _employeeAccountService.DeleteEmployeeAccountAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}

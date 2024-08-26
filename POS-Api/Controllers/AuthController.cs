using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using POS_Api.Models;
using POS_Api.Services;
using System;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using POS_Api.Helpers;
using System.Text.Json.Serialization;

namespace POS_Api.Controllers
{
    public class LoginData
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly EmployeeAccountService _employeeAccountService;
        private readonly JwtService _jwtService;

        public AuthController(EmployeeAccountService employeeAccountService, JwtService jwtService)
        {
            _employeeAccountService = employeeAccountService;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginData loginData)
        {
            if (loginData == null)
            {
                return BadRequest("Invalid client request");
            }

            var employeeAccountData = await _employeeAccountService.GetEmployeeAccountByUsernameAsync(loginData.username);

            if (employeeAccountData == null)
            {
                return NotFound();
            }

            var isPasswordVerify = PasswordHelper.VerifyPassword(loginData.password, employeeAccountData.Password);

            if (!isPasswordVerify)
            {
                return Unauthorized();
            }

            var token = _jwtService.GenerateToken(employeeAccountData);

            return Ok(new { token = token });
        }

    }
}
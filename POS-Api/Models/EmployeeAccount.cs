using Microsoft.AspNetCore.Http.HttpResults;
using System.Runtime.InteropServices;

namespace POS_Api.Models
{
    public class EmployeeAccount
    {
        public string EmployeeAccountId { get; set; }
        public string EmployeeId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string RefreshToken { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

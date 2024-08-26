using Microsoft.EntityFrameworkCore;
using POS_Api.Data;
using POS_Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Services
{
    public class EmployeeService
    {
        private readonly ApplicationDBContext _context;

        public EmployeeService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Employee?> GetEmployeeByIdAsync(string employeeId)
        {
            return await _context.Employee
                .Include(e => e.EmployeeAccount)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync()
        {
            return await _context.Employee
                .Include(e => e.EmployeeAccount)
                .ToListAsync();
        }

        public async Task<Employee> CreateEmployeeAsync(Employee employeeData)
        {
            if (employeeData == null)
            {
                throw new ArgumentNullException(nameof(employeeData));
            }

            if (!string.IsNullOrEmpty(employeeData.EmployeeId))
            {
                var employeeAccountExists = await _context.EmployeeAccount.AnyAsync(ea => ea.EmployeeId == employeeData.EmployeeId);
                if (!employeeAccountExists)
                {
                    throw new ArgumentException("Invalid Employee Account ID.");
                }
            }

            _context.Employee.Add(employeeData);
            try
            {
                await _context.SaveChangesAsync();
                return employeeData;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while creating the employee.", ex);
            }
        }

        public async Task<bool> UpdateEmployeeAsync(string employeeId, Employee employeeData)
        {
            if (employeeId != employeeData.EmployeeId)
            {
                throw new ArgumentException("Employee ID does not match.");
            }

            var existingEmployee = await _context.Employee
                .Include(e => e.EmployeeAccount)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

            if (existingEmployee == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(employeeData.EmployeeId))
            {
                var employeeAccountExists = await _context.EmployeeAccount.AnyAsync(ea => ea.EmployeeId == employeeData.EmployeeId);
                if (!employeeAccountExists)
                {
                    throw new ArgumentException("Invalid Employee Account ID.");
                }
            }

            existingEmployee.FirstName = employeeData.FirstName;
            existingEmployee.LastName = employeeData.LastName;
            existingEmployee.Email = employeeData.Email;
            existingEmployee.IsVerifEmail = employeeData.IsVerifEmail;
            existingEmployee.PhoneNumber = employeeData.PhoneNumber;
            existingEmployee.Address = employeeData.Address;
            existingEmployee.Role = employeeData.Role;
            existingEmployee.Images = employeeData.Images;
            existingEmployee.UpdatedBy = employeeData.UpdatedBy;
            existingEmployee.UpdatedAt = employeeData.UpdatedAt;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while updating the employee.", ex);
            }
        }

        public async Task<bool> DeleteEmployeeAsync(string employeeId)
        {
            var employee = await _context.Employee
                .Include(e => e.EmployeeAccount)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

            if (employee == null)
            {
                return false;
            }

            _context.Employee.Remove(employee);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while deleting the employee.", ex);
            }
        }
    }
}

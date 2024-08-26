using Microsoft.EntityFrameworkCore;
using POS_Api.Data;
using POS_Api.Models;

namespace POS_Api.Services
{
    public class EmployeeAccountService
    {
        private readonly ApplicationDBContext _context;

        public EmployeeAccountService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<EmployeeAccount?> GetEmployeeAccountByIdAsync(string employeeAccountId)
        {
            return await _context.EmployeeAccount.FindAsync(employeeAccountId);
        }

        public async Task<EmployeeAccount?> GetEmployeeAccountByUsernameAsync(string username)
        {
            var employeeAccountList = await _context.EmployeeAccount.ToListAsync();
            return employeeAccountList.Where(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

        }

        public async Task<EmployeeAccount?> CreateEmployeeAccountAsync(EmployeeAccount employeeAccount)
        {
            _context.EmployeeAccount.Add(employeeAccount);
            try
            {
                await _context.SaveChangesAsync();
                return employeeAccount;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Unable to create the employee account.", ex);
            }
        }

        public async Task<bool> UpdateEmployeeAccountAsync(EmployeeAccount updatedEmployeeAccount)
        {
            var existingAccount = await _context.EmployeeAccount.FindAsync(updatedEmployeeAccount.EmployeeAccountId);
            if (existingAccount == null)
            {
                return false;
            }

            try
            {
                _context.Entry(existingAccount).CurrentValues.SetValues(updatedEmployeeAccount);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Unable to update the employee account.", ex);
            }
        }

        public async Task<bool> DeleteEmployeeAccountAsync(string employeeAccountId)
        {
            var employeeAccount = await _context.EmployeeAccount.FindAsync(employeeAccountId);
            if (employeeAccount == null)
            {
                return false;
            }

            try
            {
                _context.EmployeeAccount.Remove(employeeAccount);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Unable to delete the employee account.", ex);
            }
        }
    }
}

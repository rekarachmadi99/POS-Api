using Microsoft.EntityFrameworkCore;
using POS_Api.Data;
using POS_Api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace POS_Api.Services
{
    public class CustomerService
    {
        private readonly ApplicationDBContext _context;

        public CustomerService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Customer?> GetCustomerByIdAsync(string customerId)
        {
            return await _context.Customer.FindAsync(customerId);
        }

        public async Task<IEnumerable<Customer>> GetCustomersAsync()
        {
            return await _context.Customer.ToListAsync();
        }

        public async Task<Customer> CreateCustomerAsync(Customer customerData)
        {
            if (customerData == null)
            {
                throw new ArgumentNullException(nameof(customerData));
            }

            _context.Customer.Add(customerData);
            try
            {
                await _context.SaveChangesAsync();
                return customerData;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while creating the customer.", ex);
            }
        }

        public async Task<bool> UpdateCustomerAsync(string customerId, Customer customerData)
        {
            if (customerId != customerData.CustomerId)
            {
                throw new ArgumentException("Customer ID does not match.");
            }

            var existingCustomer = await _context.Customer.FindAsync(customerId);
            if (existingCustomer == null)
            {
                return false;
            }

            existingCustomer.FirstName = customerData.FirstName;
            existingCustomer.LastName = customerData.LastName;
            existingCustomer.Email = customerData.Email;
            existingCustomer.Phone = customerData.Phone;
            existingCustomer.Address = customerData.Address;
            existingCustomer.UpdatedBy = customerData.UpdatedBy;
            existingCustomer.UpdatedAt = customerData.UpdatedAt;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while updating the customer.", ex);
            }
        }

        public async Task<bool> DeleteCustomerAsync(string customerId)
        {
            var customer = await _context.Customer.FindAsync(customerId);
            if (customer == null)
            {
                return false;
            }

            _context.Customer.Remove(customer);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while deleting the customer.", ex);
            }
        }
    }
}

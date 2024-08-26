using Microsoft.EntityFrameworkCore;
using POS_Api.Data;
using POS_Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Services
{
    public class TransactionService
    {
        private readonly ApplicationDBContext _context;

        public TransactionService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Transaction?> GetTransactionByIdAsync(string transactionId)
        {
            return await _context.Transaction
                .Include(t => t.Employee)
                .Include(t => t.Customer)
                .Include(t => t.TransactionDetails)
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsAsync()
        {
            return await _context.Transaction
                .Include(t => t.Employee)
                .Include(t => t.Customer)
                .Include(t => t.TransactionDetails)
                .ToListAsync();
        }

        public async Task<Transaction> CreateTransactionAsync(Transaction transactionData)
        {
            if (transactionData == null)
            {
                throw new ArgumentNullException(nameof(transactionData));
            }

            var employeeExists = await _context.Employee.AnyAsync(e => e.EmployeeId == transactionData.UserId);
            if (!employeeExists)
            {
                throw new ArgumentException("Invalid User ID.");
            }

            var customerExists = await _context.Customer.AnyAsync(c => c.CustomerId == transactionData.CustomerId);
            if (!customerExists)
            {
                throw new ArgumentException("Invalid Customer ID.");
            }

            _context.Transaction.Add(transactionData);

            try
            {
                await _context.SaveChangesAsync();
                return transactionData;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while creating the transaction.", ex);
            }
        }

        public async Task<bool> UpdateTransactionAsync(string transactionId, Transaction transactionData)
        {
            if (transactionId != transactionData.TransactionId)
            {
                throw new ArgumentException("Transaction ID does not match.");
            }

            var existingTransaction = await _context.Transaction
                .Include(t => t.TransactionDetails)
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);

            if (existingTransaction == null)
            {
                return false;
            }

            var employeeExists = await _context.Employee.AnyAsync(e => e.EmployeeId == transactionData.UserId);
            if (!employeeExists)
            {
                throw new ArgumentException("Invalid User ID.");
            }

            var customerExists = await _context.Customer.AnyAsync(c => c.CustomerId == transactionData.CustomerId);
            if (!customerExists)
            {
                throw new ArgumentException("Invalid Customer ID.");
            }

            existingTransaction.UserId = transactionData.UserId;
            existingTransaction.CustomerId = transactionData.CustomerId;
            existingTransaction.TransactionDate = transactionData.TransactionDate;
            existingTransaction.TotalAmount = transactionData.TotalAmount;
            existingTransaction.PaymentMethod = transactionData.PaymentMethod;
            existingTransaction.UpdatedBy = transactionData.UpdatedBy;
            existingTransaction.UpdatedAt = transactionData.UpdatedAt;

            _context.Entry(existingTransaction).Collection(t => t.TransactionDetails).Load();
            foreach (var detail in existingTransaction.TransactionDetails.ToList())
            {
                var updatedDetail = transactionData.TransactionDetails.FirstOrDefault(d => d.TransactionDetailId == detail.TransactionDetailId);
                if (updatedDetail != null)
                {
                    _context.Entry(detail).CurrentValues.SetValues(updatedDetail);
                }
                else
                {
                    _context.TransactionDetail.Remove(detail);
                }
            }
            foreach (var newDetail in transactionData.TransactionDetails.Where(d => string.IsNullOrEmpty(d.TransactionDetailId)))
            {
                existingTransaction.TransactionDetails.Add(newDetail);
            }

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while updating the transaction.", ex);
            }
        }

        public async Task<bool> DeleteTransactionAsync(string transactionId)
        {
            var transaction = await _context.Transaction
                .Include(t => t.TransactionDetails)
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);

            if (transaction == null)
            {
                return false;
            }

            _context.Transaction.Remove(transaction);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while deleting the transaction.", ex);
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using POS_Api.Data;
using POS_Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Services
{
    public class TransactionDetailService
    {
        private readonly ApplicationDBContext _context;

        public TransactionDetailService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<TransactionDetail?> GetTransactionDetailByIdAsync(string transactionDetailId)
        {
            return await _context.TransactionDetail
                .Include(td => td.Product)
                .FirstOrDefaultAsync(td => td.TransactionDetailId == transactionDetailId);
        }

        public async Task<IEnumerable<TransactionDetail>> GetTransactionDetailsAsync()
        {
            return await _context.TransactionDetail
                .Include(td => td.Product)
                .ToListAsync();
        }

        public async Task<TransactionDetail> CreateTransactionDetailAsync(TransactionDetail transactionDetailData)
        {
            if (transactionDetailData == null)
            {
                throw new ArgumentNullException(nameof(transactionDetailData));
            }

            var productExists = await _context.Product.AnyAsync(p => p.ProductId == transactionDetailData.ProductId);
            if (!productExists)
            {
                throw new ArgumentException("Invalid Product ID.");
            }

            _context.TransactionDetail.Add(transactionDetailData);
            try
            {
                await _context.SaveChangesAsync();
                return transactionDetailData;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while creating the transaction detail.", ex);
            }
        }

        public async Task<bool> UpdateTransactionDetailAsync(string transactionDetailId, TransactionDetail transactionDetailData)
        {
            if (transactionDetailId != transactionDetailData.TransactionDetailId)
            {
                throw new ArgumentException("TransactionDetail ID does not match.");
            }

            var existingDetail = await _context.TransactionDetail
                .Include(td => td.Product)
                .FirstOrDefaultAsync(td => td.TransactionDetailId == transactionDetailId);

            if (existingDetail == null)
            {
                return false;
            }

            var productExists = await _context.Product.AnyAsync(p => p.ProductId == transactionDetailData.ProductId);
            if (!productExists)
            {
                throw new ArgumentException("Invalid Product ID.");
            }

            existingDetail.ProductId = transactionDetailData.ProductId;
            existingDetail.Quantity = transactionDetailData.Quantity;
            existingDetail.Price = transactionDetailData.Price;
            existingDetail.Total = transactionDetailData.Total;
            existingDetail.UpdatedBy = transactionDetailData.UpdatedBy;
            existingDetail.UpdatedAt = transactionDetailData.UpdatedAt;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while updating the transaction detail.", ex);
            }
        }

        public async Task<bool> DeleteTransactionDetailAsync(string transactionDetailId)
        {
            var transactionDetail = await _context.TransactionDetail
                .FirstOrDefaultAsync(td => td.TransactionDetailId == transactionDetailId);

            if (transactionDetail == null)
            {
                return false;
            }

            _context.TransactionDetail.Remove(transactionDetail);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while deleting the transaction detail.", ex);
            }
        }
    }
}

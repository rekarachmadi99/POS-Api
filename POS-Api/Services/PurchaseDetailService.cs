using Microsoft.EntityFrameworkCore;
using POS_Api.Data;
using POS_Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Services
{
    public class PurchaseDetailService
    {
        private readonly ApplicationDBContext _context;

        public PurchaseDetailService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<PurchaseDetail?> GetPurchaseDetailByIdAsync(string purchaseDetailId)
        {
            return await _context.PurchaseDetail
                .Include(pd => pd.Product)
                .FirstOrDefaultAsync(pd => pd.PurchaseDetailId == purchaseDetailId);
        }

        public async Task<IEnumerable<PurchaseDetail>> GetPurchaseDetailsAsync()
        {
            return await _context.PurchaseDetail
                .Include(pd => pd.Product)
                .ToListAsync();
        }

        public async Task<PurchaseDetail> CreatePurchaseDetailAsync(PurchaseDetail purchaseDetailData)
        {
            if (purchaseDetailData == null)
            {
                throw new ArgumentNullException(nameof(purchaseDetailData));
            }

            var productExists = await _context.Product.AnyAsync(p => p.ProductId == purchaseDetailData.ProductId);
            if (!productExists)
            {
                throw new ArgumentException("Invalid Product ID.");
            }

            _context.PurchaseDetail.Add(purchaseDetailData);
            try
            {
                await _context.SaveChangesAsync();
                return purchaseDetailData;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while creating the purchase detail.", ex);
            }
        }

        public async Task<bool> UpdatePurchaseDetailAsync(string purchaseDetailId, PurchaseDetail purchaseDetailData)
        {
            if (purchaseDetailId != purchaseDetailData.PurchaseDetailId)
            {
                throw new ArgumentException("PurchaseDetail ID does not match.");
            }

            var existingPurchaseDetail = await _context.PurchaseDetail
                .Include(pd => pd.Product)
                .FirstOrDefaultAsync(pd => pd.PurchaseDetailId == purchaseDetailId);
            if (existingPurchaseDetail == null)
            {
                return false;
            }

            var productExists = await _context.Product.AnyAsync(p => p.ProductId == purchaseDetailData.ProductId);
            if (!productExists)
            {
                throw new ArgumentException("Invalid Product ID.");
            }

            existingPurchaseDetail.ProductId = purchaseDetailData.ProductId;
            existingPurchaseDetail.Quantity = purchaseDetailData.Quantity;
            existingPurchaseDetail.Price = purchaseDetailData.Price;
            existingPurchaseDetail.Total = purchaseDetailData.Total;
            existingPurchaseDetail.UpdatedBy = purchaseDetailData.UpdatedBy;
            existingPurchaseDetail.UpdatedAt = purchaseDetailData.UpdatedAt;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while updating the purchase detail.", ex);
            }
        }

        public async Task<bool> DeletePurchaseDetailAsync(string purchaseDetailId)
        {
            var purchaseDetail = await _context.PurchaseDetail
                .FirstOrDefaultAsync(pd => pd.PurchaseDetailId == purchaseDetailId);
            if (purchaseDetail == null)
            {
                return false;
            }

            _context.PurchaseDetail.Remove(purchaseDetail);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while deleting the purchase detail.", ex);
            }
        }
    }
}

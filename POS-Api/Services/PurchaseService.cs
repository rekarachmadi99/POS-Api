using Microsoft.EntityFrameworkCore;
using POS_Api.Data;
using POS_Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Services
{
    public class PurchaseService
    {
        private readonly ApplicationDBContext _context;

        public PurchaseService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Purchase?> GetPurchaseByIdAsync(string purchaseId)
        {
            return await _context.Purchase
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseDetails)
                .FirstOrDefaultAsync(p => p.PurchaseId == purchaseId);
        }

        public async Task<IEnumerable<Purchase>> GetPurchasesAsync()
        {
            return await _context.Purchase
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseDetails)
                .ToListAsync();
        }

        public async Task<Purchase> CreatePurchaseAsync(Purchase purchaseData)
        {
            if (purchaseData == null)
            {
                throw new ArgumentNullException(nameof(purchaseData));
            }

            var supplierExists = await _context.Supplier.AnyAsync(s => s.SupplierId == purchaseData.SupplierId);
            if (!supplierExists)
            {
                throw new ArgumentException("Invalid Supplier ID.");
            }

            _context.Purchase.Add(purchaseData);
            try
            {
                await _context.SaveChangesAsync();
                return purchaseData;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while creating the purchase.", ex);
            }
        }

        public async Task<bool> UpdatePurchaseAsync(string purchaseId, Purchase purchaseData)
        {
            if (purchaseId != purchaseData.PurchaseId)
            {
                throw new ArgumentException("Purchase ID does not match.");
            }

            var existingPurchase = await _context.Purchase
                .Include(p => p.PurchaseDetails)
                .FirstOrDefaultAsync(p => p.PurchaseId == purchaseId);
            if (existingPurchase == null)
            {
                return false;
            }

            var supplierExists = await _context.Supplier.AnyAsync(s => s.SupplierId == purchaseData.SupplierId);
            if (!supplierExists)
            {
                throw new ArgumentException("Invalid Supplier ID.");
            }

            existingPurchase.SupplierId = purchaseData.SupplierId;
            existingPurchase.PurchaseDate = purchaseData.PurchaseDate;
            existingPurchase.TotalAmount = purchaseData.TotalAmount;
            existingPurchase.UpdatedBy = purchaseData.UpdatedBy;
            existingPurchase.UpdatedAt = purchaseData.UpdatedAt;

            _context.Entry(existingPurchase).Collection(p => p.PurchaseDetails).Load();
            foreach (var detail in existingPurchase.PurchaseDetails.ToList())
            {
                var updatedDetail = purchaseData.PurchaseDetails.FirstOrDefault(d => d.PurchaseDetailId == detail.PurchaseDetailId);
                if (updatedDetail != null)
                {
                    _context.Entry(detail).CurrentValues.SetValues(updatedDetail);
                }
                else
                {
                    _context.PurchaseDetail.Remove(detail);
                }
            }
            foreach (var newDetail in purchaseData.PurchaseDetails.Where(d => string.IsNullOrEmpty(d.PurchaseDetailId)))
            {
                existingPurchase.PurchaseDetails.Add(newDetail);
            }

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while updating the purchase.", ex);
            }
        }

        public async Task<bool> DeletePurchaseAsync(string purchaseId)
        {
            var purchase = await _context.Purchase
                .Include(p => p.PurchaseDetails)
                .FirstOrDefaultAsync(p => p.PurchaseId == purchaseId);
            if (purchase == null)
            {
                return false;
            }

            _context.Purchase.Remove(purchase);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while deleting the purchase.", ex);
            }
        }
    }
}

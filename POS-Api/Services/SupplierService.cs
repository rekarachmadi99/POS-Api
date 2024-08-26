using Microsoft.EntityFrameworkCore;
using POS_Api.Data;
using POS_Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Services
{
    public class SupplierService
    {
        private readonly ApplicationDBContext _context;

        public SupplierService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Supplier?> GetSupplierByIdAsync(string supplierId)
        {
            return await _context.Supplier
                .FirstOrDefaultAsync(s => s.SupplierId == supplierId);
        }

        public async Task<IEnumerable<Supplier>> GetSuppliersAsync()
        {
            return await _context.Supplier.ToListAsync();
        }

        public async Task<Supplier> CreateSupplierAsync(Supplier supplierData)
        {
            if (supplierData == null)
            {
                throw new ArgumentNullException(nameof(supplierData));
            }

            _context.Supplier.Add(supplierData);
            try
            {
                await _context.SaveChangesAsync();
                return supplierData;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while creating the supplier.", ex);
            }
        }

        public async Task<bool> UpdateSupplierAsync(string supplierId, Supplier supplierData)
        {
            if (supplierId != supplierData.SupplierId)
            {
                throw new ArgumentException("Supplier ID does not match.");
            }

            var existingSupplier = await _context.Supplier.FindAsync(supplierId);
            if (existingSupplier == null)
            {
                return false;
            }

            existingSupplier.Name = supplierData.Name;
            existingSupplier.ContactName = supplierData.ContactName;
            existingSupplier.ContactPhone = supplierData.ContactPhone;
            existingSupplier.ContactEmail = supplierData.ContactEmail;
            existingSupplier.Address = supplierData.Address;
            existingSupplier.UpdatedBy = supplierData.UpdatedBy;
            existingSupplier.UpdatedAt = supplierData.UpdatedAt;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while updating the supplier.", ex);
            }
        }

        public async Task<bool> DeleteSupplierAsync(string supplierId)
        {
            var supplier = await _context.Supplier.FindAsync(supplierId);
            if (supplier == null)
            {
                return false;
            }

            _context.Supplier.Remove(supplier);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while deleting the supplier.", ex);
            }
        }
    }
}

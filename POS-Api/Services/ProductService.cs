using Microsoft.EntityFrameworkCore;
using POS_Api.Data;
using POS_Api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace POS_Api.Services
{
    public class ProductService
    {
        private readonly ApplicationDBContext _context;

        public ProductService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetProductByIdAsync(string productId)
        {
            return await _context.Product
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == productId);
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _context.Product
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<Product> CreateProductAsync(Product productData)
        {
            if (productData == null)
            {
                throw new ArgumentNullException(nameof(productData));
            }

            var categoryExists = await _context.Category.AnyAsync(c => c.CategoryId == productData.CategoryId);
            if (!categoryExists)
            {
                throw new ArgumentException("Invalid Category ID.");
            }

            _context.Product.Add(productData);
            try
            {
                await _context.SaveChangesAsync();
                return productData;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while creating the product.", ex);
            }
        }

        public async Task<bool> UpdateProductAsync(string productId, Product productData)
        {
            if (productId != productData.ProductId)
            {
                throw new ArgumentException("Product ID does not match.");
            }

            var existingProduct = await _context.Product.FindAsync(productId);
            if (existingProduct == null)
            {
                return false;
            }

            var categoryExists = await _context.Category.AnyAsync(c => c.CategoryId == productData.CategoryId);
            if (!categoryExists)
            {
                throw new ArgumentException("Invalid Category ID.");
            }

            existingProduct.Name = productData.Name;
            existingProduct.Description = productData.Description;
            existingProduct.Price = productData.Price;
            existingProduct.StockQuantity = productData.StockQuantity;
            existingProduct.Images = productData.Images;
            existingProduct.CategoryId = productData.CategoryId;
            existingProduct.UpdatedBy = productData.UpdatedBy;
            existingProduct.UpdatedAt = productData.UpdatedAt;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while updating the product.", ex);
            }
        }

        public async Task<bool> DeleteProductAsync(string productId)
        {
            var product = await _context.Product.FindAsync(productId);
            if (product == null)
            {
                return false;
            }

            _context.Product.Remove(product);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while deleting the product.", ex);
            }
        }
    }
}

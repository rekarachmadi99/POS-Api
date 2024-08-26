using Microsoft.EntityFrameworkCore;
using POS_Api.Data;
using POS_Api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace POS_Api.Services
{
    public class CategoryService
    {
        private readonly ApplicationDBContext _context;

        public CategoryService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Category?> GetCategoryByIdAsync(string categoryId)
        {
            return await _context.Category.FindAsync(categoryId);
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _context.Category.ToListAsync();
        }

        public async Task<Category> CreateCategoryAsync(Category categoryData)
        {
            if (categoryData == null)
            {
                throw new ArgumentNullException(nameof(categoryData));
            }

            _context.Category.Add(categoryData);
            try
            {
                await _context.SaveChangesAsync();
                return categoryData;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while creating the category.", ex);
            }
        }

        public async Task<bool> UpdateCategoryAsync(string categoryId, Category categoryData)
        {
            if (categoryId != categoryData.CategoryId)
            {
                throw new ArgumentException("Category ID does not match.");
            }

            var existingCategory = await _context.Category.FindAsync(categoryId);
            if (existingCategory == null)
            {
                return false;
            }

            existingCategory.Name = categoryData.Name;
            existingCategory.Description = categoryData.Description;
            existingCategory.UpdatedBy = categoryData.UpdatedBy;
            existingCategory.UpdatedAt = categoryData.UpdatedAt;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while updating the category.", ex);
            }
        }


        public async Task<bool> DeleteCategoryAsync(string categoryId)
        {
            var category = await _context.Category.FindAsync(categoryId);
            if (category == null)
            {
                return false;
            }

            _context.Category.Remove(category);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while deleting the category.", ex);
            }
        }
    }
}

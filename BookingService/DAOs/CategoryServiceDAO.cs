using BookingService.Models;
using Microsoft.EntityFrameworkCore;
using RequestService.DAOs;

namespace BookingService.DAOs
{
    public class CategoryServiceDAO : SingletonBase<CategoryServiceDAO>
    {
        public async Task<IEnumerable<CategoryService>> GetAllCategoryService()
        {
            return await _context.CategoryServices.ToListAsync();
        }

        public async Task<CategoryService> GetCategoryServiceById(int id)
        {
            var item = await _context.CategoryServices.FirstOrDefaultAsync(obj => obj.CategoryServiceId == id);
            if (item == null) return null;
            return item;
        }

        public async Task AddCategoryService(CategoryService item)
        {
            _context.CategoryServices.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCategoryService(CategoryService item)
        {
            var existingItem = await GetCategoryServiceById(item.CategoryServiceId);
            if (existingItem == null) return;
            _context.Entry(existingItem).CurrentValues.SetValues(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryService(int id)
        {
            var item = await GetCategoryServiceById(id);
            if (item == null) return;
            _context.CategoryServices.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}

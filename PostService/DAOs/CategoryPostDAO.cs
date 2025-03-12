using Microsoft.EntityFrameworkCore;
using PostService.Models;

namespace PostService.DAOs
{
    public class CategoryPostDAO : SingletonBaseDAO<CategoryPostDAO>
    {
        public async Task<IEnumerable<CategoryPost>> GetAll()
        {
            return await _context.CategoryPosts.ToListAsync();
        }
        public async Task<CategoryPost> FindById(int id)
        {
            var item = await _context.CategoryPosts.FirstOrDefaultAsync(obj => obj.CategoryPostId == id);
            if (item == null) return null;
            return item;
        }
        public async Task<int> Add(CategoryPost item)
        {
            _context.CategoryPosts.Add(item);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> Update(CategoryPost item)
        {
            var existingItem = await FindById(item.CategoryPostId);
            if (existingItem == null) return -1;

            existingItem.CategoryPostName = item.CategoryPostName;
            existingItem.CategoryPostDescription = item.CategoryPostDescription;
            return await _context.SaveChangesAsync();
        }

        public async Task<int> Delete(int id)
        {
            var item = await FindById(id);
            if (item == null) return -1;
            _context.CategoryPosts.Remove(item);
            return await _context.SaveChangesAsync();
        }
    }
}

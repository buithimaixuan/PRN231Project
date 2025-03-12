using Microsoft.EntityFrameworkCore;
using PostService.DAOs;
using PostService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostService.DAOs
{
    public class NewsDAO : SingletonBaseDAO<NewsDAO>
    {

        public async Task<IEnumerable<News>> GetAllNews()
        {
            return await _context.News.Where(n => n.IsDeleted == false).ToListAsync();
        }
        public async Task<IEnumerable<News>> GetAll()
        {
            return await _context.News.ToListAsync();
        }

        public async Task<News> FindById(int id)
        {
            var item = await _context.News.FirstOrDefaultAsync(obj => obj.NewsId == id);
            if (item == null) return null;
            return item;
        }

        public async Task Add(News item)
        {
            _context.News.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task Update(News item)
        {
            var existingItem = await FindById(item.NewsId);
            if (existingItem == null) return;
            _context.Entry(existingItem).CurrentValues.SetValues(item);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var item = await FindById(id);
            if (item == null) return;

            item.IsDeleted = true;
            _context.News.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<(string Month, int Count)>> GetNewsCountByMonth()
        {
            var result = await _context.Posts
                .Where(n => n.IsDeleted != true)
                .GroupBy(n => new { n.CreatedAt.Value.Year, n.CreatedAt.Value.Month })
                .Select(g => new
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Count = g.Count()
                })
                .ToListAsync();

            return result.Select(item => (item.Month, item.Count));
        }


        public async Task<int> GetTotalNewsCountAsync()
        {
            return await _context.News.CountAsync(n => n.IsDeleted == false);
        }
        
        public async Task<IEnumerable<News>> GetAllNewsByCategoryId(int categoryId)
        {
            return await _context.News.Where(n => n.CategoryNewsId == categoryId).ToListAsync();
        }

        public async Task<IEnumerable<News>> SearchNews(int category, string searchString)
        {
            var query = _context.News.AsQueryable();

            if (category > 0)
            {
                query = query.Where(n => n.CategoryNewsId == category);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(n => n.Title.Contains(searchString) || n.Content.Contains(searchString));
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<News>> GetNewsPaged(int pageNumber, int pageSize)
        {
            return await _context.News
                .Where(s => s.IsDeleted == false) // Adjust based on your business logic
                .OrderBy(s => s.NewsId) // Ensure consistent ordering
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalNewsCount()
        {
            return await _context.News.CountAsync(s => s.IsDeleted == false);
        }

    }
}


﻿using Microsoft.EntityFrameworkCore;
using PostService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostService.DAOs
{
    public class CategoryNewsDAO : SingletonBaseDAO<CategoryNewsDAO>
    {
        public async Task<CategoryNews> GetCategoryNewsById(int id)
        {
            var item = await _context.CategoryNews.FirstOrDefaultAsync(obj => obj.CategoryNewsId == id);
            if (item == null) return null;
            return item;
        }

        public async Task<IEnumerable<CategoryNews>> GetAllCategoryNews()
        {
            return await _context.CategoryNews.ToListAsync();
        }

        public async Task<IEnumerable<CategoryNews>> GetCategoriesHaveNews()
        {
            // Truy vấn để lấy danh sách các CategoryNews có bài báo
            return await _context.CategoryNews
                .Where(c => _context.News.Any(n => n.CategoryNewsId == c.CategoryNewsId))
                .ToListAsync();
        }
    }
}

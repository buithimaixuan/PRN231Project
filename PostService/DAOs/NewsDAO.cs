﻿using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<News>> GetAllNewsAvailable()
        {
            return await _context.News.Where(n => n.IsDeleted != true).ToListAsync();
        }
        public async Task<IEnumerable<News>> GetAllNews()
        {
            return await _context.News.ToListAsync();
        }

        public async Task<List<News>> GetLatestNews(int count)
        {
            return await _context.News
                .Where(n => n.IsDeleted != true)
                .OrderByDescending(n => n.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<News> FindById(int id)
        {
            var item = await _context.News.FirstOrDefaultAsync(obj => obj.NewsId == id && obj.IsDeleted == false);
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

        //****************minhuyen************
        public async Task<int> GetTotalNewsCount()
        {
            return await _context.News.CountAsync(s => s.IsDeleted == false);
        }

        public async Task<IEnumerable<News>> GetAllNewsByCategoryId(int categoryId)
        {
            return await _context.News.Where(n => n.CategoryNewsId == categoryId).ToListAsync();
        }



        public async Task<(IEnumerable<News> News, int TotalCount)> FilterAndPaginateNews(int? categoryId, string searchString, int pageNumber, int pageSize)
        {
            var query = _context.News.Where(n => n.IsDeleted == false).AsQueryable();

            if (categoryId.HasValue && categoryId > 0)
            {
                query = query.Where(n => n.CategoryNewsId == categoryId);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(n => n.Title.Contains(searchString) || n.Content.Contains(searchString));
            }

            int totalCount = await query.CountAsync();

            var newsList = await query
                .OrderByDescending(n => n.CreatedAt) // Sắp xếp theo ngày tạo
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (newsList, totalCount);
        }

    }
}


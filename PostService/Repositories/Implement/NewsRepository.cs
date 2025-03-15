using Microsoft.EntityFrameworkCore;
using PostService.DAOs;
using PostService.Models;
using PostService.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostService.Repositories.Implement
{
    public class NewsRepository : INewsRepository
    {
        private readonly NewsDAO _newsDAO;
        private readonly CategoryNewsDAO _categoryNewsDAO;

        public NewsRepository(NewsDAO newsDAO)
        {
            _newsDAO = newsDAO;
        }

        public async Task<IEnumerable<News>> GetAllNewsAvailable()
        {
            return await _newsDAO.GetAllNewsAvailable();
        }

        public async Task<IEnumerable<News>> GetAllNews()
        {
            return await _newsDAO.GetAllNews();
        }

        public async Task<News> GetById(int id) => await _newsDAO.FindById(id);
        
        public async Task Add(News news) => await _newsDAO.Add(news);
        
        public async Task Update(News news) => await _newsDAO.Update(news);

        public async Task Delete(int id) => await _newsDAO.Delete(id);

        public async Task<IEnumerable<News>> GetAllNewsByCategoryId(int categoryId) => await _newsDAO.GetAllNewsByCategoryId(categoryId);

        public async Task<CategoryNews> GetCategoryNewsById(int id) => await _categoryNewsDAO.GetCategoryNewsById(id);

        public async Task<int> GetTotalNewsRepo() => await _newsDAO.GetTotalNewsCountAsync();
        public async Task<IEnumerable<CategoryNews>> GetAllCategoryNews() => await _categoryNewsDAO.GetAllCategoryNews();

        public Task<int> GetTotalNewsCount() => _newsDAO.GetTotalNewsCount();

        public async Task<IEnumerable<(string Month, int Count)>> GetNewsCountByMonth() => await _newsDAO.GetNewsCountByMonth();

        public async Task<(IEnumerable<News> News, int TotalCount)> FilterAndPaginateNews(int? categoryId, string searchString, int pageNumber, int pageSize) 
            => await _newsDAO.FilterAndPaginateNews(categoryId, searchString, pageNumber, pageSize);
    }
}

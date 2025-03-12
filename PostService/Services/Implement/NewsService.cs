using PRN221_BusinessLogic.Interface;
using PRN221_Models.Models;
using PRN221_Repository.NewsRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN221_BusinessLogic.Service
{
    public class NewsService : INewsService
    {
        private readonly INewsRepository _newsRepo;
        private readonly ICategoryNewsRepository _categoryNewsRepository;

        public NewsService(INewsRepository newsRepo, ICategoryNewsRepository categoryNewsRepository)
        {
            _newsRepo = newsRepo;
            _categoryNewsRepository = categoryNewsRepository;
        }

        public async Task<IEnumerable<News>> GetAllNews() => await _newsRepo.GetAllNews();
        public async Task<News> GetByIdNews(int id)
        {
            return await _newsRepo.GetById(id);
        }
        public async Task AddNews(News item)
        {
            await _newsRepo.Add(item);
        }
        public async Task UpdateNews(News item)
        {
            await _newsRepo.Update(item);
        }
        public async Task DeleteNews(int id)
        {
            await _newsRepo.Delete(id);
        }


        public async Task<IEnumerable<CategoryNews>> GetAllCategoryNews() => await _categoryNewsRepository.GetAllCategoryNews();

        public async Task<CategoryNews> GetCategoryNewsById(int id) => await _categoryNewsRepository.GetCategoryNewsById(id);

        public async Task<IEnumerable<News>> GetAllNewsByCategoryId(int categoryId) => await _newsRepo.GetAllNewsByCategoryId(categoryId);

        public Task<IEnumerable<CategoryNews>> GetCategoriesHaveNews() => _categoryNewsRepository.GetCategoriesHaveNews();

        public async Task<int> GetTotalNewsService()
        {
            return await _newsRepo.GetTotalNewsRepo();
        }

        public async Task<IEnumerable<News>> SearchNews(int category, string searchString) => await _newsRepo.SearchNews(category, searchString);

        public Task<IEnumerable<News>> GetNewsPaged(int pageNumber, int pageSize) => _newsRepo.GetNewsPaged(pageNumber, pageSize);  

        public Task<int> GetTotalNewsCount() => _newsRepo.GetTotalNewsCount();

        public async Task<IEnumerable<(string Month, int Count)>> GetNewsCountByMonth() => await _newsRepo.GetNewsCountByMonth();


    }
}

using PostService.DAOs;
using PostService.Models;
using PostService.Repositories.Implement;
using PostService.Repositories.Interface;
using PostService.Services.Interface;

namespace PostService.Services.Implement
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
        public async Task<IEnumerable<News>> GetAllNewsAvailable() => await _newsRepo.GetAllNewsAvailable();

        public async Task<IEnumerable<News>> GetAllNews() => await _newsRepo.GetAllNews();

        public async Task<List<News>> GetLatestNews(int count) => await _newsRepo.GetLatestNews(count);

        public async Task<News> GetByIdNews(int id) => await _newsRepo.GetById(id);
        
        public async Task AddNews(News item) => await _newsRepo.Add(item);

        public async Task UpdateNews(News item) => await _newsRepo.Update(item);
        
        public async Task DeleteNews(int id) => await _newsRepo.Delete(id);

        public async Task<IEnumerable<CategoryNews>> GetAllCategoryNews() => await _categoryNewsRepository.GetAllCategoryNews();

        public async Task<CategoryNews> GetCategoryNewsById(int id) => await _categoryNewsRepository.GetCategoryNewsById(id);

        public async Task<IEnumerable<News>> GetAllNewsByCategoryId(int categoryId) => await _newsRepo.GetAllNewsByCategoryId(categoryId);

        public Task<IEnumerable<CategoryNews>> GetCategoriesHaveNews() => _categoryNewsRepository.GetCategoriesHaveNews();
        
        public async Task<int> GetTotalNewsService() => await _newsRepo.GetTotalNewsRepo();
        
        public Task<int> GetTotalNewsCount() => _newsRepo.GetTotalNewsRepo();
        
        public async Task<IEnumerable<(string Month, int Count)>> GetNewsCountByMonth() => await _newsRepo.GetNewsCountByMonth();
        
        public async Task<(IEnumerable<News> News, int TotalCount)> FilterAndPaginateNews(int? categoryId, string searchString, int pageNumber, int pageSize)
            => await _newsRepo.FilterAndPaginateNews(categoryId, searchString, pageNumber, pageSize);

        public async Task Add(CategoryNews news) => await _categoryNewsRepository.Add(news);

        public async Task Update(CategoryNews news) => await _categoryNewsRepository.Update(news);
    }
}

using PostService.Models;

namespace PostService.Services.Interface
{
    public interface INewsService
    {
        Task<IEnumerable<News>> GetAllNewsAvailable();
        Task<IEnumerable<News>> GetAllNews();
        Task<List<News>> GetLatestNews(int count);
        Task<IEnumerable<CategoryNews>> GetAllCategoryNews();
        Task<News> GetByIdNews(int id);
        Task AddNews(News item);
        Task UpdateNews(News item);
        Task DeleteNews(int id);
        Task<CategoryNews> GetCategoryNewsById(int id);
        Task<IEnumerable<News>> GetAllNewsByCategoryId(int categoryId);
        Task<IEnumerable<CategoryNews>> GetCategoriesHaveNews();
        Task<int> GetTotalNewsService();
        Task<int> GetTotalNewsCount();
        Task<IEnumerable<(string Month, int Count)>> GetNewsCountByMonth();
        Task<(IEnumerable<News> News, int TotalCount)> FilterAndPaginateNews(int? categoryId, string searchString, int pageNumber, int pageSize);
    }
}

using PostService.Models;

namespace PostService.Services.Interface
{
    public interface INewsService
    {
        Task<IEnumerable<News>> GetAllNews();
        Task<IEnumerable<CategoryNews>> GetAllCategoryNews();
        Task<News> GetByIdNews(int id);
        Task AddNews(News item);
        Task UpdateNews(News item);
        Task DeleteNews(int id);
        Task<CategoryNews> GetCategoryNewsById(int id);
        Task<IEnumerable<News>> GetAllNewsByCategoryId(int categoryId);
        Task<IEnumerable<CategoryNews>> GetCategoriesHaveNews();
        Task<int> GetTotalNewsService();
        Task<IEnumerable<News>> SearchNews(int category, string searchString);
        Task<IEnumerable<News>> GetNewsPaged(int pageNumber, int pageSize);
        Task<int> GetTotalNewsCount();

       Task<IEnumerable<(string Month, int Count)>> GetNewsCountByMonth();
    }
}

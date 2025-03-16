using PostService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostService.Repositories.Interface
{
    public interface INewsRepository
    {
        Task<IEnumerable<News>> GetAllNewsAvailable();
        Task<IEnumerable<News>> GetAllNews();
        Task<News> GetById(int id);
        Task Add(News item);
        Task Update(News item);
        Task Delete(int id);
        Task<IEnumerable<News>> GetAllNewsByCategoryId(int categoryId);
        Task<int> GetTotalNewsRepo();
        Task<IEnumerable<(string Month, int Count)>> GetNewsCountByMonth();
        Task<(IEnumerable<News> News, int TotalCount)> FilterAndPaginateNews(int? categoryId, string searchString, int pageNumber, int pageSize);
    }
}

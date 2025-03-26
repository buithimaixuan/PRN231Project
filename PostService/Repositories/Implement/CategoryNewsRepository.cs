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
    public class CategoryNewsRepository : ICategoryNewsRepository
    {
        private readonly CategoryNewsDAO _categoryNewsDAO;

        public CategoryNewsRepository(CategoryNewsDAO categoryNewsDAO)
        {
            _categoryNewsDAO = categoryNewsDAO;
        }

        public async Task<IEnumerable<CategoryNews>> GetAllCategoryNews() => await _categoryNewsDAO.GetAllCategoryNews();

        public async Task<IEnumerable<CategoryNews>> GetCategoriesHaveNews() => await _categoryNewsDAO.GetCategoriesHaveNews();

        public async Task<CategoryNews> GetCategoryNewsById(int id) => await _categoryNewsDAO.GetCategoryNewsById(id);

        public async Task Add(CategoryNews news) => await _categoryNewsDAO.Add(news);

        public async Task Update(CategoryNews news) => await _categoryNewsDAO.Update(news);
    }
}

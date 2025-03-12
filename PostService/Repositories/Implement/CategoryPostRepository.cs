using PostService.DAOs;
using PostService.Models;
using PostService.Repositories.Interface;

namespace PostService.Repositories.Implement
{
    public class CategoryPostRepository : ICategoryPostRepository
    {
        private readonly CategoryPostDAO _categoryPostDAO;

        public CategoryPostRepository(CategoryPostDAO categoryPostDAO)
        {
            _categoryPostDAO = categoryPostDAO;
        }

        public async Task<IEnumerable<CategoryPost>> GetAllCategory()
        {
            return await _categoryPostDAO.GetAll();
        }
        public async Task<CategoryPost> FindById(int id) => await _categoryPostDAO.FindById(id);
        public async Task<int> Add(CategoryPost news) => await _categoryPostDAO.Add(news);
        public async Task<int> Update(CategoryPost news) => await _categoryPostDAO.Update(news);

        public async Task<int> Delete(int id) => await _categoryPostDAO.Delete(id);
    }
}

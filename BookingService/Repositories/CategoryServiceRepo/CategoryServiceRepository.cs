using BookingService.DAOs;
using BookingService.Models;

namespace BookingService.Repositories.CategoryServiceRepo
{
    public class CategoryServiceRepository : ICategoryServiceRepository
    {
        private readonly CategoryServiceDAO _categoryServiceDAO;

        public CategoryServiceRepository(CategoryServiceDAO categoryServiceDAO)
        {
            _categoryServiceDAO = categoryServiceDAO;
        }

        public async Task<IEnumerable<CategoryService>> GetAllCategoryService() => await _categoryServiceDAO.GetAllCategoryService();
        public async Task<CategoryService> GetCategoryServiceById(int id) => await _categoryServiceDAO.GetCategoryServiceById(id);
        public async Task AddCategoryService(CategoryService catSer) => await _categoryServiceDAO.AddCategoryService(catSer);
        public async Task UpdateCategoryService(CategoryService catSer) => await _categoryServiceDAO.UpdateCategoryService(catSer);
        public async Task DeleteCategoryService(int id) => await _categoryServiceDAO.DeleteCategoryService(id);
    }
}

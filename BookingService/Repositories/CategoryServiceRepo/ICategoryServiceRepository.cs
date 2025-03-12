using BookingService.Models;

namespace BookingService.Repositories.CategoryServiceRepo
{
    public interface ICategoryServiceRepository
    {
        Task<CategoryService> GetCategoryServiceById(int id);
        Task<IEnumerable<CategoryService>> GetAllCategoryService();
        Task AddCategoryService(CategoryService item);
        Task UpdateCategoryService(CategoryService item);
        Task DeleteCategoryService(int id);
    }
}

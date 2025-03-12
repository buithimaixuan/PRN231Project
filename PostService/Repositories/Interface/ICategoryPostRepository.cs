using PostService.Models;

namespace PostService.Repositories.Interface
{
    public interface ICategoryPostRepository
    {
        Task<IEnumerable<CategoryPost>> GetAllCategory();
        Task<CategoryPost> FindById(int id);
        Task<int> Add(CategoryPost item);
        Task<int> Update(CategoryPost item);
        Task<int> Delete(int id);
    }
}

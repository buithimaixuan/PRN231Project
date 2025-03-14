using PostService.Models;

namespace PostService.Services.Interface
{
    public interface ICategoryPostService
    {
        Task<IEnumerable<CategoryPost>> GetAllCategory();
        Task<CategoryPost> FindById(int id);

        Task<int> AddCatPost(CategoryPost item);
        Task<int> UpdateCatPost(CategoryPost item);
        Task<int> DeleteCatPost(int id);
    }
}

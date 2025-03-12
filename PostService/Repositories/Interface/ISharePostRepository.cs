using PostService.Models;

namespace PostService.Repositories.Interface
{
    public interface ISharePostRepository
    {
        Task<IEnumerable<SharePost>> GetAllSharePostByPostId(int id);
    }
}

using PostService.Models;

namespace PostService.Repositories.Interface
{
    public interface IPostRepository
    {
        Task<IEnumerable<Post>> GetAll();
        Task<Post?> GetById(int postId);
        Task<Post> AddPost(Post post);
        Task<int> UpdatePost(Post post);
        Task<int> DeletePost(int postId);
        Task<IEnumerable<Post>> GetAllPostByAccountId(int id);

        Task<int> GetTotalPostRepo();
        Task<Dictionary<int, int>> CountPostsByAccount();
    }
}

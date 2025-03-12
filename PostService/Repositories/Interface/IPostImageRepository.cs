using PostService.Models;

namespace PostService.Repositories.Interface
{
    public interface IPostImageRepository
    {
        Task<IEnumerable<PostImage>> GetAll();
        Task<IEnumerable<PostImage>> GetAllByPostId(int postId);
        Task<int> AddPostImage(PostImage postImage);
        Task<int> DeleteImage(PostImage postImage);
    }
}

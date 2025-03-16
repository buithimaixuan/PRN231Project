using PostService.Models;

namespace PostService.Services.Interface
{
    public interface IPostImageService
    {
        Task<IEnumerable<PostImage>> GetPostImagesByPostId(int postId);
        Task AddPostImage(int postId, string urlImage, string publicId);
        Task<int> DeletePostImage(int postImageId);
    }
}

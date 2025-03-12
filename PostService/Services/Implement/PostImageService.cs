using PostService.Models;
using PostService.Repositories.Interface;
using PostService.Services.Interface;

namespace PostService.Services.Implement
{
    public class PostImageService : IPostImageService
    {
        private readonly IPostImageRepository _postImageRepository;

        public PostImageService(IPostImageRepository postImageRepository)
        {
            _postImageRepository = postImageRepository;
        }

        public async Task AddPostImage(int postId, string urlImage)
        {
            var postImage = new PostImage();
            postImage.PostId = postId;
            postImage.ImageUrl = urlImage;
            postImage.IsDeleted = false;

            await _postImageRepository.AddPostImage(postImage);
        }

        public async Task<int> DeletePostImage(PostImage postImage)
        {
            return await _postImageRepository.DeleteImage(postImage);
        }

        public async Task<IEnumerable<PostImage>> GetPostImagesByPostId(int postId)
        {
            return await _postImageRepository.GetAllByPostId(postId);
        }
    }
}

using PostService.Config;
using PostService.DAOs;
using PostService.Models;
using PostService.Repositories.Interface;
using PostService.Services.Interface;

namespace PostService.Services.Implement
{
    public class PostImageService : IPostImageService
    {
        private readonly IPostImageRepository _postImageRepository;
        private readonly CloudinaryConfig cloudinaryConfig;

        public PostImageService(IPostImageRepository postImageRepository, CloudinaryConfig cloudinaryConfig)
        {
            _postImageRepository = postImageRepository;
            this.cloudinaryConfig = cloudinaryConfig;
        }

        public async Task AddPostImage(int postId, string urlImage, string publicId)
        {
            var postImage = new PostImage();
            postImage.PostId = postId;
            postImage.ImageUrl = urlImage;
            postImage.PublicId = publicId;
            postImage.IsDeleted = false;

            await _postImageRepository.AddPostImage(postImage);
        }

        public async Task<int> DeletePostImage(int postImageId)
        {
            return await _postImageRepository.DeleteImage(postImageId);
        }

        public async Task<IEnumerable<PostImage>> GetPostImagesByPostId(int postId)
        {
            return await _postImageRepository.GetAllByPostId(postId);
        }

        public async Task DeleteAllByPostId(int postId)
        {
            IEnumerable<PostImage> postImages = await _postImageRepository.GetAllByPostId(postId);

            foreach (PostImage postImage in postImages)
            {
                await cloudinaryConfig.DeleteImageAsync(postImage.PublicId);
            }

            await _postImageRepository.DeleteAllByPostId(postId);
        }
    }
}

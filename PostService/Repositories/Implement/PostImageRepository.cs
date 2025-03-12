using PostService.DAOs;
using PostService.Models;
using PostService.Repositories.Interface;

namespace PostService.Repositories.Implement
{
    public class PostImageRepository : IPostImageRepository
    {
        private readonly PostImageDAO _postImageDAO;
        public PostImageRepository(PostImageDAO postImageDAO)
        {
            _postImageDAO = postImageDAO;
        }

        public async Task<int> AddPostImage(PostImage postImage)
        {
            return await _postImageDAO.Add(postImage);
        }

        public Task<int> DeleteImage(PostImage postImage)
        {
            return _postImageDAO.Delete(postImage);
        }

        public async Task<IEnumerable<PostImage>> GetAll()
        {
            return await _postImageDAO.GetAll();
        }

        public async Task<IEnumerable<PostImage>> GetAllByPostId(int postId)
        {
            return await _postImageDAO.GetAllByPostId(postId);
        }
    }
}

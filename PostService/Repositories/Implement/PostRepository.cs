using PostService.DAOs;
using PostService.Models;
using PostService.Repositories.Interface;

namespace PostService.Repositories.Implement
{
    public class PostRepository : IPostRepository
    {
        private readonly PostDAO _postDAO;
        public PostRepository(PostDAO postDAO)
        {
            _postDAO = postDAO;
        }

        public async Task<Post> AddPost(Post post)
        {
            return await _postDAO.Add(post);
        }

        public Task<int> DeletePost(int postId)
        {
            return _postDAO.Delete(postId);
        }

        public async Task<IEnumerable<Post>> GetAll()
        {
            return await _postDAO.GetAll();
        }

        public async Task<IEnumerable<Post>> GetAllPostByAccountId(int id) => await _postDAO.GetAllPostByAccountId(id);

        public async Task<Post?> GetById(int postId)
        {
            return await _postDAO.GetById(postId);
        }

        public async Task<int> UpdatePost(Post post)
        {
            return await _postDAO.Update(post);
        }
    }
}

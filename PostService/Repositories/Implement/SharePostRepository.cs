using PostService.DAOs;
using PostService.Models;
using PostService.Repositories.Interface;

namespace PostService.Repositories.Implement
{
    public class SharePostRepository : ISharePostRepository
    {
        private readonly SharePostDAO _sharePostDAO;

        public SharePostRepository(SharePostDAO sharePostDAO)
        {
            _sharePostDAO = sharePostDAO;
        }

        public async Task<IEnumerable<SharePost>> GetAllSharePostByPostId(int id) => await _sharePostDAO.GetAllSharePostByPostId(id);
        public async Task DeleteAllByPostId(int postId) => await _sharePostDAO.DeleteAllByPostId(postId);
    }
}

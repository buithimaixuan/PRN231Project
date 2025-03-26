using PostService.DAOs;
using PostService.Models;
using PostService.Repositories.Interface;

namespace PostService.Repositories.Implement
{
    public class ViewRepository : IViewRepository
    {
        private readonly ViewDAO _viewDAO;

        public ViewRepository(ViewDAO viewDAO)
        {
            _viewDAO = viewDAO;
        }

        public async Task<int> AddRecordPost(int acc_id, int post_id)
        {
            var view = new View
            {
                AccountId = acc_id,
                PostId = post_id,
            };
            return await _viewDAO.AddRecordPost(view);
        }

        public async Task<int> GetViewByPostId(int postId)
        {
            return await _viewDAO.GetViewByPostId(postId);
        }

        public async Task DeleteAllByPostId(int postId) => await _viewDAO.DeleteAllByPostId(postId);
    }
}

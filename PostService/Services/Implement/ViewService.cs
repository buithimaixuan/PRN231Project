using PostService.Repositories.Interface;
using PostService.Services.Interface;

namespace PostService.Services.Implement
{
    public class ViewService : IViewService
    {
        private readonly IViewRepository _viewRepository;
        public ViewService(IViewRepository viewRepository)
        {
            _viewRepository = viewRepository;
        }

        public async Task AddRecordPost(int acc_id, int post_id)
        {
            await _viewRepository.AddRecordPost(acc_id, post_id);
        }

        public async Task<int> GetViewByPostId(int postId)
        {
            return await _viewRepository.GetViewByPostId(postId);
        }
    }
}

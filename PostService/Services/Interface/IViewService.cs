namespace PostService.Services.Interface
{
    public interface IViewService
    {
        Task<int> GetViewByPostId(int postId);
        Task AddRecordPost(int acc_id, int post_id);
    }
}

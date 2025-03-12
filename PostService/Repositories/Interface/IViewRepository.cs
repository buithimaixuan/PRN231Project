namespace PostService.Repositories.Interface
{
    public interface IViewRepository
    {
        Task<int> GetViewByPostId(int postId);
        Task<int> AddRecordPost(int acc_id, int post_id);
    }
}

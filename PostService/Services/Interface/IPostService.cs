using PostService.DAOs;
using PostService.DTOs;
using PostService.Models;

namespace PostService.Services.Interface
{
    public interface IPostService
    {
        Task<List<PostDTO>> GetListPostAndImage();
        Task<List<PostDTO>> GetListPostAvailable();
        Task<PostDTO> GetPostAndImage(int postId);
        //Task<Account?> FarmerWithMostPosts();
        //Task<Account?> ExpertWithMostPosts();
        Task<bool> LikePost(int postId, int accountId);
        Task<bool> UnlikePost(int postId, int accountId);
        Task<bool> IsPostLikedByUser(int postId, int accountId);
        Task<int> GetLikeCountByPostId(int postId);
        Task<List<PostDTO>> GetAllPostByAccountId(int id);
        //Task<List<PostDTO>> GetAllPostImagesByAccountId(int id);
        Task<Post> AddPost(int categoryId, int accountId, string content);
        Task<int> DeletePost(int postId);
        Task<int> UpdatePost(int postId, int categoryid, int? accountId, string content);
        Task<List<PostImage>> GetAllPostImagesByAccountId(int id);
        Task<Comment> FindCommentById(int id);
        Task AddComment(Comment item);
        Task UpdateComment(Comment item);
        Task DeleteComment(int id);


     

        Task<int> GetTotalPostService();
        Task<Dictionary<int, int>> CountPostsByAccount();
    }
}

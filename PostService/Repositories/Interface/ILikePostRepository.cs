using PostService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostService.Repositories.Interface
{
    public interface ILikePostRepository
    {
        Task<IEnumerable<LikePost>> GetAllLikePostByPostId(int id);
        Task<LikePost> FindById(int id);
        Task<LikePost> AddLike(int accountId, int postId, bool isLike = true);
        Task RemoveLike(int accountId, int postId);
        Task UpdateLike(int likePostId, bool isLike);
    }
}

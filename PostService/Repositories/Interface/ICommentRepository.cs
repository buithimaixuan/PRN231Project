using PostService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostService.Repositories.Interface
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetAllCommentPostByPostId(int id);
        Task<Comment> FindById(int id);
        Task<Comment> Add(int? accountId, int? postId, string content); // Thay đổi đây
        Task Update(Comment item);
        Task Delete(int id);
    }
}

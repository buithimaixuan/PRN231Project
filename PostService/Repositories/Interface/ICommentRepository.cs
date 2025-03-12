using PRN221_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN221_Repository.CommentRepo
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetAllCommentPostByPostId(int id);
        Task<Comment> FindById(int id);
        Task Add(Comment item);
        Task Update(Comment item);
        Task Delete(int id);
    }
}

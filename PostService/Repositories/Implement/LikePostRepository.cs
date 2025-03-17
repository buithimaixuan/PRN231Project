using PostService.DAOs;
using PostService.Models;
using PostService.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostService.Repositories.Implement
{
    public class LikePostRepository : ILikePostRepository
    {
        private readonly LikePostDAO _likePostDAO;

        public LikePostRepository(LikePostDAO likePostDAO)
        {
            _likePostDAO = likePostDAO;
        }

        public async Task<IEnumerable<LikePost>> GetAllLikePostByPostId(int id) => await _likePostDAO.GetAllLikePostByPostId(id);
        public async Task<LikePost> FindById(int id) => await _likePostDAO.FindById(id);
        public async Task<LikePost> AddLike(int accountId, int postId, bool isLike = true) => await _likePostDAO.AddLike(accountId, postId, isLike);
        public async Task RemoveLike(int accountId, int postId) => await _likePostDAO.RemoveLike(accountId, postId);
        public async Task UpdateLike(int likePostId, bool isLike) => await _likePostDAO.UpdateLike(likePostId, isLike);
    }
}

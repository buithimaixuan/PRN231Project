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

        public async Task<int> GetLikeCountByPostId(int postId) => await _likePostDAO.GetLikeCountByPostId(postId);

        public async Task<bool> IsPostLikedByUser(int postId, int accountId) => await _likePostDAO.IsPostLikedByUser(postId, accountId);

        public async Task<bool> LikePost(int postId, int accountId) => await _likePostDAO.LikePost(postId, accountId);

        public async Task<bool> UnlikePost(int postId, int accountId) => await _likePostDAO.UnlikePost(postId, accountId);
    }
}

using Microsoft.EntityFrameworkCore;
using PostService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostService.DAOs
{
    public class LikePostDAO : SingletonBaseDAO<LikePostDAO>
    {
        public async Task<IEnumerable<LikePost>> GetAllLikePostByPostId(int postId)
        {
            return await _context.LikePosts
                .Where(l => l.PostId == postId && (l.Post.IsDeleted != true) && l.UnLike != 1) // Kiểm tra IsDeleted != true
                .ToListAsync();
        }

        public async Task<LikePost> FindById(int id)
        {
            var item = await _context.LikePosts.FirstOrDefaultAsync(obj => obj.LikePostId == id);
            if (item == null) return null;
            return item;
        }

        public async Task<LikePost> AddLike(int accountId, int postId, bool isLike = true)
        {
            var existingLike = await _context.LikePosts
                .FirstOrDefaultAsync(l => l.AccountId == accountId && l.PostId == postId);

            if (existingLike != null)
            {
                if (existingLike.UnLike == 1)
                {
                    existingLike.UnLike = 0;
                    await _context.SaveChangesAsync();
                    return existingLike;
                }
                return existingLike;
            }

            var like = new LikePost
            {
                AccountId = accountId,
                PostId = postId,
                UnLike = isLike ? 0 : 1,
                Post = null
            };

            _context.LikePosts.Add(like);
            await _context.SaveChangesAsync();
            return like;
        }

        public async Task RemoveLike(int accountId, int postId)
        {
            var like = await _context.LikePosts
                .FirstOrDefaultAsync(l => l.AccountId == accountId && l.PostId == postId);

            if (like != null)
            {
                _context.LikePosts.Remove(like);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateLike(int likePostId, bool isLike)
        {
            var like = await FindById(likePostId);
            if (like == null) return;

            like.UnLike = isLike ? 0 : 1;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAllByPostId(int postId)
        {
            var likes = await _context.LikePosts
                                 .Where(c => c.PostId == postId)
                                 .ToListAsync();

            if (likes.Any())
            {
                _context.LikePosts.RemoveRange(likes);
                await _context.SaveChangesAsync();
            }
        }
    }
}
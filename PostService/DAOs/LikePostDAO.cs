using Microsoft.EntityFrameworkCore;
using PRN221_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN221_DataAccess.DAOs
{
    public class LikePostDAO : SingletonBase<LikePostDAO>
    {
        public async Task<IEnumerable<LikePost>> GetAllLikePostByPostId(int id)
        {
            return await _context.LikePosts.Where(l => l.PostId == id).ToListAsync();
        }

        public async Task<bool> LikePost(int postId, int accountId)
        {
            var existingLike = await _context.LikePosts
                .FirstOrDefaultAsync(lp => lp.PostId == postId && lp.AccountId == accountId);

            if (existingLike == null)
            {
                var likePost = new LikePost
                {
                    PostId = postId,
                    AccountId = accountId,
                    UnLike = 0
                };

                _context.LikePosts.Add(likePost);
                await _context.SaveChangesAsync();
                return true;
            }
            else if (existingLike.UnLike == 1)
            {
                // Nếu người dùng đã từng bỏ like, chuyển nó thành like lại
                existingLike.UnLike = 0;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> UnlikePost(int postId, int accountId)
        {
            var existingLike = await _context.LikePosts
                .FirstOrDefaultAsync(lp => lp.PostId == postId && lp.AccountId == accountId);

            if (existingLike != null)
            {
                //existingLike.UnLike = 1;
                existingLike.UnLike = 1;

                // Cập nhật đối tượng đã tồn tại
                _context.LikePosts.Update(existingLike);
                await _context.SaveChangesAsync();
                return true;
            }

            return false; // Không tìm thấy like để xóa
        }

        public async Task<bool> IsPostLikedByUser(int postId, int accountId)
        {
            return await _context.LikePosts.AnyAsync(lp => lp.PostId == postId && lp.AccountId == accountId && lp.UnLike == 0);
        }

        public async Task<int> GetLikeCountByPostId(int postId)
        {
            return await _context.LikePosts.CountAsync(lp => lp.PostId == postId && lp.UnLike == 0);
        }


    }
}

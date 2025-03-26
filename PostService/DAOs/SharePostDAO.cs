using Microsoft.EntityFrameworkCore;
using PostService.Models;

namespace PostService.DAOs
{
    public class SharePostDAO : SingletonBaseDAO<SharePostDAO>
    {
        public async Task<IEnumerable<SharePost>> GetAllSharePostByPostId(int id)
        {
            return await _context.SharePosts.Where(l => l.PostId == id).ToListAsync();
        }

        public async Task DeleteAllByPostId(int postId)
        {
            var shares = await _context.SharePosts
                                 .Where(c => c.PostId == postId)
                                 .ToListAsync();

            if (shares.Any())
            {
                _context.SharePosts.RemoveRange(shares);
                await _context.SaveChangesAsync();
            }
        }
    }
}

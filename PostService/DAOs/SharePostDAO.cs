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
    }
}

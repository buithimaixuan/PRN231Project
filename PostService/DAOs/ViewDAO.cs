using Microsoft.EntityFrameworkCore;
using PostService.Models;

namespace PostService.DAOs
{
    public class ViewDAO : SingletonBaseDAO<ViewDAO>
    {
        public async Task<int> GetViewByPostId(int postId)
        {
            var response = _context.Views.Where(v => v.PostId == postId).Count();
            return response > 0 ? response : 0;
        }

        public async Task<int> AddRecordPost(View view)
        {
            _context.Views.Add(view);
            return await _context.SaveChangesAsync();
        }

        public async Task DeleteAllByPostId(int postId)
        {
            var views = await _context.Views
                                 .Where(c => c.PostId == postId)
                                 .ToListAsync();

            if (views.Any())
            {
                _context.Views.RemoveRange(views);
                await _context.SaveChangesAsync();
            }
        }
    }
}

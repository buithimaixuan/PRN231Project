using Microsoft.EntityFrameworkCore;
using PostService.Models;
using System.Diagnostics;

namespace PostService.DAOs
{
    public class PostDAO : SingletonBaseDAO<PostDAO>
    {
        public async Task<IEnumerable<Post>> GetAll()
        {
            return await _context.Posts.ToListAsync();
        }

        public async Task<Post> GetById(int postId)
        {
            return await _context.Posts.FirstOrDefaultAsync(p => p.PostId == postId);
        }
        //public async Task<int> GetPostCountByYear(int year)
        //{
        //    return _context.Posts.Count(p => p.CreatedAt.HasValue && p.CreatedAt.Value.Year == year && p.IsDeleted == false);
        //}

        public async Task<int[]> GetPostCountByYear(int year)
        {
            int[] monthlyCounts = new int[12];

            var results = await _context.Posts
                .Where(p => p.CreatedAt.HasValue && p.CreatedAt.Value.Year == year && p.IsDeleted == false)
                .GroupBy(p => p.CreatedAt.Value.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToListAsync();

            foreach (var result in results)
            {
                monthlyCounts[result.Month - 1] = result.Count; // Tháng 1 là chỉ số 0 trong mảng
            }

            return monthlyCounts;
        }


        public async Task<Post?> Add(Post post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<int> Update(Post post)
        {
            var existItem = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == post.PostId);
            if (existItem == null) return -1;

            existItem.UpdateAt = post.UpdateAt;
            existItem.PostContent = post.PostContent;
            existItem.CategoryPostId = post.CategoryPostId;
            existItem.IsDeleted = post.IsDeleted;

            return await _context.SaveChangesAsync();
        }

        public async Task<int> Delete(int postId)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == postId);
            if (post == null) return -1;
            post.IsDeleted = true;
            post.DeletedAt = DateTime.Now;
            return _context.SaveChanges();
        }

        public async Task<IEnumerable<Post>> GetAllPostByAccountId(int id)
        {
            return await _context.Posts.Where(p => p.AccountId == id).ToListAsync();
        }


        //****************minhuyen************




        public async Task<int> GetTotalPostCountAsync()
        {
            return await _context.Posts.CountAsync(n => n.IsDeleted != true);
        }

        public async Task<int?> GetAccountWithMostPostsThisMonth()
        {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;

            var accountWithMostPosts = await _context.Posts
                .Where(p => p.IsDeleted == false &&
                            p.CreatedAt.HasValue &&
                            p.CreatedAt.Value.Year == currentYear &&
                            p.CreatedAt.Value.Month == currentMonth)
                .GroupBy(p => p.AccountId)
                .Select(g => new
                {
                    AccountId = g.Key,
                    PostCount = g.Count()
                })
                .OrderByDescending(g => g.PostCount)
                .FirstOrDefaultAsync();

            return accountWithMostPosts?.AccountId;
        }




        public async Task<Dictionary<int, int>> CountPostsByAccount()
        {
            return await _context.Posts
                .Where(p => p.AccountId != null && p.IsDeleted == false)
                .GroupBy(p => p.AccountId)
                .Select(g => new { AccountId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.AccountId.Value, x => x.Count);
        }

        //****************minhuyen************


    }
}

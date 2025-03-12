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

        //public async Task<Account?> FarmerWithMostPosts()
        //{
        //    var currentYear = DateTime.Now.Year;
        //    var currentMonth = DateTime.Now.Month;

        //    var farmerWithMostPosts = await _context.Posts
        //        .Where(p => p.IsDeleted == false &&
        //                    p.CreatedAt.HasValue &&
        //                     p.CreatedAt.Value.Year == currentYear &&
        //                     p.CreatedAt.Value.Month == currentMonth) // Chỉ lấy các bài viết không bị xóa và thuộc tháng/năm hiện tại
        //        .Join(_context.Accounts, // Kết hợp với bảng Accounts để lấy RoleId
        //            post => post.AccountId,
        //            account => account.AccountId,
        //            (post, account) => new { post, account }) // Tạo đối tượng mới với post và account
        //        .Where(pa => pa.account.RoleId == 1) // Lọc chỉ những bài viết của account có RoleId = 1
        //        .GroupBy(pa => pa.post.AccountId) // Nhóm theo AccountId
        //        .Select(g => new
        //        {
        //            AccountId = g.Key,
        //            PostCount = g.Count()
        //        })
        //        .OrderByDescending(g => g.PostCount) // Sắp xếp theo số lượng bài viết giảm dần
        //        .FirstOrDefaultAsync();

        //    // Kiểm tra xem có bài viết nào không
        //    if (farmerWithMostPosts != null)
        //    {
        //        Debug.WriteLine("AccountId with most posts: " + farmerWithMostPosts.AccountId);

        //        // Lấy account tương ứng với AccountId
        //        var expert = await _context.Accounts
        //            .FirstOrDefaultAsync(a => a.AccountId == farmerWithMostPosts.AccountId);

        //        if (expert != null)
        //        {
        //            Debug.WriteLine(".....acc....." + expert.AccountId);
        //            Debug.WriteLine(".....role...." + expert.RoleId);
        //            return expert; // Trả về account nếu tồn tại
        //        }
        //        else
        //        {
        //            Debug.WriteLine("No experts found with AccountId: " + farmerWithMostPosts.AccountId);
        //        }
        //    }
        //    else
        //    {
        //        Debug.WriteLine("No posts found for farmers.");
        //    }

        //    Debug.WriteLine("No accounts found with posts.");
        //    return null; // Trả về null nếu không tìm thấy
        //}

        //public async Task<Account?> ExpertWithMostPosts()
        //{
        //    var currentYear = DateTime.Now.Year;
        //    var currentMonth = DateTime.Now.Month;

        //    var expertWithMostPosts = await _context.Posts
        //        .Where(p => p.IsDeleted == false &&
        //                    p.CreatedAt.HasValue &&
        //                     p.CreatedAt.Value.Year == currentYear &&
        //                     p.CreatedAt.Value.Month == currentMonth) // Chỉ lấy các bài viết không bị xóa
        //        .Join(_context.Accounts, // Kết hợp với bảng Accounts để lấy RoleId
        //            post => post.AccountId,
        //            account => account.AccountId,
        //            (post, account) => new { post, account }) // Tạo đối tượng mới với post và account
        //        .Where(pa => pa.account.RoleId == 2) // Lọc chỉ những bài viết của account có RoleId = 2
        //        .GroupBy(pa => pa.post.AccountId) // Nhóm theo AccountId
        //        .Select(g => new
        //        {
        //            AccountId = g.Key,
        //            PostCount = g.Count()
        //        })
        //        .OrderByDescending(g => g.PostCount) // Sắp xếp theo số lượng bài viết giảm dần
        //        .FirstOrDefaultAsync();

        //    // Kiểm tra xem có bài viết nào không
        //    if (expertWithMostPosts != null)
        //    {
        //        Debug.WriteLine("AccountId with most posts: " + expertWithMostPosts.AccountId);

        //        // Lấy account tương ứng với AccountId
        //        var expert = await _context.Accounts
        //            .FirstOrDefaultAsync(a => a.AccountId == expertWithMostPosts.AccountId);

        //        if (expert != null)
        //        {
        //            Debug.WriteLine(".....acc....." + expert.AccountId);
        //            Debug.WriteLine(".....role...." + expert.RoleId);
        //            return expert; // Trả về account nếu tồn tại
        //        }
        //        else
        //        {
        //            Debug.WriteLine("No experts found with AccountId: " + expertWithMostPosts.AccountId);
        //        }
        //    }
        //    else
        //    {
        //        Debug.WriteLine("No posts found for experts.");
        //    }

        //    Debug.WriteLine("No accounts found with posts.");
        //    return null; // Trả về null nếu không tìm thấy
        //}

    }
}

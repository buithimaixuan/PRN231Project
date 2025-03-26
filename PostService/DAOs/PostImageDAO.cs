using Microsoft.EntityFrameworkCore;
using PostService.Models;

namespace PostService.DAOs
{
    public class PostImageDAO : SingletonBaseDAO<PostImageDAO>
    {
        public async Task<IEnumerable<PostImage>> GetAll()
        {
            return await _context.PostImages.ToListAsync();
        }

        public async Task<IEnumerable<PostImage>> GetAllByPostId(int postId)
        {
            return await _context.PostImages.Where(img => img.PostId == postId).ToListAsync();
        }

        public async Task<int> Add(PostImage postImage)
        {
            _context.PostImages.Add(postImage);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> Delete(int postImageId)
        {
            var postImage = await _context.PostImages.FindAsync(postImageId);
            if (postImage == null)
            {
                return -1; // Không tìm thấy ảnh
            }

            _context.PostImages.Remove(postImage);
            return await _context.SaveChangesAsync();
        }

        public async Task DeleteAllByPostId(int postId)
        {
            var images = await _context.PostImages
                                 .Where(c => c.PostId == postId)
                                 .ToListAsync();

            if (images.Any())
            {
                _context.PostImages.RemoveRange(images);
                await _context.SaveChangesAsync();
            }
        }
    }
}

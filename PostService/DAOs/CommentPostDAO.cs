using Microsoft.EntityFrameworkCore;
using PostService.DAOs;
using PostService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostService.DAOs
{
    public class CommentPostDAO : SingletonBaseDAO<CommentPostDAO>
    {
        public async Task<IEnumerable<Comment>> GetAllCommentPostByPostId(int id)
        {
            return await _context.Comments.Where(l => l.PostId == id).ToListAsync();
        }
        public async Task<Comment> FindById(int id)
        {
            var item = await _context.Comments.FirstOrDefaultAsync(obj => obj.CommentId == id);
            if (item == null) return null;
            return item;
        }

        public async Task Add(Comment item)
        {
            item.IsDeleted = false;
            _context.Comments.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Comment item)
        {
            var existingItem = await FindById(item.CommentId);
            if (existingItem == null) return;
            _context.Entry(existingItem).CurrentValues.SetValues(item);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var item = await FindById(id);
            if (item == null) return;

            item.IsDeleted = true;
            _context.Comments.Update(item);
            await _context.SaveChangesAsync();
        }
    }
}

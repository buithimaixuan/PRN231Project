﻿using PostService.DAOs;
using PostService.Models;
using PostService.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostService.Repositories.Implement
{
    public class CommentRepository : ICommentRepository
    {
        private readonly CommentPostDAO _commentPostDAO;

        public CommentRepository(CommentPostDAO commentPostDAO)
        {
            _commentPostDAO = commentPostDAO;
        }

        public async Task<Comment> Add(int? accountId, int? postId, string content)
        {
            return await _commentPostDAO.Add(accountId, postId, content); // Trả về Comment
        }

        public async Task Delete(int id) => await _commentPostDAO.Delete(id);

        public async Task<Comment> FindById(int id) => await _commentPostDAO.FindById(id);

        public async Task<IEnumerable<Comment>> GetAllCommentPostByPostId(int id) => await _commentPostDAO.GetAllCommentPostByPostId(id);

        public async Task Update(Comment item) => await _commentPostDAO.Update(item);

        public async Task DeleteAllByPostId(int postId) => await _commentPostDAO.DeleteAllByPostId(postId);
    }
}

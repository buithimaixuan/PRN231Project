﻿using Microsoft.EntityFrameworkCore;
using PostService.DTOs;
using PostService.Models;
using PostService.Repositories.Interface;
using PostService.Services.Interface;

namespace PostService.Services.Implement
{
    public class PostsService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IPostImageRepository _postImageRepository;
        private readonly ILikePostRepository _likePostRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly ISharePostRepository _sharePostRepository;

        public PostsService(IPostRepository postRepository, IPostImageRepository postImageRepository, ILikePostRepository likePostRepository, ICommentRepository commentRepository, ISharePostRepository sharePostRepository)
        {
            _postRepository = postRepository;
            _postImageRepository = postImageRepository;
            _likePostRepository = likePostRepository;
            _commentRepository = commentRepository;
            _sharePostRepository = sharePostRepository;
        }

        public async Task<List<PostDTO>> GetAllPostByAccountId(int id)
        {
            var response = new List<PostDTO>();
            var listPost = await _postRepository.GetAllPostByAccountId(id);

            foreach (var item in listPost)
            {
                if (item.IsDeleted == false)
                {
                    var listImageByPost = await _postImageRepository.GetAllByPostId(item.PostId);
                    var likePost = await _likePostRepository.GetAllLikePostByPostId(item.PostId);
                    var comment = await _commentRepository.GetAllCommentPostByPostId(item.PostId);
                    var sharePost = await _sharePostRepository.GetAllSharePostByPostId(item.PostId);

                    var postItemDto = new PostDTO(item, listImageByPost, likePost, comment, sharePost);

                    response.Add(postItemDto);
                }
            }

            return response;
        }

        public async Task<List<PostImage>> GetAllPostImagesByAccountId(int id)
        {
            var allImages = new List<PostImage>();

            // Lấy tất cả bài viết của tài khoản theo accountId
            var listPost = await _postRepository.GetAllPostByAccountId(id);

            // Lặp qua từng bài viết và lấy hình ảnh
            foreach (var post in listPost)
            {
                var listImagesByPost = await _postImageRepository.GetAllByPostId(post.PostId);

                // Thêm hình ảnh vào danh sách tổng hợp
                allImages.AddRange(listImagesByPost);
            }

            return allImages;
        }

        //Lấy toàn bộ post bao gồm đã bị xóa
        public async Task<List<PostDTO>> GetListPostAndImage()
        {
            var response = new List<PostDTO>();
            var listPost = await _postRepository.GetAll();

            foreach (var item in listPost)
            {
                var listImageByPost = await _postImageRepository.GetAllByPostId(item.PostId);
                var likePost = await _likePostRepository.GetAllLikePostByPostId(item.PostId);
                var comment = await _commentRepository.GetAllCommentPostByPostId(item.PostId);
                var sharePost = await _sharePostRepository.GetAllSharePostByPostId(item.PostId);

                var postItemDto = new PostDTO(item, listImageByPost, likePost, comment, sharePost);

                response.Add(postItemDto);
            }

            return response;
        }

        public async Task<PostDTO> GetPostAndImage(int postId)
        {
            var response = new PostDTO();
            var post = await _postRepository.GetById(postId);

            if (post == null) return null;

            response.post = post;
            response.postImages = await _postImageRepository.GetAllByPostId(post.PostId);
            response.likePosts = await _likePostRepository.GetAllLikePostByPostId(post.PostId);
            response.comments = await _commentRepository.GetAllCommentPostByPostId(post.PostId);
            response.sharePosts = await _sharePostRepository.GetAllSharePostByPostId(post.PostId);

            return response;
        }

        public async Task<IEnumerable<LikePost>> GetAllLikePostByPostId(int postId)
        {
            return await _likePostRepository.GetAllLikePostByPostId(postId);
        }

        public async Task<LikePost> GetLikeById(int id)
        {
            var like = await _likePostRepository.FindById(id);
            if (like == null)
                throw new Exception("Like not found.");
            return like;
        }

        public async Task<LikePost> AddLike(int accountId, int postId, bool isLike = true)
        {
            if (accountId <= 0 || postId <= 0)
                throw new ArgumentException("AccountId and PostId must be positive.");

            var like = await _likePostRepository.AddLike(accountId, postId, isLike);
            return like;
        }

        public async Task RemoveLike(int accountId, int postId)
        {
            if (accountId <= 0 || postId <= 0)
                throw new ArgumentException("AccountId and PostId must be positive.");
            await _likePostRepository.RemoveLike(accountId, postId);
        }

        public async Task UpdateLike(int likePostId, bool isLike)
        {
            if (likePostId <= 0)
                throw new ArgumentException("Invalid likePostId.");
            await _likePostRepository.UpdateLike(likePostId, isLike);
        }

        public async Task<Post> AddPost(int categoryId, int accountId, string content)
        {
            var post = new Post
            {
                CategoryPostId = categoryId,
                AccountId = accountId,
                PostContent = content,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            return await _postRepository.AddPost(post);
        }

        //Lấy list post không bị xóa
        public async Task<List<PostDTO>> GetListPostAvailable()
        {
            var response = new List<PostDTO>();
            var listPost = await _postRepository.GetAll();

            foreach (var item in listPost)
            {
                var listImageByPost = await _postImageRepository.GetAllByPostId(item.PostId);
                var likePost = await _likePostRepository.GetAllLikePostByPostId(item.PostId);
                var comment = await _commentRepository.GetAllCommentPostByPostId(item.PostId);
                var sharePost = await _sharePostRepository.GetAllSharePostByPostId(item.PostId);

                var postItemDto = new PostDTO(item, listImageByPost, likePost, comment, sharePost);

                if (item.IsDeleted == false)
                {
                    response.Add(postItemDto);
                }
            }

            return response;
        }

        //public Task<int> GetLikeCountByPostId(int postId)
        //{
        //    return _likePostRepository.GetLikeCountByPostId(postId);
        //}

        public Task<int> DeletePost(int postId)
        {
            return _postRepository.DeletePost(postId);
        }
        public async Task<IEnumerable<Comment>> GetAllCommentPostByPostId(int id)
        {
            return await _commentRepository.GetAllCommentPostByPostId(id);
        }

        public async Task<Comment> FindCommentById(int id)
        {
            return await _commentRepository.FindById(id);
        }

        public async Task<Comment> AddComment(int? accountId, int? postId, string content)
        {
            // Kiểm tra đầu vào cơ bản
            if (string.IsNullOrEmpty(content))
                throw new ArgumentException("Content cannot be empty.");
            if (accountId <= 0 || postId <= 0)
                throw new ArgumentException("AccountId and PostId must be positive.");

            // Thêm bình luận
            var comment = await _commentRepository.Add(accountId, postId, content); // Trả về Comment
            return comment;
        }

        public async Task UpdateComment(Comment item)
        {
            await _commentRepository.Update(item);
        }

        public async Task DeleteComment(int id)
        {
            await _commentRepository.Delete(id);
        }

        public async Task<int> UpdatePost(int postId, int categoryid, int? accountId, string content)
        {
            DateTime currentUtcDateTime = DateTime.UtcNow;

            var post = new Post
            {
                PostId = postId,
                AccountId = accountId,
                CategoryPostId = categoryid,
                PostContent = content,
                UpdateAt = currentUtcDateTime,
                IsDeleted = false
            };

            return await _postRepository.UpdatePost(post);
        }

        public async Task<int> GetTotalExpertService() => await _postRepository.GetTotalPostRepo();

       public Task<Dictionary<int, int>> CountPostsByAccount()
    {
        return _postRepository.CountPostsByAccount();
    }

        public Task<int> GetTotalPostService()
        {
            throw new NotImplementedException();
        }
    }
}

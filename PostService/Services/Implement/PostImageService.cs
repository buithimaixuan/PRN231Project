﻿using PostService.Models;
using PostService.Repositories.Interface;
using PostService.Services.Interface;

namespace PostService.Services.Implement
{
    public class PostImageService : IPostImageService
    {
        private readonly IPostImageRepository _postImageRepository;

        public PostImageService(IPostImageRepository postImageRepository)
        {
            _postImageRepository = postImageRepository;
        }

        public async Task AddPostImage(int postId, string urlImage, string publicId)
        {
            var postImage = new PostImage();
            postImage.PostId = postId;
            postImage.ImageUrl = urlImage;
            postImage.PublicId = publicId;
            postImage.IsDeleted = false;

            await _postImageRepository.AddPostImage(postImage);
        }

        public async Task<int> DeletePostImage(int postImageId)
        {
            return await _postImageRepository.DeleteImage(postImageId);
        }

        public async Task<IEnumerable<PostImage>> GetPostImagesByPostId(int postId)
        {
            return await _postImageRepository.GetAllByPostId(postId);
        }
    }
}

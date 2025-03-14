﻿using PostService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostService.Repositories.Interface
{
    public interface ILikePostRepository
    {
        Task<IEnumerable<LikePost>> GetAllLikePostByPostId(int id);
        Task<bool> LikePost(int postId, int accountId);
        Task<bool> UnlikePost(int postId, int accountId);
        Task<bool> IsPostLikedByUser(int postId, int accountId);
        Task<int> GetLikeCountByPostId(int postId);
    }
}

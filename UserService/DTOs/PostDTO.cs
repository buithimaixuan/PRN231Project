namespace UserService.DTOs
{
    public class PostDTO
    {
        public Post post { get; set; }
        public IEnumerable<PostImage> postImages { get; set; }
        public IEnumerable<LikePost> likePosts { get; set; }
        public IEnumerable<Comment> comments { get; set; }
        public IEnumerable<SharePost> sharePosts { get; set; }

        public PostDTO()
        {

        }

        public PostDTO(Post post, IEnumerable<PostImage> postImages, IEnumerable<LikePost> likePosts, IEnumerable<Comment> comments, IEnumerable<SharePost> sharePosts)
        {
            this.post = post;
            this.postImages = postImages;
            this.likePosts = likePosts;
            this.comments = comments;
            this.sharePosts = sharePosts;
        }

        public class Post
        {
            public int PostId { get; set; }
            public int CategoryPostId { get; set; }
            public int? AccountId { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdateAt { get; set; }
            public string? PostContent { get; set; }
            public bool? IsDeleted { get; set; }
            public DateTime? DeletedAt { get; set; }
        }

        public class PostImage
        {
            public int PostImageId { get; set; }
            public int PostId { get; set; }
            public string ImageUrl { get; set; } = null!;
            public bool? IsDeleted { get; set; }
            public string PublicId { get; set; }

        }

        public class LikePost
        {
            public int LikePostId { get; set; }

            public int PostId { get; set; }

            public int AccountId { get; set; }

            public int? UnLike { get; set; }
        }
        public class Comment
        {
            public int CommentId { get; set; }

            public int? AccountId { get; set; }

            public int? PostId { get; set; }

            public string? Content { get; set; }

            public int? Rate { get; set; }

            public DateOnly CreatedAt { get; set; }

            public DateOnly? UpdatedAt { get; set; }

            public bool? IsDeleted { get; set; }
        }

        public class SharePost
        {
            public int SharePostId { get; set; }

            public int PostId { get; set; }

            public int SharerId { get; set; }

            public DateOnly ShareAt { get; set; }

            public bool? IsDeleted { get; set; }
        }
    } 
}

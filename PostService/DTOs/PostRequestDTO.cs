namespace PostService.DTOs
{
    public class PostRequestDTO
    {
        public int PostId { get; set; }

        public int CategoryPostId { get; set; }

        public int? AccountId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdateAt { get; set; }

        public string? PostContent { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        public PostRequestDTO(int postId, int categoryPostId, int? accountId, DateTime? createdAt, DateTime? updateAt, string? postContent, bool? isDeleted, DateTime? deletedAt)
        {
            PostId = postId;
            CategoryPostId = categoryPostId;
            AccountId = accountId;
            CreatedAt = createdAt;
            UpdateAt = updateAt;
            PostContent = postContent;
            IsDeleted = isDeleted;
            DeletedAt = deletedAt;
        }
    }
}

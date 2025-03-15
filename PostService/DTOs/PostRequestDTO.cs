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
    }
}

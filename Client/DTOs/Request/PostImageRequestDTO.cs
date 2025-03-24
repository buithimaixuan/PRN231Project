namespace Client.DTOs.Request
{
    public class PostImageRequestDTO
    {
        public int PostImageId { get; set; }

        public int PostId { get; set; }

        public string ImageUrl { get; set; } = null!;

        public bool IsDeleted { get; set; }
        public string PublicId { get; set; }

        public PostImageRequestDTO(int postImageId, int postId, string imageUrl, bool isDeleted, string publicId)
        {
            PostImageId = postImageId;
            PostId = postId;
            ImageUrl = imageUrl;
            IsDeleted = isDeleted;
            PublicId = publicId;
        }

        public PostImageRequestDTO()
        {
        }
    }
}

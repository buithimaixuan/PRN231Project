namespace PostService.DTOs
{
    public class NewsDTO
    {
        public int CategoryNewsId { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string? ImageUrl { get; set; }

        public NewsDTO(int categoryNewsId, string title, string content, string? imageUrl)
        {
            CategoryNewsId = categoryNewsId;
            Title = title;
            Content = content;
            ImageUrl = imageUrl;
        }
    }

}

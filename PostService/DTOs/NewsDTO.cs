namespace PostService.DTOs
{
    public class NewsDTO
    {
        public int CategoryNewsId { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string? ImageUrl { get; set; }
    }

}

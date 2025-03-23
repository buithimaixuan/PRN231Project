using Client.Models;

namespace Client.DTOs
{
    public class NewsFilterResponse
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public List<News> News { get; set; }
    }
}

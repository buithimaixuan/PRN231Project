using Client.Models;

namespace Client.ViewModel
{
    public class NewsDetailViewModel
    {
        public News NewsDetail { get; set; }
        public CategoryNews CategoryNews { get; set; }
        public List<CategoryNews> CategoryNewsList { get; set; } = new List<CategoryNews>();
        public List<News> List2News { get; set; } = new List<News>(); // Danh sách 2 tin tức mới nhất
        public string SearchKey { get; set; }
        public Dictionary<int, string> CategoryNames { get; set; } = new Dictionary<int, string>();
    }
}

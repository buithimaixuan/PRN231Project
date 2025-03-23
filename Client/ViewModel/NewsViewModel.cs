using Client.Models;

namespace Client.ViewModel
{
    public class NewsViewModel
    {
        public List<News> NewsList { get; set; } = new List<News>();
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public string SearchKey { get; set; }
        public int? Category { get; set; }
        public string CategoryName { get; set; } // Thêm thuộc tính để lưu tên danh mục
        public List<CategoryNews> CategoryNewsList { get; set; } = new List<CategoryNews>();
        public Dictionary<int, string> CategoryNames { get; set; } = new Dictionary<int, string>(); // Lưu tên danh mục cho từng tin tức
    }
}

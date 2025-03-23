using Client.DTOs;
using Client.Models;
using Client.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Client.Controllers
{
    public class NewsController : Controller
    {
        private readonly HttpClient _clientNews;
        private readonly string _newsUrl;

        public NewsController()
        {
            _clientNews = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _clientNews.DefaultRequestHeaders.Accept.Add(contentType); // Chỉ thêm header Accept
            _newsUrl = "https://localhost:7231/api/News";
        }

        public async Task<IActionResult> Index(int? cat, string searchKey = "", int p = 1)
        {
            try
            {
                // Gọi API FilterNews
                var query = $"?categoryId={cat}&searchString={searchKey}&pageNumber={p}&pageSize=8";
                HttpResponseMessage response = await _clientNews.GetAsync($"{_newsUrl}/FilterNews{query}");

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    var newsResponse = JsonConvert.DeserializeObject<NewsFilterResponse>(data);

                    // Gọi API để lấy danh sách danh mục có tin tức
                    HttpResponseMessage categoryResponse = await _clientNews.GetAsync($"{_newsUrl}/NewsCategory/HaveNews");
                    List<CategoryNews> categoryList = new List<CategoryNews>();
                    if (categoryResponse.IsSuccessStatusCode)
                    {
                        string categoryData = await categoryResponse.Content.ReadAsStringAsync();
                        categoryList = JsonConvert.DeserializeObject<List<CategoryNews>>(categoryData);
                    }

                    ViewBag.Categories = categoryList;

                    // Lấy tên danh mục cho Category hiện tại (nếu có)
                    string categoryName = null;
                    if (cat.HasValue)
                    {
                        categoryName = await GetCategoryName(cat.Value);
                    }

                    // Lấy tên danh mục cho từng tin tức
                    var categoryNames = new Dictionary<int, string>();
                    foreach (var news in newsResponse.News)
                    {
                        if (!categoryNames.ContainsKey(news.CategoryNewsId))
                        {
                            categoryNames[news.CategoryNewsId] = await GetCategoryName(news.CategoryNewsId);
                        }
                    }

                    // Tạo ViewModel
                    var viewModel = new NewsViewModel
                    {
                        NewsList = newsResponse.News,
                        TotalPages = newsResponse.TotalPages,
                        CurrentPage = newsResponse.PageNumber,
                        SearchKey = searchKey,
                        Category = cat,
                        CategoryName = categoryName,
                        CategoryNewsList = categoryList,
                        CategoryNames = categoryNames
                    };

                    return View(viewModel);
                }
                else
                {
                    ViewBag.Error = "Không thể tải danh sách tin tức";
                    return View(new NewsViewModel());
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Có lỗi xảy ra: {ex.Message}";
                return View(new NewsViewModel());
            }
        }

        public async Task<IActionResult> NewsDetail(int id)
        {
            try
            {
                // Lấy chi tiết tin tức
                HttpResponseMessage newsResponse = await _clientNews.GetAsync($"{_newsUrl}/{id}");
                if (!newsResponse.IsSuccessStatusCode)
                {
                    return NotFound();
                }
                string newsData = await newsResponse.Content.ReadAsStringAsync();
                var newsDetail = JsonConvert.DeserializeObject<News>(newsData);

                // Lấy danh mục của tin tức
                HttpResponseMessage categoryResponse = await _clientNews.GetAsync($"{_newsUrl}/Categories/{newsDetail.CategoryNewsId}");
                CategoryNews categoryNews = null;
                if (categoryResponse.IsSuccessStatusCode)
                {
                    string categoryData = await categoryResponse.Content.ReadAsStringAsync();
                    categoryNews = JsonConvert.DeserializeObject<CategoryNews>(categoryData);
                }

                // Lấy danh sách danh mục có tin tức
                HttpResponseMessage categoryListResponse = await _clientNews.GetAsync($"{_newsUrl}/NewsCategory/HaveNews");
                List<CategoryNews> categoryList = new List<CategoryNews>();
                if (categoryListResponse.IsSuccessStatusCode)
                {
                    string categoryListData = await categoryListResponse.Content.ReadAsStringAsync();
                    categoryList = JsonConvert.DeserializeObject<List<CategoryNews>>(categoryListData);
                }

                // Lấy 2 tin tức mới nhất
                HttpResponseMessage latestNewsResponse = await _clientNews.GetAsync($"{_newsUrl}/Latest?count=2");
                List<News> latestNews = new List<News>();
                if (latestNewsResponse.IsSuccessStatusCode)
                {
                    string latestNewsData = await latestNewsResponse.Content.ReadAsStringAsync();
                    latestNews = JsonConvert.DeserializeObject<List<News>>(latestNewsData);
                }

                // Lấy tên danh mục cho List2News
                var categoryNames = new Dictionary<int, string>();
                foreach (var news in latestNews)
                {
                    if (!categoryNames.ContainsKey(news.CategoryNewsId))
                    {
                        categoryNames[news.CategoryNewsId] = await GetCategoryName(news.CategoryNewsId);
                    }
                }

                // Tạo ViewModel
                var viewModel = new NewsDetailViewModel
                {
                    NewsDetail = newsDetail,
                    CategoryNews = categoryNews,
                    CategoryNewsList = categoryList,
                    List2News = latestNews,
                    SearchKey = "",
                    CategoryNames = categoryNames
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Có lỗi xảy ra: {ex.Message}";
                return View(new NewsDetailViewModel());
            }
        }

        public async Task<string> GetCategoryName(int categoryId)
        {
            HttpResponseMessage response = await _clientNews.GetAsync($"{_newsUrl}/Categories/{categoryId}");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var category = JsonConvert.DeserializeObject<CategoryNews>(data);
                return category?.CategoryNewsName ?? "Không xác định";
            }
            return "Không xác định";
        }
    }
}

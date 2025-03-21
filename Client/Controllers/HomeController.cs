using Client.DTOs;
using Client.Models;
using Client.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient client;
        private string authenUrl = "";
        private string accountUrl = "";
        private string postUrl = "";
        private string categoryPostUrl = "";

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            authenUrl = "https://localhost:7272/api/Auth";
            accountUrl = "https://localhost:7272/api/Accounts";
            postUrl = "https://localhost:7231/api/post";
            categoryPostUrl = "https://localhost:7231/api/post-categories";
            this._logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            int accId = GetSessionAccId();
            var viewModel = new HomeViewModel();

            // Tạo danh sách các task để gọi API song song
            var tasks = new List<Task>();

            //Lay account cua nguoi dung hien tai
            if (accId > 0)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var accountInfor = await client.GetAsync($"{accountUrl}/DetailFarmer{accId}");
                    if (accountInfor.IsSuccessStatusCode)
                    {
                        var responseAccount = await accountInfor.Content.ReadAsStringAsync();
                        viewModel.Account = JsonConvert.DeserializeObject<Account>(responseAccount);
                    }
                }));
            }

            //Lấy danh sách bài viết từ PostService
            var postTask = client.GetAsync($"{postUrl}/all/available");
            tasks.Add(postTask);

            //Lấy danh mục bài viết
            var categoryTask = client.GetAsync($"{categoryPostUrl}");
            tasks.Add(categoryTask);

            // Chạy tất cả các task song song
            await Task.WhenAll(tasks);

            //Xu ly lay list post o day, neu co
            var listPostDTO = await postTask;
            if (listPostDTO.IsSuccessStatusCode)
            {
                var responseListPost = await listPostDTO.Content.ReadAsStringAsync();
                viewModel.PostDTOs = JsonConvert.DeserializeObject<List<PostDTO>>(responseListPost);

                // ✅ Gọi API AccountService để lấy thông tin Account của từng bài viết song song
                var accountTasks = viewModel.PostDTOs.Select(async postDto =>
                {
                    var accountResponse = await client.GetAsync($"{accountUrl}/DetailFarmer{postDto.post.AccountId}");
                    if (accountResponse.IsSuccessStatusCode)
                    {
                        var responseAccount = await accountResponse.Content.ReadAsStringAsync();
                        postDto.Account = JsonConvert.DeserializeObject<Account>(responseAccount);
                    }
                });
                await Task.WhenAll(accountTasks);
            }

            //Xu ly category post
            var listCategoryPost = await categoryTask;
            if (listCategoryPost.IsSuccessStatusCode)
            {
                var responseListCategoryPost = await listCategoryPost.Content.ReadAsStringAsync();
                viewModel.categoryPosts = JsonConvert.DeserializeObject<List<CategoryPost>>(responseListCategoryPost);
            }

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private int GetSessionAccId()
        {
            return HttpContext.Session.GetInt32("AccountID") ?? -1;
        }
    }
}

using Client.DTOs;
using Client.Models;
using Client.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient client;
        private string authenUrl = "";
        private string accountUrl = "";
        private string postUrl = "";
        private string categoryPostUrl = "";
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            authenUrl = "https://localhost:7272/api/Auth";
            accountUrl = "https://localhost:7272/api/Accounts";
            postUrl = "https://localhost:7231/api/post";
            categoryPostUrl = "https://localhost:7231/api/post-categories";
            this._logger = logger;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            int accId = GetCookiesAccId();
            var viewModel = new HomeViewModel();

            //Lay account cua nguoi dung hien tai
            if (accId > 0)
            {
                var accountInfor = await client.GetAsync($"{accountUrl}/DetailFarmer{accId}");
                if (accountInfor.IsSuccessStatusCode)
                {
                    var responseAccount = await accountInfor.Content.ReadAsStringAsync();
                    viewModel.Account = JsonConvert.DeserializeObject<Account>(responseAccount);
                }
            }

            // Lấy danh sách bài viết từ PostService
            var postResponse = await client.GetAsync($"{postUrl}/all/available");
            if (postResponse.IsSuccessStatusCode)
            {
                var responseListPost = await postResponse.Content.ReadAsStringAsync();
                viewModel.PostDTOs = JsonConvert.DeserializeObject<List<PostDTO>>(responseListPost);

                // Lấy thông tin Account của từng bài viết
                foreach (var postDto in viewModel.PostDTOs)
                {
                    var accountResponse = await client.GetAsync($"{accountUrl}/DetailFarmer{postDto.post.AccountId}");
                    if (accountResponse.IsSuccessStatusCode)
                    {
                        var responseAccount = await accountResponse.Content.ReadAsStringAsync();
                        postDto.Account = JsonConvert.DeserializeObject<Account>(responseAccount);
                    }
                }
            }

            // Lấy danh mục bài viết
            var categoryResponse = await client.GetAsync($"{categoryPostUrl}");
            if (categoryResponse.IsSuccessStatusCode)
            {
                var responseListCategoryPost = await categoryResponse.Content.ReadAsStringAsync();
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

        public int GetCookiesAccId()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null || !httpContext.User.Identity.IsAuthenticated)
            {
                return -1; // Người dùng chưa đăng nhập
            }

            var accountIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "AccountID");
            if (accountIdClaim != null && int.TryParse(accountIdClaim.Value, out int accountId))
            {
                return accountId;
            }

            return -1; // Không tìm thấy AccountID
        }

    }
}

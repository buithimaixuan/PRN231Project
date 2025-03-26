using Client.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace Client.Controllers
{
 
    public class StatisticController : Controller
    {

        private readonly HttpClient client;
        private string StatisticPostUri = "";
        private string StatisticNewUri = "";
        private string StatisticAccUri = "";
        private string StatisticServiceUri = "";
        private readonly IHttpClientFactory httpClient;

        public StatisticController(IHttpClientFactory clientFactory)
        {
            httpClient = clientFactory;
            client = clientFactory.CreateClient();
            StatisticPostUri = "http://localhost:5007/api/post";
            StatisticNewUri = "http://localhost:5007/api/News";
            StatisticAccUri = "http://localhost:5157/api/Accounts";
            StatisticServiceUri = "http://localhost:5122/api/Services";

        }

        //public async Task<IActionResult> PostStatistics(int year = 2025)
        //{
        //    var apiUrl = $"https://localhost:7231/api/post/countPostInYear/{year}";
        //    int[] postCounts = new int[12]; // Mặc định là 12 tháng, tránh null

        //    try
        //    {
        //        var response = await client.GetAsync(apiUrl);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            var jsonData = await response.Content.ReadAsStringAsync();
        //            postCounts = JsonConvert.DeserializeObject<int[]>(jsonData) ?? new int[12];
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Error = $"Lỗi khi gọi API: {ex.Message}";
        //    }

        //    return View(postCounts);
        //}

        public async Task<IActionResult> Index(int? year)
        {
            int totalPosts = await GetTotalPosts();
            Console.WriteLine($"Total Posts: {totalPosts}");
            ViewBag.TotalPosts = totalPosts;

            int totalNews = await GetTotalNews();
            Console.WriteLine($"Total Posts: {totalNews}");
            ViewBag.TotalNews = totalNews;

            int totalServices = await GetTotalServices();
            Console.WriteLine($"Total Posts: {totalServices}");
            ViewBag.TotalServices = totalServices;

            int totalExperts = await GetTotalExperts();
            Console.WriteLine($"Total Posts: {totalExperts}");
            ViewBag.TotalExperts = totalExperts;

            int totalFarmers = await GetTotalFarmers();
            Console.WriteLine($"Total Posts: {totalFarmers}");
            ViewBag.TotalFarmers = totalFarmers;


            int selectedYear = year ?? DateTime.Now.Year; // Nếu không có thì lấy năm hiện tại
            var apiUrl = $"https://localhost:7231/api/post/countPostInYear/{selectedYear}";
            int[] postCounts = new int[12];

            try
            {
                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    postCounts = JsonConvert.DeserializeObject<int[]>(jsonData) ?? new int[12];
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Lỗi khi gọi API: {ex.Message}";
            }

            ViewBag.SelectedYear = selectedYear; // Truyền năm đã chọn xuống View
            ViewBag.PostCounts = postCounts;



            var apiUrlTopAccount = "https://localhost:7231/api/post/accountWithMostPosts";

            try
            {
                var response = await client.GetAsync(apiUrlTopAccount);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                    if (jsonData != null && jsonData.ContainsKey("message"))
                    {
                        string message = jsonData["message"];

                        if (int.TryParse(message, out int accountId))
                        {
                            string fullName = await GetFullNameById(accountId);
                            ViewBag.TopAccountMessage = $"{fullName} (ID: {accountId})";
                        }
                        else
                        {
                            ViewBag.TopAccountMessage = message;
                        }
                    }
                    else
                    {
                        ViewBag.TopAccountMessage = "Không thể lấy dữ liệu";
                    }
                }
                else
                {
                    ViewBag.TopAccountMessage = "Không thể lấy dữ liệu";
                }
            }
            catch (Exception ex)
            {
                ViewBag.TopAccountMessage = $"Lỗi khi gọi API: {ex.Message}";
            }

            return View();
        }



        private async Task<string> GetFullNameById(int accountId)
        {
            var apiUrl = $"https://localhost:7272/api/Accounts/GetFullName/{accountId}";

            try
            {
                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);

                    if (result != null && result.ContainsKey("fullName"))
                        return result["fullName"];

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gọi API lấy FullName: {ex.Message}");
            }

            return "Không xác định";
        }


        public async Task<int> GetTotalPosts()
        {
            using var client = httpClient.CreateClient();
            var responseP = await client.GetFromJsonAsync<int>($"{StatisticPostUri}/total-post");
            return responseP;
        }

        public async Task<int> GetTotalNews()
        {
            using var client = httpClient.CreateClient();
            var responseN = await client.GetFromJsonAsync<int>($"{StatisticNewUri}/total-news");
            return responseN;
        }

        public async Task<int> GetTotalExperts()
        {
            using var client = httpClient.CreateClient();
            var responseE = await client.GetFromJsonAsync<int>($"{StatisticAccUri}/total-experts");
            return responseE;
        }

        public async Task<int> GetTotalFarmers()
        {
            using var client = httpClient.CreateClient();
            var responseF = await client.GetFromJsonAsync<int>($"{StatisticAccUri}/total-farmers");
            return responseF;
        }

        public async Task<int> GetTotalServices()
        {
            using var client = httpClient.CreateClient();
            var response = await client.GetFromJsonAsync<int>($"{StatisticServiceUri}/count-all");
            return response;
        }


    }
}

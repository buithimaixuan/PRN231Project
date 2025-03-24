using Client.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace Client.Controllers
{
    public class StatisticController : Controller
    {

        private readonly HttpClient client;
        private string StatisticPostUri = "";
        private string StatisticNewUri = "";
        private string StatisticServiceUri = "";
        private readonly IHttpClientFactory httpClient;

        public StatisticController(IHttpClientFactory clientFactory)
        {
            httpClient = clientFactory;
            StatisticPostUri = "http://localhost:5007/api/post";
            StatisticNewUri = "http://localhost:5007/api/News";
            StatisticServiceUri = "http://localhost:5122/api/Services";
        }




        //[HttpGet("MonthlyPostChart/{year}")]
        //public async Task<IActionResult> MonthlyPostChart(int year)
        //{
        //    var client = httpClient.CreateClient();
        //    var monthlyCounts = await client.GetFromJsonAsync<int[]>($"{StatisticPostUri}/countPostInYear/{year}");

        //    ViewBag.MonthlyCounts = monthlyCounts;
        //    ViewBag.Year = year;

        //    return View();
        //}

        [HttpGet("MonthlyPostChart/{year}")]
        public async Task<IActionResult> MonthlyPostChart(int year)
        {
            var client = httpClient.CreateClient();
            var monthlyCounts = await client.GetFromJsonAsync<int[]>($"{StatisticPostUri}/countPostInYear/{year}");

            return Json(monthlyCounts); // Trả về dữ liệu JSON
        }

        public async Task<IActionResult> Index()
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
            return View();
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

        public async Task<int> GetTotalServices()
        {
            using var client = httpClient.CreateClient();
            var response = await client.GetFromJsonAsync<int>($"{StatisticServiceUri}/count-all");
            return response;
        }


    }
}

using Azure;
using Client.DTOs;
using Client.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Client.Controllers
{
    public class CategoryNewsAdminController : Controller
    {
        private readonly IHttpClientFactory httpClient;

        private string BookApiUrl = "";

        public CategoryNewsAdminController(IHttpClientFactory clientFactory)
        {
            httpClient = clientFactory;
            BookApiUrl = "http://localhost:5007/api/News/NewsCategory/All";
        }
        public async Task<IActionResult> Index()
        {
            var client = httpClient.CreateClient();
            //call odata service
            var responseNews = await client.GetStringAsync(BookApiUrl);
            var listCat = JsonConvert.DeserializeObject<IEnumerable<CategoryNews>>(responseNews);
            return View(listCat);

        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {

            return View();


        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryNews book)
        {
            var client = httpClient.CreateClient();
            if (ModelState.IsValid)
            {
                var content = new StringContent(JsonConvert.SerializeObject(book), System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync("http://localhost:5007/api/News/NewsCategory/Create", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(book);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = httpClient.CreateClient();
            var responseCat = await client.GetStringAsync($"http://localhost:5007/api/News/Categories/{id}");

            var listCat = JsonConvert.DeserializeObject<CategoryNews>(responseCat);
        
            return View(listCat);


        }


        [HttpPost]
        public async Task<IActionResult> Edit(CategoryNews book)
        {
            var client = httpClient.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(book), System.Text.Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"http://localhost:5007/api/News/NewsCategory/Edit/{book.CategoryNewsId}", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View(book);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var client = httpClient.CreateClient();
            var responseCat = await client.GetStringAsync($"http://localhost:5007/api/News/Categories/{id}");

            var listCat = JsonConvert.DeserializeObject<CategoryNews>(responseCat);

            return View(listCat);


        }
    }
}

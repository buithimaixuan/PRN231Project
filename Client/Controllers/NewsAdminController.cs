using Client.DTOs;
using Client.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Client.Controllers
{
    public class NewsAdminController : Controller
    {
        private readonly IHttpClientFactory httpClient;

        private string BookApiUrl = "";

        public NewsAdminController(IHttpClientFactory clientFactory)
        {
            httpClient = clientFactory;
            BookApiUrl = "http://localhost:5007/api/News/Available";
        }
        public async Task<IActionResult> Index()
        {
            var client = httpClient.CreateClient();
            //call odata service
            var responseNews = await client.GetStringAsync(BookApiUrl);
            //deserialize the respone of list Book
            var listNews = JsonConvert.DeserializeObject<IEnumerable<News>>(responseNews);

            var responseCat = await client.GetStringAsync("http://localhost:5007/api/News/NewsCategory/All");

            var listCat = JsonConvert.DeserializeObject<IEnumerable<CategoryNews>>(responseCat);

            var listIndex = new List<NewsAdminDTO>();
            var itemIndex = new NewsAdminDTO();
            foreach (var item in listNews)
            {
               
                itemIndex.NewsId = item.NewsId;
                itemIndex.CategoryNewsId = item.CategoryNewsId;
                itemIndex.Title = item.Title;
                itemIndex.Content = item.Content;
                itemIndex.ImageUrl = item.ImageUrl;
                itemIndex.CreatedAt = item.CreatedAt;
                itemIndex.UpdatedAt = item.UpdatedAt;
                itemIndex.DeletedAt = item.DeletedAt;
               
                itemIndex.IsDeleted = item.IsDeleted;
                foreach (var item1 in listCat)
                {
                    if(item.CategoryNewsId == item1.CategoryNewsId)
                    {
                        itemIndex.CategoryNewsName = item1.CategoryNewsName;
                        listIndex.Add(itemIndex);
                        itemIndex = new NewsAdminDTO();
                        break;
                    }
                    
                }
            }


            return View(listIndex.AsEnumerable());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var client = httpClient.CreateClient();
            var responseCat = await client.GetStringAsync("http://localhost:5007/api/News/NewsCategory/All");

            var listCat = JsonConvert.DeserializeObject<IEnumerable<CategoryNews>>(responseCat)?.ToList();
            var model = new NewsAdminDTO
            {
                Categories = listCat
            };
            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> Create(NewsAdminDTO book)
        {
            var client = httpClient.CreateClient();
            

            var part1 = Request.Form.Files.GetFile("Image");


            string avatar = part1 != null ? await UploadImage(part1, client) : "";


            var expert = new NewsDTO(book.CategoryNewsId,book.Title, book.Content, avatar);


            var content = new StringContent(JsonConvert.SerializeObject(expert), System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://localhost:5007/api/News", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
           
            return View(book);
        }
        private async Task<string> UploadImage(IFormFile file, HttpClient client)
        {
            using var content = new MultipartFormDataContent();
            using var fileStream = file.OpenReadStream();
            content.Add(new StreamContent(fileStream), "formFile", file.FileName);

            var response = await client.PostAsync("http://localhost:5007/api/cloudinary/upload", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error uploading image.");
            }

            var responseData = await response.Content.ReadFromJsonAsync<ImageUploadResponseDTO>();

            return responseData.ImageUrl;
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = httpClient.CreateClient();

            var responseNews = await client.GetStringAsync($"http://localhost:5007/api/News/{id}");

            var news = JsonConvert.DeserializeObject<NewsAdminDTO>(responseNews);

            var responseCat = await client.GetStringAsync("http://localhost:5007/api/News/NewsCategory/All");

            var listCat = JsonConvert.DeserializeObject<IEnumerable<CategoryNews>>(responseCat)?.ToList();
            news.Categories = listCat;
           
            return View(news);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(NewsAdminDTO book)
        {
            var client = httpClient.CreateClient();

            var responseNews = await client.GetStringAsync($"http://localhost:5007/api/News/{book.NewsId}");

            var news = JsonConvert.DeserializeObject<NewsAdminDTO>(responseNews);

            var part1 = Request.Form.Files.GetFile("Image");


            string avatar = part1 != null ? await UploadImage(part1, client) : "";


            if (avatar.Equals(""))
            {
                book.ImageUrl = news.ImageUrl;
            }
            else
            {
                book.ImageUrl = avatar;
            }

            var expert = new NewsDTO(book.CategoryNewsId, book.Title, book.Content, book.ImageUrl);


            var content = new StringContent(JsonConvert.SerializeObject(expert), System.Text.Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"http://localhost:5007/api/News/{book.NewsId}", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View(book);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var client = httpClient.CreateClient();

            var responseNews = await client.GetStringAsync($"http://localhost:5007/api/News/{id}");

            var news = JsonConvert.DeserializeObject<NewsAdminDTO>(responseNews);

            var responseCat = await client.GetStringAsync("http://localhost:5007/api/News/NewsCategory/All");

            var listCat = JsonConvert.DeserializeObject<IEnumerable<CategoryNews>>(responseCat)?.ToList();
            news.Categories = listCat;

            return View(news);

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = httpClient.CreateClient();
            var response = await client.DeleteAsync($"http://localhost:5007/api/News/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index"); // Or wherever you want to redirect after deletion
            }

            // Handle failure (optional)
            ModelState.AddModelError("", "Failed to delete the Expert.");
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var client = httpClient.CreateClient();

            var responseNews = await client.GetStringAsync($"http://localhost:5007/api/News/{id}");

            var news = JsonConvert.DeserializeObject<NewsAdminDTO>(responseNews);

            var responseCat = await client.GetStringAsync("http://localhost:5007/api/News/NewsCategory/All");

            var listCat = JsonConvert.DeserializeObject<IEnumerable<CategoryNews>>(responseCat)?.ToList();
            news.Categories = listCat;

            return View(news);

        }
    }
}

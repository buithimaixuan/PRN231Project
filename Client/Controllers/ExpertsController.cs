using Client.DTOs;
using Client.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Client.Controllers
{
    public class ExpertsController : Controller
    {

        private readonly IHttpClientFactory httpClient;

        private string BookApiUrl = "";

        public ExpertsController(IHttpClientFactory clientFactory)
        {
            httpClient = clientFactory;
            BookApiUrl = "http://localhost:5157/api/Accounts/ALlExpert/3";
        }
        public async Task<IActionResult> Index()
        {
            var client = httpClient.CreateClient();
            //call odata service
            var respone = await client.GetStringAsync(BookApiUrl);
            //deserialize the respone of list Book
            var listPro = JsonConvert.DeserializeObject<IEnumerable<Account>>(respone);
            return View(listPro);
        }
        public IActionResult Create() {
            var model = new ExpertDTO(); // Đảm bảo model được khởi tạo
            return View(model);
           
        }

        [HttpPost]
        public async Task<IActionResult> Create(ExpertDTO book)
        {
            var client = httpClient.CreateClient();
            var client2 = httpClient.CreateClient();
            var client3 = httpClient.CreateClient();

            var part1 = Request.Form.Files.GetFile("AvatarFile");
            var part2 = Request.Form.Files.GetFile("DegreeFile");
            var part3 = Request.Form.Files.GetFile("EducationFile");
            //Upload avatar

            string avatar = part1 != null ? await UploadImage(part1, client) : null;
            string degree = part2 != null ? await UploadImage(part2, client2) : null;
            string education = part3 != null ? await UploadImage(part3, client3) : null;

            DateTime? nullableDateTime = book.DateOfBirth;
            DateOnly? dateOnly = nullableDateTime.HasValue ? DateOnly.FromDateTime(nullableDateTime.Value) : null;


            var expert = new AccountDTO
            {
                RoleId = 3,
                FullName = book.FullName,
                Username = book.Username,
                Email = book.Email,
                Phone = book.Phone,
                Gender = book.Gender,
                DateOfBirth = dateOnly,
                ShortBio = book.ShortBio,
                EducationUrl = education,
                YearOfExperience = book.YearOfExperience,
                DegreeUrl = degree,
                Avatar = avatar,
                Major = book.Major,
                Address = book.Address,
                Password = book.Password,
            };


            var content = new StringContent(JsonConvert.SerializeObject(expert), System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://localhost:5157/api/Accounts/Experts/Create", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            Console.WriteLine(expert.FullName);
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

        public async Task<IActionResult> Details(int id) {
            var client = httpClient.CreateClient();
           
            var response = await client.GetStringAsync($"http://localhost:5157/api/Accounts/DetailFarmer{id}");
           
            var account = JsonConvert.DeserializeObject<Account>(response);
            //var Book = Books.FirstOrDefault(); // Get the first Book
           
            return View(account);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client = httpClient.CreateClient();

            var response = await client.GetStringAsync($"http://localhost:5157/api/Accounts/DetailFarmer{id}");

            var account = JsonConvert.DeserializeObject<Account>(response);
            //var Book = Books.FirstOrDefault(); // Get the first Book

            return View(account);
        }
        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = httpClient.CreateClient();
            var response = await client.DeleteAsync($"http://localhost:5157/api/Accounts/DeleteFarmer{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index"); // Or wherever you want to redirect after deletion
            }

            // Handle failure (optional)
            ModelState.AddModelError("", "Failed to delete the Expert.");
            return RedirectToAction("Index");
        }



    }
}

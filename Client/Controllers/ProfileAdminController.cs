using Client.DTOs;
using Client.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Client.Controllers
{
    public class ProfileAdminController : Controller
    {
        private readonly IHttpClientFactory httpClient;

        public ProfileAdminController(IHttpClientFactory httpClientFactory)
        {
            this.httpClient = httpClientFactory;
        }
        public async Task<IActionResult> Index()
        {
            var client = httpClient.CreateClient();
            int? accountId = HttpContext.Session.GetInt32("AccountID");

            var account = new Account();
            if (accountId.HasValue)
            {

                var response = await client.GetStringAsync($"http://localhost:5157/api/Accounts/DetailFarmer{accountId}");
                account = JsonConvert.DeserializeObject<Account>(response);

            }


            return View(account);
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
        public async Task<IActionResult> Edit(int id)
        {
            var client = httpClient.CreateClient();

            var response = await client.GetStringAsync($"http://localhost:5157/api/Accounts/DetailFarmer{id}");

            var account = JsonConvert.DeserializeObject<Account>(response);
            //var Book = Books.FirstOrDefault(); // Get the first Book

            return View(account);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Account account)
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

            //DateTime? nullableDateTime = account.DateOfBirth;
            //DateOnly? dateOnly = nullableDateTime.HasValue ? DateOnly.FromDateTime(nullableDateTime.Value) : null;


            var expert = new AccountDTO
            {
                RoleId = account.RoleId,
                FullName = account.FullName,
                Username = account.Username,
                Email = account.Email,
                Phone = account.Phone,
                Gender = account.Gender,
                DateOfBirth = account.DateOfBirth,
                ShortBio = account.ShortBio,
                EducationUrl = education,
                YearOfExperience = account.YearOfExperience,
                DegreeUrl = degree,
                Avatar = avatar,
                Major = account.Major,
                Address = account.Address,
                Password = account.Password,
            };


            var responseNews = await client.GetStringAsync($"http://localhost:5157/api/Accounts/DetailFarmer{account.AccountId}");

            var oldAcc = JsonConvert.DeserializeObject<Account>(responseNews);

            if (avatar == null)
            {
                expert.Avatar = oldAcc.Avatar;
            }
            else
            {
                expert.Avatar = avatar;
            }

            if (degree == null)
            {
                expert.DegreeUrl = oldAcc.DegreeUrl;
            }
            else
            {
                expert.DegreeUrl = degree;
            }

            if (education == null)
            {
                expert.EducationUrl = oldAcc.EducationUrl;
            }
            else
            {
                expert.EducationUrl = education;
            }


            var content = new StringContent(JsonConvert.SerializeObject(expert), System.Text.Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"http://localhost:5157/api/Accounts/Update/{account.AccountId}", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View(account);
        }
    }
}

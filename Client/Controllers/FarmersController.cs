using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using Client.Models;

namespace Client.Controllers
{
    public class FarmersController : Controller
    {
        private readonly HttpClient client;
        private string VoucherODataUri = "";

        public FarmersController(IHttpClientFactory httpClientFactory)
        {
            //_httpClientFactory = httpClientFactory;


            client = new HttpClient();

            VoucherODataUri = "http://localhost:5157/api/Accounts";




        }

        public async Task<IActionResult> Index()
        {
            //var client = _httpClientFactory.CreateClient();




            var response = await client.GetAsync(VoucherODataUri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var Farmers = JsonSerializer.Deserialize<IEnumerable<Account>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return View(Farmers);
            }
            return View();


        }

        public async Task<IActionResult> Details(int id)
        {
            //var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{VoucherODataUri}/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var account = JsonSerializer.Deserialize<Account>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return View(account);
            }
            return NotFound();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Account form)
        {
            //var client = _httpClientFactory.CreateClient();
            if (ModelState.IsValid)
            {


                var jsonContent = new StringContent(JsonSerializer.Serialize(form), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(VoucherODataUri, jsonContent);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Không thể tạo account. Vui lòng thử lại.");
            }
            return View(form);
        }





        [HttpGet("accounts/edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            //var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{VoucherODataUri}/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var account = JsonSerializer.Deserialize<Account>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });


                return View(account);
            }
            return NotFound();
        }

        //[HttpPost("Edit/{id}")]

        [HttpPost("accounts/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Account form)
        {
            //var client = _httpClientFactory.CreateClient();


            var jsonContent = new StringContent(JsonSerializer.Serialize(form), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{VoucherODataUri}/{id}", jsonContent);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Không thể cập nhật voucher. Vui lòng thử lại.");

            return View(form);
        }


        public async Task<IActionResult> Delete(int id)
        {
            //var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{VoucherODataUri}/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var account = JsonSerializer.Deserialize<Account>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return View(account);
            }
            return NotFound();
        }


        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"{VoucherODataUri}/{id}");
            Console.WriteLine($"dddddd.......{VoucherODataUri}/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Failed to delete the Voucher.");
            return RedirectToAction("Index");
        }
    }
}

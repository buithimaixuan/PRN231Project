using System.Composition;
using System.Net.Http.Headers;
using System.Security.Claims;
using Client.DTOs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Client.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Client.Controllers
{
    public class AuthenController : Controller
    {
        private readonly HttpClient client;
        private string authenUrl = "";
        private string accountUrl = "";

        public AuthenController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            // docker
            authenUrl = "http://localhost:5157/api/Auth";
            accountUrl = "https://localhost:5157/api/Accounts";

            //SWAGGER
            authenUrl = "https://localhost:7272/api/Auth";
            accountUrl = "https://localhost:7272/api/Accounts";
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDTO request)
        {
            var content = new StringContent(JsonConvert.SerializeObject(request), System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{authenUrl}/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonConvert.DeserializeObject<LoginResponseDTO>(responseString);

                // Lưu thông tin vào Cookie Authentication
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Role, loginResponse.RoleId.ToString()),
                    new Claim("AccountID", loginResponse.AccountId.ToString()),
                    new Claim("UserToken", loginResponse.Token) // Lưu UserToken vào Cookie
                };

                var claimsIdentity = new ClaimsIdentity(claims, "CookiesPRN231");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true, 
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // Thời gian hết hạn của Cookie
                };

                await HttpContext.SignInAsync("CookiesPRN231", claimsPrincipal, authProperties);

                // Lưu thông tin vào Session (đồng bộ với Cookie)
                HttpContext.Session.SetString("UserToken", loginResponse.Token);
                HttpContext.Session.SetInt32("UserRole", loginResponse.RoleId);
                HttpContext.Session.SetInt32("AccountID", loginResponse.AccountId);



                if (loginResponse.RoleId == 2 || loginResponse.RoleId == 3)
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (loginResponse.RoleId == 1)
                {
                    return RedirectToAction("Index", "Statistic");
                }
            }

            ViewBag.ErrorMessage = "Invalid login credentials";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("UserToken");
            HttpContext.Session.Remove("UserRole");
            HttpContext.Session.Remove("AccountID");
            await HttpContext.SignOutAsync("CookiesPRN231");
            return RedirectToAction("Index", "Home");
        }
    }
}

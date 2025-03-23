using System.Composition;
using System.Net.Http.Headers;
using System.Security.Claims;
using Client.DTOs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Client.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.Google;

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
            //authenUrl = "http://localhost:5157/api/Auth";
            //accountUrl = "https://localhost:5157/api/Accounts";

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
                    new Claim("UserToken", loginResponse.Token.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true, 
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // Thời gian hết hạn của Cookie
                };

                // Lưu thông tin vào Session (đồng bộ với Cookie)
                HttpContext.Session.SetInt32("UserRole", loginResponse.RoleId);
                HttpContext.Session.SetInt32("AccountID", loginResponse.AccountId);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);

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
        public IActionResult LoginWithGoogle()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleCallback", "Authen")
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleCallback()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
            {
                ViewBag.ErrorMessage = "Google authentication failed.";
                return RedirectToAction("Index");
            }

            // Lấy thông tin từ Google
            var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = authenticateResult.Principal.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                ViewBag.ErrorMessage = "Unable to retrieve email from Google.";
                return RedirectToAction("Index");
            }

            // Gọi API để đăng nhập hoặc đăng ký tài khoản với thông tin từ Google
            var googleLoginRequest = new
            {
                Email = email,
                FullName = name
            };

            var content = new StringContent(JsonConvert.SerializeObject(googleLoginRequest), System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{authenUrl}/google-login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonConvert.DeserializeObject<LoginResponseDTO>(responseString);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Role, loginResponse.RoleId.ToString()),
                    new Claim("AccountID", loginResponse.AccountId.ToString()),
                    new Claim("UserToken", loginResponse.Token)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // Thời gian hết hạn của Cookie
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);

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

            ViewBag.ErrorMessage = "Failed to login with Google.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("UserRole");
            HttpContext.Session.Remove("AccountID");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}

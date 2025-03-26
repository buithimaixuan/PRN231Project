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
using Client.ViewModel;
using static System.Net.Mime.MediaTypeNames;

namespace Client.Controllers
{
    public class AuthenController : Controller
    {
        private readonly HttpClient client;
        private string authenUrl = "";
        private string accountUrl = "";
        private string cloudUrl = "";

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

            cloudUrl = "https://localhost:7231/api/cloudinary";
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
        public async Task<IActionResult> OptionRole()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> RegisterFarmer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterFarmer(RegisterFarmerViewModel request)
        {
            client.Timeout = TimeSpan.FromMinutes(5);

            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Thông tin không hợp lệ.";
                return View(request);
            }

            AccountDTO accountDTO = new AccountDTO();
            accountDTO.FullName = request.FullName;
            accountDTO.Gender = request.Gender ?? "Male";
            accountDTO.DateOfBirth = request.DateOfBirth;
            accountDTO.Phone = request.PhoneNumber;
            accountDTO.Username = request.Username;
            accountDTO.Password = request.Password;
            accountDTO.RoleId = 2;


            var content = new StringContent(JsonConvert.SerializeObject(accountDTO), System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{authenUrl}/register", content);

            if (!response.IsSuccessStatusCode)
            {
                TempData["RegisterSuccess"] = "Đăng ký thành công.";
                TempData.Keep("RegisterSuccess");
                return View(request);
            }

            TempData["RegisterSuccess"] = "Đăng ký thành công.";
            TempData.Keep("RegisterSuccess");
            return RedirectToAction("Index", "Authen");
        }
        [HttpGet]
        public async Task<IActionResult> RegisterExpert()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterExpert([FromForm] RegisterExpertViewModel request)
        {
            if (!ModelState.IsValid)
            {
                // Log lỗi ra console để debug
                foreach (var error in ModelState)
                {
                    foreach (var message in error.Value.Errors)
                    {
                        Console.WriteLine($"Lỗi tại {error.Key}: {message.ErrorMessage}");
                    }
                }
                return View(request);
            }

            AccountDTO accountDTO = new AccountDTO();
            accountDTO.FullName = request.FullName;
            accountDTO.Gender = request.Gender ?? "Male";
            accountDTO.DateOfBirth = request.DateOfBirth;
            accountDTO.Phone = request.PhoneNumber;
            accountDTO.Username = request.Username;
            accountDTO.Password = request.Password;
            accountDTO.RoleId = 3;
            accountDTO.Email = request.Email;
            accountDTO.Address = request.Address;
            accountDTO.Major = request.Major;
            accountDTO.YearOfExperience = request.YearOfExperience;
            accountDTO.ShortBio = request.ShortBio;

            //GỌI API UPLOAD FILE ẢNH
            ImageUploadResponseDTO education = await UploadFileAsync(request.fileEducation);
            accountDTO.EducationUrl = education.ImageUrl;

            ImageUploadResponseDTO degree = await UploadFileAsync(request.fileDegree);
            accountDTO.DegreeUrl = degree.ImageUrl;

            ImageUploadResponseDTO avatar = await UploadFileAsync(request.fileAvatar);
            accountDTO.Avatar = avatar.ImageUrl;

            var content = new StringContent(JsonConvert.SerializeObject(accountDTO), System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{authenUrl}/register", content);

            if (!response.IsSuccessStatusCode)
            {
                TempData["RegisterSuccess"] = "Đăng ký thành công.";
                TempData.Keep("RegisterSuccess");
                return View(request);
            }

            TempData["RegisterSuccess"] = "Đăng ký thành công.";
            TempData.Keep("RegisterSuccess");
            return RedirectToAction("Index", "Authen");
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

        public async Task<ImageUploadResponseDTO> UploadFileAsync(IFormFile file)
        {
            using (var contentCloud = new MultipartFormDataContent())
            {
                var memoryStream = new MemoryStream();
                file.OpenReadStream().CopyTo(memoryStream);
                memoryStream.Position = 0; // Đặt lại vị trí đầu stream

                var fileContent = new StreamContent(memoryStream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

                contentCloud.Add(fileContent, "formFile", file.FileName);

                //Gọi api
                var responseCloud = await client.PostAsync($"{cloudUrl}/upload", contentCloud);

                if (responseCloud.IsSuccessStatusCode)
                {
                    var responseStringImage = await responseCloud.Content.ReadAsStringAsync();
                    ImageUploadResponseDTO imageResponse = JsonConvert.DeserializeObject<ImageUploadResponseDTO>(responseStringImage);

                    return imageResponse;

                }
            }
            return null;
        }

    }
}

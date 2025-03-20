using Client.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Security.Claims;

namespace Client.Controllers
{
    public class ProfileController : Controller
    {
        private readonly HttpClient _clientProfile;
        private readonly string _authenUrl;

        public ProfileController()
        {
            _clientProfile = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _clientProfile.DefaultRequestHeaders.Accept.Add(contentType); // Chỉ thêm header Accept
            _authenUrl = "http://localhost:5157/api/Accounts";
        }

        [Authorize]
        public async Task<IActionResult> PersonalPage(int id)
        {
            try
            {
                // Kiểm tra trạng thái đăng nhập từ Cookie Authentication
                var isLoggedIn = User.Identity.IsAuthenticated;

                // Lấy AccountIDLogin từ Claims
                int? accountIDLogin = null;
                if (isLoggedIn)
                {
                    var accountIdClaim = User.Claims.FirstOrDefault(c => c.Type == "AccountID");
                    if (accountIdClaim != null && int.TryParse(accountIdClaim.Value, out int accountIdFromCookie))
                    {
                        accountIDLogin = accountIdFromCookie;
                    }
                }

                if (!accountIDLogin.HasValue)
                {
                    TempData["ErrorMessage"] = "You must be logged in to view this page.";
                    return RedirectToAction("Index", "Authen");
                }

                // Gọi API GetUserPersonalPage
                string requestUrl = $"{_authenUrl}/personal-page/{id}";
                Console.WriteLine($"Calling API: {requestUrl}");
                var response = await _clientProfile.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        TempData["ErrorMessage"] = $"No account found with account_id {id}";
                        return RedirectToAction("Index", "Home");
                    }

                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to fetch profile: {response.ReasonPhrase}. Details: {errorContent}");
                }

                // Đọc và deserialize dữ liệu từ API
                var content = await response.Content.ReadAsStringAsync();
                var profile = JsonSerializer.Deserialize<PersonalPageDTO>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (profile == null || profile.accountDTO == null)
                {
                    TempData["ErrorMessage"] = "Profile data is not available.";
                    return RedirectToAction("Index", "Home");
                }

                // Truyền dữ liệu vào ViewBag
                ViewBag.IsLoggedIn = isLoggedIn;
                ViewBag.AccountIDLogin = accountIDLogin;

                // Truyền dữ liệu vào View
                return View(profile);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public async Task<IActionResult> SendFriendRequest(int senderId, int receiverId)
        {
            try
            {
                var request = new FriendRequestDTO
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    RequestStatus = "pending"
                };

                var content = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
                var response = await _clientProfile.PostAsync($"{_authenUrl}/friend-request/send", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Failed to send friend request: {errorContent}";
                }
                else
                {
                    TempData["SuccessMessage"] = "Friend request sent successfully.";
                }

                return RedirectToAction("PersonalPage", new { id = receiverId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction("PersonalPage", new { id = receiverId });
            }
        }

        [Authorize]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateFriendRequest(int senderId, int receiverId, string status)
        {
            try
            {
                // Kiểm tra trạng thái đăng nhập và AccountIDLogin
                var isLoggedIn = User.Identity.IsAuthenticated;
                int? accountIDLogin = null;
                if (isLoggedIn)
                {
                    var accountIdClaim = User.Claims.FirstOrDefault(c => c.Type == "AccountID");
                    if (accountIdClaim != null && int.TryParse(accountIdClaim.Value, out int accountIdFromCookie))
                    {
                        accountIDLogin = accountIdFromCookie;
                    }
                }

                if (!accountIDLogin.HasValue || accountIDLogin != receiverId)
                {
                    return Json(new { success = false, message = "You are not authorized to perform this action." });
                }

                var request = new FriendRequestDTO
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    RequestStatus = status
                };

                var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
                var response = await _clientProfile.PutAsync($"{_authenUrl}/friend-request/update", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"Failed to update friend request: {errorContent}" });
                }

                return Json(new { success = true, message = $"Friend request {status} successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [Authorize]
        public async Task<IActionResult> Unfriend(int userId1, int userId2)
        {
            try
            {
                // Kiểm tra trạng thái đăng nhập và AccountIDLogin
                var isLoggedIn = User.Identity.IsAuthenticated;
                int? accountIDLogin = null;
                if (isLoggedIn)
                {
                    var accountIdClaim = User.Claims.FirstOrDefault(c => c.Type == "AccountID");
                    if (accountIdClaim != null && int.TryParse(accountIdClaim.Value, out int accountIdFromCookie))
                    {
                        accountIDLogin = accountIdFromCookie;
                    }
                }

                // Gọi API Unfriend
                var requestUrl = $"{_authenUrl}/friend/unfriend?userId1={userId1}&userId2={userId2}";
                var response = await _clientProfile.DeleteAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Failed to unfriend: {errorContent}";
                }
                else
                {
                    TempData["SuccessMessage"] = "Successfully unfriended.";
                }

                return RedirectToAction("PersonalPage", new { id = userId2 });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction("PersonalPage", new { id = userId2 });
            }
        }
    }
}
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
        private readonly string _accountUrl;

        public ProfileController()
        {
            _clientProfile = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _clientProfile.DefaultRequestHeaders.Accept.Add(contentType); // Chỉ thêm header Accept
            _accountUrl = "http://localhost:5157/api/Accounts";
        }

        [Authorize]
        public async Task<IActionResult> PersonalPage(int id, string view = "posts")
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
                string requestUrl = $"{_accountUrl}/personal-page/{id}";
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
                ViewBag.ViewMode = view; // Truyền view mode vào ViewBag

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
                var response = await _clientProfile.PostAsync($"{_accountUrl}/friend-request/send", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"Failed to send friend request: {errorContent}" });
                }

                return Json(new { success = true, message = "Friend request sent successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

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
                var response = await _clientProfile.PutAsync($"{_accountUrl}/friend-request/update", content);

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

                if (!accountIDLogin.HasValue || (accountIDLogin != userId1 && accountIDLogin != userId2))
                {
                    return Json(new { success = false, message = "You are not authorized to perform this action." });
                }

                // Gọi API Unfriend
                var requestUrl = $"{_accountUrl}/friend/unfriend?userId1={userId1}&userId2={userId2}";
                var response = await _clientProfile.DeleteAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"Failed to unfriend: {errorContent}" });
                }

                return Json(new { success = true, message = "Successfully unfriended." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [Authorize]
        public async Task<IActionResult> ListFriends(int id)
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

                if (!accountIDLogin.HasValue)
                {
                    TempData["ErrorMessage"] = "You must be logged in to view this page.";
                    return RedirectToAction("Index", "Authen");
                }

                // Gọi API để lấy danh sách bạn bè
                string requestUrl = $"{_accountUrl}/all-friends/{id}";
                Console.WriteLine($"Calling API: {requestUrl}");
                var response = await _clientProfile.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        TempData["ErrorMessage"] = $"No friends found for account_id {id}";
                        return RedirectToAction("PersonalPage", new { id });
                    }

                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to fetch friends: {response.ReasonPhrase}. Details: {errorContent}");
                }

                // Đọc và deserialize dữ liệu từ API
                var content = await response.Content.ReadAsStringAsync();
                var friends = JsonSerializer.Deserialize<IEnumerable<FriendRequestDTO>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (friends == null)
                {
                    TempData["ErrorMessage"] = "No friends found.";
                    return RedirectToAction("PersonalPage", new { id });
                }

                // Gọi API để lấy thông tin của user (cho thanh điều hướng hoặc header)
                var profileResponse = await _clientProfile.GetAsync($"{_accountUrl}/personal-page/{id}");
                if (!profileResponse.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = $"Failed to fetch profile for account_id {id}";
                    return RedirectToAction("PersonalPage", new { id });
                }

                var profileContent = await profileResponse.Content.ReadAsStringAsync();
                var profile = JsonSerializer.Deserialize<PersonalPageDTO>(profileContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Truyền dữ liệu vào ViewBag
                ViewBag.IsLoggedIn = isLoggedIn;
                ViewBag.AccountIDLogin = accountIDLogin;
                ViewBag.Profile = profile;

                return View(friends);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction("PersonalPage", new { id });
            }
        }

        [Authorize]
        public async Task<IActionResult> ListPhotos(int id)
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

                if (!accountIDLogin.HasValue)
                {
                    TempData["ErrorMessage"] = "You must be logged in to view this page.";
                    return RedirectToAction("Index", "Authen");
                }

                // Gọi API để lấy danh sách ảnh
                string requestUrl = $"{_accountUrl}/all-photos/{id}";
                Console.WriteLine($"Calling API: {requestUrl}");
                var response = await _clientProfile.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        TempData["ErrorMessage"] = $"No photos found for account_id {id}";
                        return RedirectToAction("PersonalPage", new { id });
                    }

                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to fetch photos: {response.ReasonPhrase}. Details: {errorContent}");
                }

                // Đọc và deserialize dữ liệu từ API
                var content = await response.Content.ReadAsStringAsync();
                var photosDTO = JsonSerializer.Deserialize<AccountPhotosDTO>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (photosDTO == null || photosDTO.Photos == null)
                {
                    TempData["ErrorMessage"] = "No photos found.";
                    return RedirectToAction("PersonalPage", new { id });
                }

                // Gọi API để lấy thông tin của user (cho thanh điều hướng hoặc header)
                var profileResponse = await _clientProfile.GetAsync($"{_accountUrl}/personal-page/{id}");
                if (!profileResponse.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = $"Failed to fetch profile for account_id {id}";
                    return RedirectToAction("PersonalPage", new { id });
                }

                var profileContent = await profileResponse.Content.ReadAsStringAsync();
                var profile = JsonSerializer.Deserialize<PersonalPageDTO>(profileContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Truyền dữ liệu vào ViewBag
                ViewBag.IsLoggedIn = isLoggedIn;
                ViewBag.AccountIDLogin = accountIDLogin;
                ViewBag.Profile = profile;

                return View(photosDTO);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction("PersonalPage", new { id });
            }
        }
    }
}
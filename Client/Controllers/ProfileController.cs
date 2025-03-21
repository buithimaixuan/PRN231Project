using Client.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Security.Claims;
using System.Net.Http;

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
        public async Task<IActionResult> UpdateProfile(string view = "UpdateProfile")
        {
            // Kiểm tra trạng thái đăng nhập và lấy AccountIDLogin từ Claims
            var isLoggedIn = User.Identity.IsAuthenticated;
            int? accountIDLogin = null;
            try
            {
                if (isLoggedIn)
                {
                    var accountIdClaim = User.Claims.FirstOrDefault(c => c.Type == "AccountID");
                    if (accountIdClaim != null && int.TryParse(accountIdClaim.Value, out int accountIdFromCookie))
                    {
                        accountIDLogin = accountIdFromCookie;
                    }
                }

                // Gọi API để lấy thông tin tài khoản hiện tại
                string requestUrl = $"{_accountUrl}/personal-page/{accountIDLogin}";
                var response = await _clientProfile.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Failed to fetch profile: {errorContent}";
                    return RedirectToAction("PersonalPage", new { id = accountIDLogin });
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
                    return RedirectToAction("PersonalPage", new { id = accountIDLogin });
                }

                // Truyền dữ liệu vào ViewBag
                ViewBag.IsLoggedIn = isLoggedIn;
                ViewBag.AccountIDLogin = accountIDLogin;
                ViewBag.ViewMode = view;
                // Truyền AccountDTO vào View để hiển thị form
                return View(profile.accountDTO);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction("PersonalPage", new { id = accountIDLogin });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAvatar(int accountId, IFormFile avatarFile)
        {
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
            try
            {
                if (avatarFile == null || avatarFile.Length == 0)
                {
                    TempData["ErrorMessage"] = "Please select an image file.";
                    return RedirectToAction("Index");
                }

                // Chuẩn bị form data để gửi lên API
                using var formData = new MultipartFormDataContent();
                using var fileStream = avatarFile.OpenReadStream();
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(avatarFile.ContentType);
                formData.Add(fileContent, "AvatarFile", avatarFile.FileName);

                // Gọi API UpdateAvatar từ UserService
                string apiUrl = $"{_accountUrl}/UpdateAvatar/{accountId}";
                var response = await _clientProfile.PutAsync(apiUrl, formData);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var jsonDoc = JsonDocument.Parse(responseContent);
                    var avatarUrl = jsonDoc.RootElement.GetProperty("avatarUrl").GetString();

                    TempData["SuccessMessage"] = "Avatar updated successfully!";
                    return RedirectToAction("UpdateProfile", "Profile", new { id = accountIDLogin });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Failed to update avatar: {errorContent}";
                    return RedirectToAction("UpdateProfile", "Profile", new { id = accountIDLogin });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating avatar: {ex.Message}";
                return RedirectToAction("UpdateProfile", "Profile", new { id = accountIDLogin });
            }
        }

        // POST: /Profile/UpdateProfile
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(AccountDTO accountDTO)
        {
            try
            {
                // Kiểm tra trạng thái đăng nhập và lấy AccountIDLogin từ Claims
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

                // Kiểm tra xem accountDTO có hợp lệ không
                if (accountDTO == null)
                {
                    TempData["ErrorMessage"] = "Invalid profile data.";
                    return View(accountDTO);
                }

                // Map AccountDTO to UpdateAccountDTO
                var updateAccountDTO = new UpdateAccountDTO
                {
                    FullName = accountDTO.FullName,
                    ShortBio = accountDTO.ShortBio,
                    Gender = accountDTO.Gender,
                    Email = accountDTO.Email,
                    Phone = accountDTO.Phone,
                    DateOfBirth = accountDTO.DateOfBirth,
                    Address = accountDTO.Address,
                    EducationUrl = accountDTO.EducationUrl,
                    YearOfExperience = accountDTO.YearOfExperience,
                    DegreeUrl = accountDTO.DegreeUrl,
                    Major = accountDTO.Major
                };

                // Gọi API để cập nhật thông tin tài khoản
                var content = new StringContent(JsonSerializer.Serialize(updateAccountDTO), System.Text.Encoding.UTF8, "application/json");
                var response = await _clientProfile.PutAsync($"{_accountUrl}/Update/{accountIDLogin}", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Failed to update profile: {errorContent}";
                    return View(accountDTO);
                }

                TempData["SuccessMessage"] = "Profile updated successfully.";
                return RedirectToAction("UpdateProfile", new { id = accountIDLogin });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return View(accountDTO);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO model)
        {
            var isLoggedIn = User.Identity.IsAuthenticated;
            int? accountIDLogin = null;
            try
            {
                if (isLoggedIn)
                {
                    var accountIdClaim = User.Claims.FirstOrDefault(c => c.Type == "AccountID");
                    if (accountIdClaim != null && int.TryParse(accountIdClaim.Value, out int accountIdFromCookie))
                    {
                        accountIDLogin = accountIdFromCookie;
                    }
                }

                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Please fill in all required fields.";
                    return RedirectToAction("UpdateProfile", new { view = "ChangePassword" });
                }

                // Kiểm tra confirm password
                if (model.newPassword != model.confirmNewPassword)
                {
                    TempData["ErrorMessage"] = "New password and confirm password do not match.";
                    return RedirectToAction("UpdateProfile", new { view = "ChangePassword" });
                }

                // Gọi API để đổi mật khẩu
                string requestUrl = $"{_accountUrl}/ChangePassword/{accountIDLogin}";
                var content = new StringContent(JsonSerializer.Serialize(model), System.Text.Encoding.UTF8, "application/json");
                var response = await _clientProfile.PutAsync(requestUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Failed to change password: {errorContent}";
                    return RedirectToAction("UpdateProfile", new { view = "ChangePassword" });
                }

                TempData["SuccessMessage"] = "Password changed successfully!";
                return RedirectToAction("UpdateProfile", new { view = "UpdateProfile" });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction("UpdateProfile", new { view = "ChangePassword" });
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
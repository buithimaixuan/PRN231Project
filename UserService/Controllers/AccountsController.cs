using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Net.Http;
using System.Text.Json;
using UserService.DAOs;
using UserService.DTOs;
using UserService.Models;
using UserService.PasswordHashing;
using UserService.Services.Interface;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UserService.Controllers
{
    //đổi trong DBContext , appsetting, docker compose


    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly PasswordHasher _passwordHasher;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IEmailSenderService _emailSender;

        public AccountsController(IAccountService accountService, IHttpClientFactory httpClientFactory, PasswordHasher passwordHasher, IEmailSenderService emailSender)
        {
            _accountService = accountService;
            _passwordHasher = new PasswordHasher();
            _httpClientFactory = httpClientFactory;
            _passwordHasher = passwordHasher;
            _emailSender = emailSender;
        }





        [HttpGet("GetFullName/{id}")]
        public async Task<IActionResult> GetFullNameById(int id)
        {
            try
            {
                var account = await _accountService.GetByIdAccount(id);
                if (account == null)
                    return NotFound("Account not found.");

                return Ok(new { FullName = account.FullName });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }







        [HttpPut("UpdateAvatar/{id}")]
        public async Task<IActionResult> UpdateAvatar(int id, [FromForm] UpdateAvatarDTO request)
        {
            try
            {
                // Validate the request
                if (request == null || request.AvatarFile == null || request.AvatarFile.Length == 0)
                    return BadRequest("Invalid avatar file.");

                // Get the existing account
                var existingAccount = await _accountService.GetByIdAccount(id);
                if (existingAccount == null)
                    return NotFound("Account not found.");

                // Create HttpClient for PostService
                var client = _httpClientFactory.CreateClient("PostService");

                // Prepare the form data for the API call
                using var formData = new MultipartFormDataContent();
                using var fileStream = request.AvatarFile.OpenReadStream();
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(request.AvatarFile.ContentType);
                formData.Add(fileContent, "formFile", request.AvatarFile.FileName);

                // Call the PostService Cloudinary upload API
                var response = await client.PostAsync("https://localhost:7231/api/cloudinary/upload", formData);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return BadRequest($"Failed to upload avatar: {errorContent}");
                }

                // Parse the response to get the new avatar URL directly
                var responseContent = await response.Content.ReadAsStringAsync();
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;
                var uploadedAvatarUrl = root.GetProperty("imageUrl").GetString();

                if (string.IsNullOrEmpty(uploadedAvatarUrl))
                    return BadRequest("Failed to retrieve the uploaded avatar URL.");

                // Update the account's avatar URL (without deleting the old avatar)
                existingAccount.Avatar = uploadedAvatarUrl;
                await _accountService.UpdateAccount(existingAccount);

                // Return only the avatar URL
                return Ok(new { AvatarUrl = uploadedAvatarUrl });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating avatar: {ex.Message}");
            }
        }


        // GET: api/account
        [HttpGet("All")]
        public async Task<IActionResult> GetAllAccounts()
        {
            var accounts = await _accountService.GetListAllAccount();
            return Ok(accounts);
        }


        // GET: api/account
        [HttpGet("Available")]
        public async Task<IActionResult> GetAllAccountAvailable()
        {
            var accounts = await _accountService.GetAllAccountAvailable();
            return Ok(accounts);
        }

        

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] UpdateAccountDTO accountDTO)
        {
            if (accountDTO == null)
                return BadRequest("Invalid account.");

            var existingAccount = await _accountService.GetByIdAccount(id);
            if (existingAccount == null)
                return NotFound("Account not found.");

            existingAccount.FullName = accountDTO.FullName ?? null;
            existingAccount.ShortBio = accountDTO.ShortBio ?? null;
            existingAccount.Gender = accountDTO.Gender ?? null;
            existingAccount.Email = accountDTO.Email;
            existingAccount.Phone = accountDTO.Phone ?? null;
            existingAccount.DateOfBirth = accountDTO.DateOfBirth ?? null;
            existingAccount.Address = accountDTO.Address ?? null;
            existingAccount.EducationUrl = accountDTO.EducationUrl ?? null;
            existingAccount.YearOfExperience = accountDTO.YearOfExperience ?? null;
            existingAccount.DegreeUrl = accountDTO.DegreeUrl ?? null;
            existingAccount.Major = accountDTO.Major ?? null;

            await _accountService.UpdateAccount(existingAccount);
            return Ok(existingAccount);
        }


        [HttpPut("UpdateOTP/{id}")]
        public async Task<IActionResult> UpdateOTP(int id, [FromBody] OTPUpdateDTO accountDTO)
        {
            if (accountDTO == null)
                return BadRequest("Invalid account.");

            var existingAccount = await _accountService.GetByIdAccount(id);
            if (existingAccount == null)
                return NotFound("Account not found.");

            string repEmail = existingAccount.Email;

            string emailBody = $"Đây là mã OTP của bạn: {accountDTO.OTP}";

            _emailSender.SendEmail(repEmail, "Mã OTP mật khẩu", emailBody);

            existingAccount.FullName = accountDTO.FullName ?? null;
            existingAccount.ShortBio = accountDTO.ShortBio ?? null;
            existingAccount.Gender = accountDTO.Gender ?? null;
            existingAccount.Email = accountDTO.Email;
            existingAccount.Phone = accountDTO.Phone ?? null;
            existingAccount.DateOfBirth = accountDTO.DateOfBirth ?? null;
            existingAccount.Address = accountDTO.Address ?? null;
            existingAccount.EducationUrl = accountDTO.EducationUrl ?? null;
            existingAccount.YearOfExperience = accountDTO.YearOfExperience ?? null;
            existingAccount.DegreeUrl = accountDTO.DegreeUrl ?? null;
            existingAccount.Major = accountDTO.Major ?? null;
            existingAccount.Otp = accountDTO.OTP ?? null;

            await _accountService.UpdateAccount(existingAccount);
            return Ok(existingAccount);
        }

        // GET: api/account
        [HttpPut("ChangePassword/{id}")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordDTO changePasswordDTO)
        {
            var existingAccount = await _accountService.GetByIdAccount(id);
            if (existingAccount == null)
                return NotFound("No Account found!");

            if (!_passwordHasher.VerifyPassword(changePasswordDTO.currentPassword, existingAccount.Password))
                return BadRequest("Current password is incorrect!");

            if (changePasswordDTO.newPassword != changePasswordDTO.confirmNewPassword)
                return BadRequest("Confirm password does not match!");

            existingAccount.Password = _passwordHasher.HashPassword(changePasswordDTO.newPassword);
            return Ok("Password changed successfully!");
        }

        [HttpPut("SetPassword/{id}")]
        public async Task<IActionResult> SetPassword(int id, [FromBody] SetPasswordDTO request)
        {
            try
            {
                // Kiểm tra tài khoản tồn tại
                var account = await _accountService.GetByIdAccount(id);
                if (account == null)
                {
                    return NotFound("Account not found.");
                }

                // Kiểm tra xem tài khoản có được tạo bằng Google không (Otp = -1)
 /*               if (!account.Otp.HasValue || account.Otp != -1)
                {
                    return BadRequest("This account was not created using Google or already has a password set.");
                }*/

                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(request.NewPassword) || string.IsNullOrEmpty(request.ConfirmNewPassword))
                {
                    return BadRequest("New password and confirm password are required.");
                }

                // Kiểm tra mật khẩu mới và xác nhận mật khẩu có khớp không
                if (request.NewPassword != request.ConfirmNewPassword)
                {
                    return BadRequest("New password and confirm password do not match.");
                }

                // Hash mật khẩu mới
                account.Password = _passwordHasher.HashPassword(request.NewPassword);
                account.Otp = null; // Xóa dấu hiệu tài khoản Google

                // Cập nhật tài khoản
                await _accountService.UpdateAccount(account);

                return Ok("Password set successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nInner Exception: {ex.InnerException?.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message} - {ex.InnerException?.Message}");
            }
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                var account = await _accountService.GetAccountByEmail(email);
                if (account == null)
                    return NotFound($"No account found with email {email}");
                
                return Ok(account);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("personal-page/{accountId}")]
        public async Task<IActionResult> GetUserPersonalPage(int accountId)
        {
            try
            {
                var profile = await _accountService.GetPersonalPageDTO(accountId);
                if (profile == null)
                    return NotFound($"No account found with account_id {accountId}");
                
                return Ok(profile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all-photos/{accountId}")]
        public async Task<IActionResult> GetAccountAllPhotos(int accountId)
        {
            try
            {
                var photos = await _accountService.GetAccountPhotos(accountId);
                if (photos == null)
                    return NotFound($"No photos found with account_id {accountId}");
                
                return Ok(photos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("friend-requests/received/{accountId}")]
        public async Task<IActionResult> GetFriendRequestReceivers(int accountId)
        {
            try
            {
                var friendRequests = await _accountService.GetFriendRequestReceivers(accountId);
                if (friendRequests == null)
                    return NotFound($"No friend ferquests found with account {accountId}");
                
                return Ok(friendRequests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("friend-requests/sent/{accountId}")]
        public async Task<IActionResult> GetFriendRequestSenders(int accountId)
        {
            try
            {
                var friendRequests = await _accountService.GetFriendRequestSenders(accountId);
                if (friendRequests == null)
                    return NotFound($"No friend ferquests found with account {accountId}");
                
                return Ok(friendRequests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all-friends/{accountId}")]
        public async Task<IActionResult> GetListFriends(int accountId)
        {
            try
            {
                var friends = await _accountService.GetListFriends(accountId);
                if (friends == null)
                    return NotFound($"No friends found with account {accountId}");

                return Ok(friends);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Gửi yêu cầu kết bạn
        [HttpPost("friend-request/send")]
        public async Task<IActionResult> SendFriendRequest([FromBody] FriendRequestDTO request)
        {
            try
            {
                if (request.SenderId == request.ReceiverId)
                    return BadRequest("Cannot send a friend request to yourself.");

                var friendRequest = await _accountService.SendFriendRequest(request.SenderId, request.ReceiverId);
                return Ok(new { Message = "Friend request sent successfully.", RequestId = friendRequest.RequestId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Chấp nhận hoặc từ chối yêu cầu kết bạn
        [HttpPut("friend-request/update")]
        public async Task<IActionResult> UpdateFriendRequest([FromBody] FriendRequestDTO request)
        {
            try
            {
                await _accountService.UpdateFriendRequestStatus(request.SenderId, request.ReceiverId, request.RequestStatus);
                return Ok(new { Message = $"Friend request {request.RequestStatus} successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Hủy kết bạn
        [HttpDelete("friend/unfriend")]
        public async Task<IActionResult> Unfriend([FromQuery] int userId1, [FromQuery] int userId2)
        {
            try
            {
                await _accountService.Unfriend(userId1, userId2);
                return Ok(new { Message = "Successfully unfriended." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //*****************MINH UYEN***********

        [HttpPost("Farmers/Create")]
        public async Task<IActionResult> CreateFarmerAccount([FromBody] AccountDTO accountDTO)
        {
            if (accountDTO == null)
                return BadRequest("Invalid account data.");

            var existingAccount = await _accountService.GetAccountByIdentifier(accountDTO.Email);
            if (existingAccount != null)
                return Conflict("Account already exists.");

            string hashedPassword = _passwordHasher.HashPassword(accountDTO.Password);

            accountDTO.Password = hashedPassword;

            await _accountService.CreateNewFarmerAccount(accountDTO);

            return Ok("Farmer account created successfully!");
        }


        [HttpGet("AllFarmer")]
        public async Task<ActionResult<IEnumerable<Account>>> GetAllFarmer(int roleId)
        {
            IEnumerable<Account> List = await _accountService.GetListAccountByRoleId(2);
            return Ok(List);
        }


        [HttpGet("DetailFarmer{id}")]
        public async Task<IActionResult> GetAccountById(int id)
        {
            var news = await _accountService.GetByIdAccount(id);
            if (news == null)
                return NotFound("Account not found.");
            return Ok(news);
        }

        [HttpDelete("DeleteFarmer{id}")]
        public async Task<IActionResult> DeleteFarmer(int id)
        {
            var existingNews = await _accountService.GetByIdAccount(id);
            if (existingNews == null)
                return NotFound("Farmer not found.");

            await _accountService.DeleteAccount(existingNews);
            return Ok("Delete Account successfully!");
        }

        [HttpGet("total-farmers")]
        public async Task<IActionResult> GetTotalFarmers()
        {
            int totalFarmers = await _accountService.GetTotalFarmerService();
            return Ok(totalFarmers);
        }
        [HttpGet("total-experts")]
        public async Task<IActionResult> GetTotalExperts()
        {
            int totalExperts = await _accountService.GetTotalExpertService();
            return Ok(totalExperts);
        }

        //******************MINH UYEN***********


        // DELETE: api/Account/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var existingNews = await _accountService.GetByIdAccount(id);
            if (existingNews == null)
                return NotFound("Acco not found.");

            await _accountService.DeleteAccount(existingNews);
            return Ok("Delete Account successfully!");
        }


        [HttpGet("top-farmer")]
        public async Task<IActionResult> GetTopFarmer([FromServices] IAccountService accountService)
        {
            var topFarmer = await accountService.GetTopFarmer();

            if (topFarmer == null) return NotFound("Không tìm thấy Farmer nào có bài post!");

            return Ok(new
            {
                AccountId = topFarmer.AccountId,
                Username = topFarmer.Username,
                FullName = topFarmer.FullName
            });
        }

        // Mai Xuân crud expert
        [HttpGet("ALlExpert/{roleId}")]
        public async Task<ActionResult<IEnumerable<Account>>> GetAllExpert(int roleId)
        {
            IEnumerable<Account> List = await _accountService.GetListAccountByRoleId(roleId);
            return Ok(List);
        }

        [HttpPost("Experts/Create")]
        public async Task<IActionResult> CreateExpertAccount([FromBody] AccountDTO accountDTO)
        {
            if (accountDTO == null)
                return BadRequest("Invalid account data.");

            var existingAccount = await _accountService.GetAccountByIdentifier(accountDTO.Email);
            if (existingAccount != null)
                return Conflict("Account already exists.");

            string hashedPassword = _passwordHasher.HashPassword(accountDTO.Password);

            accountDTO.Password = hashedPassword;

            await _accountService.CreateNewExpertAccount(accountDTO);

            return Ok("Farmer account created successfully!");
        }
    }
}

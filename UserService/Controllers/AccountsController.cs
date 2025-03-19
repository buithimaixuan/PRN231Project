using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using UserService.DAOs;
using UserService.DTOs;
using UserService.PasswordHashing;
using UserService.Services.Interface;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly PasswordHasher _passwordHasher;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
            _passwordHasher = new PasswordHasher();
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
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] AccountDTO accountDTO)
        {
            if (accountDTO == null)
                return BadRequest("Invalid account.");

            var existingAccount = await _accountService.GetByIdAccount(id);
            if (existingAccount == null)
                return NotFound("Account not found.");

            string hashedPassword = _passwordHasher.HashPassword(accountDTO.Password);

            existingAccount.RoleId = existingAccount.RoleId; // mặc định 3 (Farmer)
            existingAccount.Username = accountDTO.Username;
            existingAccount.Password = hashedPassword;
            existingAccount.Email = accountDTO.Email;
            existingAccount.Phone = accountDTO.Phone ?? null;
            existingAccount.Gender = accountDTO.Gender ?? null;
            existingAccount.FullName = accountDTO.FullName ?? null;
            existingAccount.DateOfBirth = accountDTO.DateOfBirth ?? null;
            existingAccount.ShortBio = accountDTO.ShortBio ?? null;
            existingAccount.EducationUrl = accountDTO.EducationUrl ?? null;
            existingAccount.YearOfExperience = accountDTO.YearOfExperience ?? null;
            existingAccount.DegreeUrl = accountDTO.DegreeUrl ?? null;
            existingAccount.Avatar = accountDTO.Avatar ?? null;
            existingAccount.Major = accountDTO.Major ?? null;
            existingAccount.Address = accountDTO.Address ?? null;
            existingAccount.IsDeleted = false;
            existingAccount.Otp = null;

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
                var friendRequest = await _accountService.SendFriendRequest(request.SenderId, request.ReceiverId);
                if (request == null)
                    return NotFound($"No friends found with account");

                if (request.SenderId == request.RequestId)
                    return NotFound($"Cannot send a friend requset to yourself.");

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

        //*****************MINH UYEN***********

        [HttpPost("createFarmer")]
        public async Task<IActionResult> CreateFarmerAccount([FromBody] AccountDTO accountDTO)
        {
            if (accountDTO == null)
                return BadRequest("Invalid account data.");

            var existingAccount = await _accountService.GetAccountByIdentifier(accountDTO.Email);
            if (existingAccount != null)
                return Conflict("Account already exists.");

            string hashedPassword = _passwordHasher.HashPassword(accountDTO.Password);

            await _accountService.CreateNewFarmerAccount(
                accountDTO.Username,
                hashedPassword,
                accountDTO.FullName,
                accountDTO.Email,
                accountDTO.Phone,
                accountDTO.Address,
                accountDTO.Avatar
            );

            return Ok("Farmer account created successfully!");
        }


        [HttpGet("AllFarmer")]
        public async Task<IActionResult> GetAllAccountsFarmer(int roleId)
        {
            var accounts = await _accountService.GetListAccountByRoleId(roleId);
            return Ok(accounts);
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


    }
}

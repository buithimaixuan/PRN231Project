using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserService.DTOs;
using UserService.PasswordHashing;
using UserService.Services.Interface;

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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountById(int id)
        {
            var news = await _accountService.GetByIdAccount(id);
            if (news == null)
                return NotFound("Account not found.");
            return Ok(news);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] AccountDTO accountDTO)
        {
            if (accountDTO == null)
                return BadRequest("Invalid account.");

            var existingAccount = await _accountService.GetByIdAccount(id);
            if (existingAccount == null)
                return NotFound("Account not found.");

            string hashedPassword = _passwordHasher.HashPassword(accountDTO.Password);

            existingAccount.RoleId = accountDTO.RoleId; // mặc định 3 (Farmer)
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

        // DELETE: api/Account/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var existingNews = await _accountService.GetByIdAccount(id);
            if (existingNews == null)
                return NotFound("News not found.");

            await _accountService.DeleteAccount(existingNews);
            return Ok("Delete Account successfully!");
        }
    }
}

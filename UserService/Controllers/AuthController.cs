using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserService.DTOs;
using UserService.Models;
using UserService.PasswordHashing;
using UserService.Services.Interface;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly PasswordHasher _passwordHasher;
        public AuthController(IAccountService accountService)
        {
            _accountService = accountService;
            _passwordHasher = new PasswordHasher();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AccountDTO accountDTO)
        {
            try
            {
                // Kiểm tra các trường bắt buộc
                if (string.IsNullOrEmpty(accountDTO.Email) || string.IsNullOrEmpty(accountDTO.Username) || string.IsNullOrEmpty(accountDTO.Password))
                {
                    return BadRequest("Email, username, and password are required.");
                }

                // Kiểm tra xem email hoặc username đã tồn tại chưa
                var existingAccountByPhone = await _accountService.GetAccountByPhone(accountDTO.Phone);
                if (existingAccountByPhone != null)
                {
                    return Conflict("Phone already exists.");
                }

                // Kiểm tra xem email hoặc username đã tồn tại chưa
                var existingAccountByEmail = await _accountService.GetAccountByEmail(accountDTO.Email);
                if (existingAccountByEmail != null)
                {
                    return Conflict("Email already exists.");
                }

                var existingAccountByUsername = await _accountService.GetByUsername(accountDTO.Username);
                if (existingAccountByUsername != null)
                {
                    return Conflict("Username already exists.");
                }

                // Hash password
                string hashedPassword = _passwordHasher.HashPassword(accountDTO.Password);

                // Tạo đối tượng Account để lưu
                var account = new Account
                {
                    RoleId = 3, // mặc định 3 (Farmer)
                    Username = accountDTO.Username,
                    Password = hashedPassword,
                    Email = accountDTO.Email,
                    EmailConfirmed = 0, // Mặc định chưa xác nhận email
                    Phone = accountDTO.Phone ?? null, // Sử dụng giá trị từ DTO hoặc null
                    PhoneConfirmed = 0,
                    Gender = accountDTO.Gender ?? null,
                    FullName = accountDTO.FullName ?? null,
                    DateOfBirth = accountDTO.DateOfBirth ?? null,
                    ShortBio = accountDTO.ShortBio ?? null,
                    EducationUrl = accountDTO.EducationUrl ?? null,
                    YearOfExperience = accountDTO.YearOfExperience ?? null,
                    DegreeUrl = accountDTO.DegreeUrl ?? null,
                    Avatar = accountDTO.Avatar ?? null,
                    Major = accountDTO.Major ?? null,
                    Address = accountDTO.Address ?? null,
                    IsDeleted = false,
                    Otp = null
                };

                // Thêm tài khoản vào database
                await _accountService.AddAccount(account);

                return Ok(new { Message = "Registration successful." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nInner Exception: {ex.InnerException?.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message} - {ex.InnerException?.Message}");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(loginDTO?.Identifier) || string.IsNullOrEmpty(loginDTO?.Password))
                {
                    return BadRequest("Identifier (Email, Username, or PhoneNumber) and password are required.");
                }

                // Tìm tài khoản bằng identifier (Email, Username, hoặc PhoneNumber)
                var account = await _accountService.GetAccountByIdentifier(loginDTO.Identifier);
                if (account == null)
                {
                    return Unauthorized("Invalid identifier or password.");
                }

                // Kiểm tra trạng thái tài khoản
                if (account.IsDeleted.HasValue && account.IsDeleted.Value)
                {
                    return Unauthorized("Account has been deleted.");
                }

                // Xác minh password
                bool isPasswordValid = _passwordHasher.VerifyPassword(loginDTO.Password, account.Password);
                if (!isPasswordValid)
                {
                    return Unauthorized("Invalid identifier or password.");
                }

                // Đăng nhập thành công, trả về thông tin cơ bản (có thể mở rộng để trả về token JWT)
                return Ok(new
                {
                    Message = "Login successful.",
                    AccountId = account.AccountId,
                    Username = account.Username,
                    Email = account.Email,
                    Phone = account.Phone,
                    RoleId = account.RoleId
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nInner Exception: {ex.InnerException?.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message} - {ex.InnerException?.Message}");
            }
        }
    }
}

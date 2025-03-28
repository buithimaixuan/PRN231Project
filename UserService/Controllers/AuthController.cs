﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using UserService.Config;
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
        private readonly AuthenConfig _config;
        private readonly Services.Interface.IEmailSender _emailSender;

        public AuthController(IAccountService accountService, AuthenConfig config, IEmailSender emailSender)
        {
            _accountService = accountService;
            _passwordHasher = new PasswordHasher();
            _config = config;
            _emailSender = emailSender;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AccountDTO accountDTO)
        {
            try
            {
                // Kiểm tra các trường bắt buộc
                if (string.IsNullOrEmpty(accountDTO.Username) || string.IsNullOrEmpty(accountDTO.Password))
                {
                    return BadRequest("Email, username, and password are required.");
                }

                // Kiểm tra xem email hoặc username đã tồn tại chưa
                if (accountDTO.Phone != null)
                {
                    var existingAccountByPhone = await _accountService.GetAccountByPhone(accountDTO.Phone);
                    if (existingAccountByPhone != null)
                    {
                        return Conflict("Phone already exists.");
                    }
                }

                // Kiểm tra xem email hoặc username đã tồn tại chưa
                if (accountDTO.Email != null)
                {
                    var existingAccountByEmail = await _accountService.GetAccountByEmail(accountDTO.Email);
                    if (existingAccountByEmail != null)
                    {
                        return Conflict("Email already exists.");
                    }
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
                    RoleId = accountDTO.RoleId, // mặc định 3 (Farmer)
                    Username = accountDTO.Username,
                    Password = hashedPassword,
                    Email = accountDTO.Email ?? null,
                    EmailConfirmed = accountDTO.EmailConfirmed ?? 0, // Mặc định chưa xác nhận email
                    Phone = accountDTO.Phone ?? null,
                    PhoneConfirmed = 0,
                    Gender = accountDTO.Gender ?? null,
                    FullName = accountDTO.FullName ?? null,
                    DateOfBirth = accountDTO.DateOfBirth ?? null,
                    ShortBio = accountDTO.ShortBio ?? null,
                    EducationUrl = accountDTO.EducationUrl ?? null,
                    YearOfExperience = accountDTO.YearOfExperience ?? null,
                    DegreeUrl = accountDTO.DegreeUrl ?? null,
                    Avatar = accountDTO.Avatar ?? "https://firebasestorage.googleapis.com/v0/b/prn221-69738.appspot.com/o/image%2F638665051034994468_av.jpg?alt=media&token=be337fe1-d4bb-4e4b-9495-dbb921b4779a",
                    Major = accountDTO.Major ?? null,
                    Address = accountDTO.Address ?? null,
                    IsDeleted = false,
                    Otp = accountDTO.Otp ?? null
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
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var result = await _config.Authenticate(loginDTO);
            if (result == null)
            {
                return Unauthorized();
            }

            return Ok(result);
        }

        [HttpPost("send-email")] 
        public async Task<IActionResult> SendEmail([FromBody] SenderDTO request)
        {
            if (string.IsNullOrEmpty(request.To) || string.IsNullOrEmpty(request.Subject) || string.IsNullOrEmpty(request.Body))
            {
                return BadRequest("All fields are required.");
            }

            try
            {
                await _emailSender.SendEmailAsync(request.To, request.Subject, request.Body);
                return Ok("Email sent successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to send email: {ex.Message}");
            }
        }


        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email))
                {
                    return BadRequest("Email is required.");
                }

                // Kiểm tra xem email đã tồn tại chưa
                var existingAccount = await _accountService.GetAccountByEmail(request.Email);
                int accountId;
                int roleId;
                string username;
                string email;
                string phone;

                if (existingAccount == null)
                {
                    // Nếu chưa tồn tại, tạo tài khoản mới
                    var newAccount = new Account
                    {
                        RoleId = 2, // Mặc định là Farmer
                        Username = request.Email.Split('@')[0],
                        Password = _passwordHasher.HashPassword(Guid.NewGuid().ToString()), // Tạo mật khẩu ngẫu nhiên
                        Email = request.Email,
                        EmailConfirmed = 1, // Đánh dấu email đã xác nhận
                        FullName = request.FullName ?? request.Email.Split('@')[0],
                        Avatar = "https://firebasestorage.googleapis.com/v0/b/prn221-69738.appspot.com/o/image%2F638665051034994468_av.jpg?alt=media&token=be337fe1-d4bb-4e4b-9495-dbb921b4779a",
                        IsDeleted = false,
                        Otp = -1
                    };

                    await _accountService.AddAccount(newAccount);
                    accountId = newAccount.AccountId;
                    roleId = newAccount.RoleId;
                    username = newAccount.Username;
                    email = newAccount.Email;
                    phone = newAccount.Phone;
                }
                else
                {
                    // Nếu đã tồn tại, sử dụng thông tin tài khoản hiện có
                    if (existingAccount.IsDeleted.HasValue && existingAccount.IsDeleted.Value)
                    {
                        return Unauthorized("Account has been deleted.");
                    }

                    accountId = existingAccount.AccountId;
                    roleId = existingAccount.RoleId;
                    username = existingAccount.Username;
                    email = existingAccount.Email;
                    phone = existingAccount.Phone;
                }

                // Tạo token
                var loginDTO = new LoginDTO
                {
                    Identifier = email
                };
                string token = _config.GenerateToken(loginDTO);

                return Ok(new LoginResponseDTO
                {
                    Message = "Google login successful.",
                    AccountId = accountId,
                    Username = username,
                    Email = email,
                    Phone = phone,
                    RoleId = roleId,
                    Token = token
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
